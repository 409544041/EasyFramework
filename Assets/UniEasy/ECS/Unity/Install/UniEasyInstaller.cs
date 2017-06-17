using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UniEasy.Console;
using UnityEngine;
using System.Linq;
using UniEasy.DI;
using UniRx;

namespace UniEasy.ECS
{
	public class UniEasyInstaller : Installer<UniEasyInstaller>
	{
		public static DiContainer ProjectContainer { get; set; }

		[RuntimeInitializeOnLoadMethod (RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void Initialize ()
		{
			ProjectContainer = new DiContainer ();
			ProjectContainer.Bind<DiContainer> ().FromInstance (ProjectContainer).AsSingle ();
			UniEasyInstaller.Install (ProjectContainer);

			var scenesPool = new Dictionary<Scene, DiContainer> ();
			SceneManager.sceneLoaded += (scene, mode) => {
				if (mode == LoadSceneMode.Single) {
					var list = scenesPool.Select (x => x.Value).ToArray ();
					for (int i = 0; i < list.Length; i++) {
						list [i].UnbindAll ();
					}
					scenesPool.Clear ();
					scenesPool.Add (scene, ProjectContainer.CreateSubContainer ());
				} else {
					if (scenesPool.ContainsKey (scene)) {
						scenesPool [scene].UnbindAll ();
					} else {
						scenesPool.Add (scene, ProjectContainer.CreateSubContainer ());
					}
				}

				var container = scenesPool [scene];
				container.Bind<DiContainer> ().FromInstance (container).AsSingle ();
				for (int i = 0; i < scene.rootCount; i++) {
					var list = scene.GetRootGameObjects () [i].GetComponentsInChildren<MonoInstaller> ();
					for (int j = 0; j < list.Length; j++) {
						container.Inject (list [j]);
						list [j].InstallBindings ();
					}
				}
				for (int i = 0; i < scene.rootCount; i++) {
					var systems = scene.GetRootGameObjects () [i].GetComponentsInChildren<SystemBehaviour> ().ToArray<object> ();
					var components = scene.GetRootGameObjects () [i].GetComponentsInChildren<ComponentBehaviour> ().ToArray<object> ();
					var list = systems.Union (components).ToArray ();
					for (int j = 0; j < list.Length; j++) {
						container.Inject (list [j]);
					}
				}
			};
		}

		public override void InstallBindings ()
		{
			Container.Bind<IMessageBroker> ().To<MessageBroker> ().AsSingle ();
			Container.Bind<IEventSystem> ().To<EventSystem> ().AsSingle ();
			Container.Bind<IIdentityGenerator> ().To<SequentialIdentityGenerator> ().AsSingle ();
			Container.Bind<IPoolManager> ().To<PoolManager> ().AsSingle ();
			Container.Bind<GroupFactory> ().To<GroupFactory> ().AsSingle ();
			Container.Bind<PrefabFactory> ().To<PrefabFactory> ().AsSingle ();

			InstallComponentBehaviour<DebugCanvas> ();
			InstallComponentBehaviour<DebugView> ();
			InstallSystemBehaviour<DebugSystem> ();
			InstallComponentBehaviour<ConsoleView> ();
			InstallSystemBehaviour<Consoler> ();
			CommandInstaller.Install (Container);
		}

		public T InstallSystemBehaviour<T> () where T : Component
		{
			var system = new GameObject (typeof(T).Name).AddComponent<T> ();
			GameObject.DontDestroyOnLoad (system);
			Container.Inject (system);
			Container.Bind (typeof(T)).FromInstance (system).AsSingle ();
			return (T)system;
		}

		public T InstallComponentBehaviour<T> () where T : Component
		{
			var go = new GameObject (typeof(T).Name);
			var entityBehaviour = go.AddComponent<EntityBehaviour> ();
			var component = go.AddComponent<T> ();
			Container.Inject (entityBehaviour);
			GameObject.DontDestroyOnLoad (component);
			return (T)component;
		}
	}
}

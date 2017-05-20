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
			var DebugSystem = GameObject.FindObjectOfType<DebugSystem> () ??
			                  new GameObject ("DebugSystem").AddComponent<DebugSystem> ();
			GameObject.DontDestroyOnLoad (DebugSystem);
			Container.Inject (DebugSystem);
			Container.Bind<DebugSystem> ().FromInstance (DebugSystem).AsSingle ();

			var Console = new GameObject ("Console");
			var ConsoleSystem = new GameObject ("ConsoleSystem");
			var ConsoleComponent = new GameObject ("ConsoleComponent");
			ConsoleSystem.transform.SetParent (Console.transform);
			ConsoleComponent.transform.SetParent (Console.transform);
			var ConsoleEntity = ConsoleComponent.AddComponent<EntityBehaviour> ();
			ConsoleComponent.AddComponent<ConsoleView> ();
			Container.Inject (ConsoleEntity);
			var ConsoleController = ConsoleSystem.AddComponent<Consoler> ();
			Container.Inject (ConsoleController);
			GameObject.DontDestroyOnLoad (Console);
			Container.Bind<Consoler> ().FromInstance (ConsoleController).AsSingle ();
			CommandInstaller.Install (Container);
		}
	}
}

using UnityEngine;
using UniEasy.DI;
using UniRx;

namespace UniEasy.ECS
{
	public class UniEasyInstaller : Installer<UniEasyInstaller>
	{
		[RuntimeInitializeOnLoadMethod (RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void Initialize ()
		{
			DiContainer container = new DiContainer ();
			container.Bind<DiContainer> ().FromInstance (container).AsSingle ();

			UniEasyInstaller.Install (container);
			var installers = GameObject.FindObjectsOfType<MonoInstallerBase> ();
			for (int i = 0; i < installers.Length; i++) {
				container.Inject (installers [i]);
				installers [i].InstallBindings ();
			}
		}

		public override void InstallBindings ()
		{
			IBehaviourExtensions.Container = Container;
			Container.Bind<IMessageBroker> ().To<MessageBroker> ().AsSingle ();
			Container.Bind<IEventSystem> ().To<EventSystem> ().AsSingle ();
			Container.Bind<IIdentityGenerator> ().To<SequentialIdentityGenerator> ().AsSingle ();
			Container.Bind<IPoolManager> ().To<PoolManager> ().AsSingle ();
			Container.Bind<GroupFactory> ().To<GroupFactory> ().AsSingle ();
			var DebugSystem = GameObject.FindObjectOfType<DebugSystem> () ??
			                  new GameObject ("DebugSystem").AddComponent<DebugSystem> ();
			GameObject.DontDestroyOnLoad (DebugSystem);
			Container.Bind<DebugSystem> ().FromInstance (DebugSystem).AsSingle ();
		}
	}
}
using UnityEngine;
using UniEasy.DI;
using UniRx;

namespace UniEasy.ECS
{
	public class FootstoneInstaller : Installer<FootstoneInstaller>
	{
		[RuntimeInitializeOnLoadMethod (RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void Initialize ()
		{
			DiContainer container = new DiContainer ();
			container.Bind<DiContainer> ().FromInstance (container).AsSingle ();

			FootstoneInstaller.Install (container);
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
		}
	}
}
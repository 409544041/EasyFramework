using UnityEngine;
using UniEasy.DI;

namespace UniEasy.ECS
{
	public class FootstoneInstaller : Installer<FootstoneInstaller>
	{
		[RuntimeInitializeOnLoadMethod]
		public static void Initialize ()
		{
			DiContainer container = new DiContainer ();
			container.Bind<DiContainer> ().FromInstance (container).AsSingle ();
			container.InjectSelf ();

			FootstoneInstaller.Install (container);
			var installers = GameObject.FindObjectsOfType<MonoInstallerBase> ();
			for (int i = 0; i < installers.Length; i++) {
				container.Inject (installers [i]);
				installers [i].InstallBindings ();
			}
		}

		public override void InstallBindings ()
		{
			
		}
	}
}
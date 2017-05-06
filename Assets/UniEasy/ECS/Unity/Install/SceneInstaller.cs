using UnityEngine;
using UniEasy.DI;

namespace UniEasy.ECS
{
	public class SceneInstaller : MonoInstaller
	{
		public override void InstallBindings ()
		{
			var systems = GetComponentsInChildren<SystemBehaviour> ();
			for (int i = 0; i < systems.Length; i++) {
				Container.Bind (systems [i].GetType ()).FromInstance (systems [i]).AsSingle ();
			}
		}
	}
}

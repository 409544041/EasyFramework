using UniEasy.DI;

namespace UniEasy.ECS
{
	public static class IBehaviourExtensions
	{
		public static DiContainer Container {
			get;
			set;
		}

		public static void InjectSelf (this IBehaviour behaviour)
		{
			Container.Inject (behaviour);
		}
	}
}

using UniEasy.DI;

namespace UniEasy.ECS
{
	public static class IBehaviourExtensions
	{
		public static void InjectSelf (this IBehaviour behaviour)
		{
			DiContainerExtensions.Container.Inject (behaviour);
		}
	}
}

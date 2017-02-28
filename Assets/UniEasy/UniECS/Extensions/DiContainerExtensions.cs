using UniEasy.DI;

namespace UniEasy.ECS
{
	public static class DiContainerExtensions
	{
		public static void InjectSelf (this DiContainer container)
		{
			container.Inject (container);
		}
	}
}

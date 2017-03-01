using UniEasy.DI;

namespace UniEasy.ECS
{
	public static class DiContainerExtensions
	{
		public static DiContainer Container {
			get;
			set;
		}

		public static void InjectSelf (this DiContainer container)
		{
			Container = container;
			Container.Inject (container);
		}
	}
}

namespace UniEasy
{
	public class FromBinder : ScopeBinder
	{
		public FromBinder (BindInfo bindInfo, DiContainer container) : base (bindInfo)
		{
			Container = container;
		}

		public DiContainer Container {
			get;
			private set;
		}
	}
}

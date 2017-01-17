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

		protected ScopeBinder FromInstanceBase (object instance)
		{
//			BindInfo.ContractTypes
//			var provider = new InstanceProvider (type, instance);
//			Container.RegisterProvider ();
			return new ScopeBinder (BindInfo);
		}
	}
}

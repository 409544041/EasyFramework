namespace UniEasy
{
	public class FromBinder : ScopeBinder
	{
		protected IBindingFinalizer BindingFinalizer;

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
			BindingFinalizer = new ScopableBindingFinalizer (BindInfo, (container, concreteType) => {
				return new InstanceProvider (concreteType, instance);
			});
			BindInfo.DistinctUntilChanged += FlushBindings;
			FlushBindings ();
			return this;
		}

		protected virtual void FlushBindings ()
		{
			if (BindingFinalizer == null) {
				return;
			}
			BindingFinalizer.FinalizeBinding (Container);
		}
	}
}

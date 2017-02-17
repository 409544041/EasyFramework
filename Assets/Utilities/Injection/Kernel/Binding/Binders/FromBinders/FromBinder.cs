namespace UniEasy
{
	public class FromBinder : ScopeBinder
	{
		protected IBindingFinalizer BindingFinalizer;

		public FromBinder (BindInfo bindInfo, BindFinalizerWrapper finalizerWrapper) : base (bindInfo)
		{
			FinalizerWrapper = finalizerWrapper;
		}

		protected BindFinalizerWrapper FinalizerWrapper {
			get;
			private set;
		}

		protected IBindingFinalizer SubFinalizer {
			set {
				FinalizerWrapper.SubFinalizer = value;
			}
		}

		protected ScopeBinder FromInstanceBase (object instance)
		{
			SubFinalizer = new ScopableBindingFinalizer (BindInfo, (container, concreteType) => {
				return new InstanceProvider (concreteType, instance);
			});
			return this;
		}
	}
}

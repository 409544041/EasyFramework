using UniRx;

namespace UniEasy
{
	public class ScopeBinder : ConditionBinder
	{
		protected IBindingFinalizer BindingFinalizer {
			get;
			set;
		}

		public ScopeBinder (BindInfo bindInfo) : base (bindInfo)
		{

		}

		public ConditionBinder AsSingle ()
		{
			return AsSingle (null);
		}

		public ConditionBinder AsSingle (object concreteIdentifier)
		{
			BindInfo.Scope = ScopeTypes.Singleton;
			BindInfo.ConcreteIdentifier = concreteIdentifier;
			MessageBroker.Default.Publish<IBindingFinalizer> (BindingFinalizer);
			return this;
		}
	}
}

using System.Linq;
using UniRx;

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
			BindingFinalizer = new ScopableBindingFinalizer (BindInfo, (container, concreteType) => {
				return new InstanceProvider (concreteType, instance);
			});
			MessageBroker.Default.Publish<IBindingFinalizer> (BindingFinalizer);
			return this;
		}
	}
}

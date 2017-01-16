namespace UniEasy
{
	public class ConcreteIdBinderGeneric<TContract> : ConcreteBinderGeneric<TContract>
	{
		public ConcreteIdBinderGeneric (BindInfo bindInfo, DiContainer container) : base (bindInfo, container)
		{
		}

		public ConcreteBinderGeneric<TContract> WithId (object identifier)
		{
			BindInfo.Identifier = identifier;
			return this;
		}
	}
}

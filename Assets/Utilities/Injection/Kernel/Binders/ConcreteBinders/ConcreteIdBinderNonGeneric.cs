namespace UniEasy
{
	public class ConcreteIdBinderNonGeneric : ConcreteBinderNonGeneric
	{
		public ConcreteIdBinderNonGeneric (BindInfo bindInfo, DiContainer container) : base (bindInfo, container)
		{
		}

		public ConcreteBinderNonGeneric WithId (object identifier)
		{
			BindInfo.Identifier = identifier;
			return this;
		}
	}
}

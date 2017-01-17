namespace UniEasy
{
	public class FromBinderNonGeneric : FromBinder
	{
		public FromBinderNonGeneric (BindInfo bindInfo, DiContainer container) : base (bindInfo, container)
		{
		}

		public ScopeBinder FromInstance (object instance)
		{
			return FromInstanceBase (instance);
		}
	}
}

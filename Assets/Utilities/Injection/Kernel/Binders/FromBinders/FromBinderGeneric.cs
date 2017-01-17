using System;

namespace UniEasy
{
	public class FromBinderGeneric<TContract> : FromBinder
	{
		public FromBinderGeneric (BindInfo bindInfo, DiContainer container) : base (bindInfo, container)
		{
		}

		public ScopeBinder FromInstance (TContract instance)
		{
			return FromInstanceBase (instance);
		}
	}
}

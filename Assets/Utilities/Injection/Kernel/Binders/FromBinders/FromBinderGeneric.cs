using System;

namespace UniEasy
{
	public class FromBinderGeneric<TContract> : FromBinder
	{
		public FromBinderGeneric (BindInfo bindInfo, DiContainer container) : base (bindInfo, container)
		{
		}

		public ScopeBinder From ()
		{
			for (int i = 0; i < BindInfo.ContractTypes.Count; i++) {
				for (int j = 0; j < BindInfo.ToTypes.Count; j++) {
					if (ValidateBindTypes (BindInfo.ContractTypes [i], BindInfo.ToTypes [j])) {

					}
				}
			}
			return new ScopeBinder (BindInfo);
		}

		bool ValidateBindTypes (Type concreteType, Type contractType)
		{
			return true;
		}
	}
}

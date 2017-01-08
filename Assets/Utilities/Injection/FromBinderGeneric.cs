using System;
using System.Linq;

namespace UniEasy
{
	public class FromBinderGeneric<TContract> : ScopeBinder
	{
		public FromBinderGeneric (BindInfo bindInfo) : base (bindInfo)
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

	public class FromBinderNonGeneric : ScopeBinder
	{
		public FromBinderNonGeneric (BindInfo bindInfo) : base (bindInfo)
		{
		}

		public ScopeBinder From ()
		{
			return new ScopeBinder (BindInfo);
		}
	}
}

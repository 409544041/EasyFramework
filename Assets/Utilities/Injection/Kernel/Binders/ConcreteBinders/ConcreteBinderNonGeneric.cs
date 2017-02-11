using System.Collections.Generic;
using System;
using System.Linq;

namespace UniEasy
{
	public class ConcreteBinderNonGeneric : FromBinderNonGeneric
	{
		public ConcreteBinderNonGeneric (BindInfo bindInfo, DiContainer container) : base (bindInfo, container)
		{
		}

		public FromBinderNonGeneric To<TConcrete> ()
		{
			return To (typeof(TConcrete));
		}

		public FromBinderNonGeneric To (params Type[] concreteTypes)
		{
			return To ((IEnumerable<Type>)concreteTypes);
		}

		public FromBinderNonGeneric To (IEnumerable<Type> concreteTypes)
		{
			BindInfo.ToTypes = concreteTypes.ToList ();
			return this;
		}
	}
}

using System.Collections.Generic;
using System;
using System.Linq;

namespace UniEasy
{
	public class ConcreteBinderGeneric<TContract> : FromBinderGeneric<TContract>
	{
		public ConcreteBinderGeneric (BindInfo bindInfo, DiContainer container) : base (bindInfo, container)
		{
		}

		public FromBinderGeneric<TConcrete> To<TConcrete> ()
			where TConcrete : TContract
		{
			BindInfo.ToTypes = new List<Type> () { typeof(TConcrete) };
			return new FromBinderGeneric<TConcrete> (BindInfo, Container);
		}

		public FromBinderNonGeneric To (params Type[] concreteTypes)
		{
			return To ((IEnumerable<Type>)concreteTypes);
		}

		public FromBinderNonGeneric To (IEnumerable<Type> concreteTypes)
		{
			BindInfo.ToTypes = concreteTypes.ToList ();
			return new FromBinderNonGeneric (BindInfo, Container);
		}
	}
}

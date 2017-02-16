using System.Collections.Generic;
using System;
using System.Linq;

namespace UniEasy
{
	public class ConcreteBinderGeneric<TContract> : FromBinderGeneric<TContract>
	{
		public ConcreteBinderGeneric (BindInfo bindInfo, BindFinalizerWrapper finalizerWrapper) : base (bindInfo, finalizerWrapper)
		{
		}

		public FromBinderGeneric<TConcrete> To<TConcrete> ()
			where TConcrete : TContract
		{
			BindInfo.ToTypes = new List<Type> () { typeof(TConcrete) };
			return new FromBinderGeneric<TConcrete> (BindInfo, FinalizerWrapper);
		}

		public FromBinderNonGeneric To (params Type[] concreteTypes)
		{
			return To ((IEnumerable<Type>)concreteTypes);
		}

		public FromBinderNonGeneric To (IEnumerable<Type> concreteTypes)
		{
			BindInfo.ToTypes = concreteTypes.ToList ();
			return new FromBinderNonGeneric (BindInfo, FinalizerWrapper);
		}
	}
}

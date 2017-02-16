﻿using System;

namespace UniEasy
{
	public class FromBinderGeneric<TContract> : FromBinder
	{
		public FromBinderGeneric (BindInfo bindInfo, BindFinalizerWrapper finalizerWrapper) : base (bindInfo, finalizerWrapper)
		{
		}

		public ScopeBinder FromInstance (TContract instance)
		{
			return FromInstanceBase (instance);
		}
	}
}

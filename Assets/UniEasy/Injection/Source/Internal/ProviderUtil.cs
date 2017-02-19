using System;

namespace UniEasy
{
	public static class ProviderUtil
	{
		public static Type GetTypeToInstantiate (Type contractType, Type concreteType)
		{
			if (concreteType.IsOpenGenericType ()) {
				return contractType;
			}

			return concreteType;
		}
	}
}

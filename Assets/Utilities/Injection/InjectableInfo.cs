using System;

namespace UniEasy
{
	public class InjectableInfo
	{
		public readonly Type MemberType;
		public readonly Action<object, object> Setter;

		public InjectableInfo (Type memberType)
		{
			MemberType = memberType;
//			Setter = setter;
		}
	}
}

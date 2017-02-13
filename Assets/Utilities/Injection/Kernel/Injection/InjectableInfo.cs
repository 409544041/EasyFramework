using System;

namespace UniEasy
{
	public class InjectableInfo
	{
		public readonly Type MemberType;
		public readonly Action<object, object> Setter;
		public readonly object Identifier;

		public InjectableInfo (Type memberType, object identifier, Action<object, object> setter)
		{
			Identifier = identifier;
			MemberType = memberType;
			Setter = setter;
		}
	}
}

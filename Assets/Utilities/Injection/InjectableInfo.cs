using System;
using UniRx;

namespace UniEasy
{
	public class InjectableInfo
	{
		public readonly Type MemberType;
		public readonly Action<object, object> Setter;

		public InjectableInfo (Type memberType, Action<object, object> setter)
		{
			MemberType = memberType;
			Setter = setter;

//			MessageBroker.Default.Receive<ProviderInfo> ()
//				.Where (info => MemberType == info.)
//				.Subscribe (info => {
//					
//			});
		}
	}
}

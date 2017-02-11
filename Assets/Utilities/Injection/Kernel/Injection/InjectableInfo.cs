using System;
using UniRx;

namespace UniEasy
{
	public class InjectableInfo
	{
		public object Entity;
		public readonly Type MemberType;
		public readonly Action<object, object> Setter;
		public readonly object Identifier;

		public InjectableInfo (Type memberType, object identifier, Action<object, object> setter)
		{
			Identifier = identifier;
			MemberType = memberType;
			Setter = setter;

			MessageBroker.Default.Receive<InjectContext> ()
				.Where (context => Entity != null && context.GetBindingId () == new BindingId (MemberType, Identifier))
				.Subscribe (context => {
				var value = context.Container.Resolve (context.GetBindingId ());
				Setter (Entity, value); 
			});
		}
	}
}

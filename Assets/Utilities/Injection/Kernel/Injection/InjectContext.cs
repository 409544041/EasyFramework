using System;

namespace UniEasy
{
	public class InjectContext
	{
		public InjectContext ()
		{
		}

		public InjectContext (DiContainer container, Type memberType) : this ()
		{
			Container = container;
			MemberType = memberType;
		}

		public InjectContext (DiContainer container, Type memberType, object identifier)
			: this (container, memberType)
		{
			Identifier = identifier;
		}

		// Identifier - most of the time this is null
		public object Identifier {
			get;
			set;
		}

		// The type of the constructor parameter, field or property
		public Type MemberType {
			get;
			set;
		}

		// The container used for this injection
		public DiContainer Container {
			get;
			set;
		}

		public Type ObjectType {
			get;
			set;
		}

		public object ObjectInstance {
			get;
			set;
		}

		public BindingId GetBindingId ()
		{
			return new BindingId (MemberType, Identifier);
		}
	}
}

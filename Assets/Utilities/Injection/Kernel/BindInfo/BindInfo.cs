using System.Collections.Generic;
using System;
using UniRx;
using System.Linq;

namespace UniEasy
{
	public enum ScopeTypes
	{
		Transient,
		Singleton,
	}

	public class BindInfo
	{
		public BindInfo (List<Type> contractTypes)
		{
			NonLazyReactiveProperty.DistinctUntilChanged ().Subscribe (_ => {
				FlushWhenChanged ();
			});
			ContractTypesReactiveCollection.ObserveEveryValueChanged (x => x).Subscribe (_ => {
				FlushWhenChanged ();
			});
			ToTypesReactiveCollection.ObserveEveryValueChanged (x => x).Subscribe (_ => {
				FlushWhenChanged ();
			});
			ConditionReactiveProperty.DistinctUntilChanged ().Subscribe (_ => {
				FlushWhenChanged ();
			});
			ScopeReactiveProperty.DistinctUntilChanged ().Subscribe (_ => {
				FlushWhenChanged ();
			});

			Identifier = null;
			ContractTypes = contractTypes;
			ToTypes = new List<Type> ();
			NonLazy = false;
			Scope = ScopeTypes.Transient;
		}

		public BindInfo (Type contractType) : this (new List<Type> () { contractType })
		{
		}

		public object Identifier {
			get;
			set;
		}

		private ReactiveProperty<bool> NonLazyReactiveProperty = new ReactiveProperty<bool> ();

		public bool NonLazy {
			get {
				return NonLazyReactiveProperty.Value;
			}
			set {
				NonLazyReactiveProperty.Value = value;
			}
		}

		private ReactiveCollection<Type> ContractTypesReactiveCollection = new ReactiveCollection<Type> ();

		public List<Type> ContractTypes {
			get {
				return ContractTypesReactiveCollection.ToList<Type> ();
			}
			set {
				ContractTypesReactiveCollection = new ReactiveCollection<Type> (value);
			}
		}

		private ReactiveCollection<Type> ToTypesReactiveCollection = new ReactiveCollection<Type> ();

		public List<Type> ToTypes {
			get {
				return ToTypesReactiveCollection.ToList<Type> ();
			}
			set {
				ToTypesReactiveCollection = new ReactiveCollection<Type> (value);
			}
		}

		private ReactiveProperty<BindingCondition> ConditionReactiveProperty = new ReactiveProperty<BindingCondition> ();

		public BindingCondition Condition {
			get {
				return ConditionReactiveProperty.Value;
			}
			set {
				ConditionReactiveProperty.Value = value;
			}
		}

		private ReactiveProperty<ScopeTypes> ScopeReactiveProperty = new ReactiveProperty<ScopeTypes> ();

		public ScopeTypes Scope {
			get {
				return ScopeReactiveProperty.Value;
			}
			set {
				ScopeReactiveProperty.Value = value;
			}
		}

		public object ConcreteIdentifier {
			get;
			set;
		}

		public event Action DistinctUntilChanged;

		private void FlushWhenChanged ()
		{
			if (DistinctUntilChanged != null) {
				DistinctUntilChanged ();
			}
		}
	}
}

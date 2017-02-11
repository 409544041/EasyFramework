using System.Collections.Generic;
using System.Linq;
using System;
using UniRx;

namespace UniEasy
{
	public class BindInfo
	{
		public BindInfo (List<Type> contractTypes)
		{
			this.identifier = new ReactiveProperty<object> ();
			this.nonLazy = new ReactiveProperty<bool> ();
			this.contractTypes = new ReactiveCollection<Type> ();
			this.toTypes = new ReactiveCollection<Type> ();

			Identifier = null;
			ContractTypes = contractTypes;
			ToTypes = new List<Type> ();
			NonLazy = false;

			nonLazy.DistinctUntilChanged ().Where (b => b).Subscribe (_ => {
				
			});
		}

		public BindInfo (Type contractType) : this (new List<Type> () { contractType })
		{
		}

		private ReactiveProperty<object> identifier;
		private ReactiveProperty<bool> nonLazy;
		private ReactiveCollection<Type> contractTypes;
		private ReactiveCollection<Type> toTypes;

		public object Identifier {
			get {
				return identifier.Value;
			}
			set {
				identifier.Value = value;
			}
		}

		public bool NonLazy {
			get {
				return nonLazy.Value;
			}
			set {
				nonLazy.Value = value;
			}
		}

		public List<Type> ContractTypes {
			get {
				return contractTypes.ToList<Type> ();
			}
			set {
				contractTypes = new ReactiveCollection<Type> (value);
			}
		}

		public List<Type> ToTypes {
			get {
				return toTypes.ToList<Type> ();
			}
			set {
				toTypes = new ReactiveCollection<Type> (value);
			}
		}

		public BindingCondition Condition {
			get;
			set;
		}
	}
}

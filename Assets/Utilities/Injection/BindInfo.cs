using System.Collections.Generic;
using System;
using UniRx;
using System.Linq;

namespace UniEasy
{
	public class BindInfo
	{
		public BindInfo (List<Type> contractTypes)
		{
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
	}
}

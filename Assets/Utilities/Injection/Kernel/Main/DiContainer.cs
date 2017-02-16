using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using UniRx;

namespace UniEasy
{
	public delegate bool BindingCondition (InjectContext context);

	public class ProviderInfo
	{
		public ProviderInfo (IProvider provider, BindingCondition condition)
		{
			Provider = provider;
			Condition = condition;
		}

		public IProvider Provider {
			get;
			private set;
		}

		public BindingCondition Condition {
			get;
			private set;
		}
	}

	public class DiContainer
	{
		readonly Dictionary<BindingId, List<ProviderInfo>> providers = new Dictionary<BindingId, List<ProviderInfo>> ();
		readonly SingletonProviderCreator singletonProviderCreator;
		readonly Queue<IBindingFinalizer> currentBindings = new Queue<IBindingFinalizer> ();

		public DiContainer ()
		{
			singletonProviderCreator = new SingletonProviderCreator (this);

			Inject (this);
		}

		public SingletonProviderCreator SingletonProviderCreator {
			get {
				return singletonProviderCreator;
			}
		}

		public void Inject (object injectable)
		{
			FlushBindings ();
			var type = injectable.GetType ();
			var typeInfo = TypeAnalyzer.GetInfo (type);
			var injectInfos = typeInfo.FieldInjectables.Concat (typeInfo.PropertyInjectables).ToArray ();
			for (int i = 0; i < injectInfos.Length; i++) {
				var injectInfo = injectInfos [i];
				var injectContext = injectInfo.CreateInjectContext (this, injectable);
				injectInfo.Setter (injectable, Resolve (injectContext));
			}
		}

		// Do not use this - it is for internal use only
		public void FlushBindings ()
		{
			while (currentBindings.Any ()) {
				var binding = currentBindings.Dequeue ();
				binding.FinalizeBinding (this);
			}
		}

		public BindFinalizerWrapper StartBinding ()
		{
			FlushBindings ();
			var bindingFinalizer = new BindFinalizerWrapper ();
			currentBindings.Enqueue (bindingFinalizer);
			return bindingFinalizer;
		}

		public ConcreteIdBinderGeneric<TContract> Bind<TContract> ()
		{
			var bindInfo = new BindInfo (typeof(TContract));
			return new ConcreteIdBinderGeneric<TContract> (bindInfo, StartBinding ());
		}

		public ConcreteIdBinderNonGeneric Bind (params Type[] contractTypes)
		{
			return Bind ((IEnumerable<Type>)contractTypes);
		}

		public ConcreteIdBinderNonGeneric Bind (IEnumerable<Type> contractTypes)
		{
			var contractTypesList = contractTypes.ToList ();
			var bindInfo = new BindInfo (contractTypesList);
			return new ConcreteIdBinderNonGeneric (bindInfo, StartBinding ());
		}

		public void RegisterProvider (BindingId bindingId, BindingCondition condition, IProvider provider)
		{
			var info = new ProviderInfo (provider, condition);

			if (providers.ContainsKey (bindingId)) {
				providers [bindingId].Add (info);
			} else {
				providers.Add (bindingId, new List<ProviderInfo> () { info });
			}
		}

		public object Resolve (InjectContext context)
		{
			FlushBindings ();
			IProvider provider;
			var result = TryGetUniqueProvider (context, out provider);
			if (result) {
				var runner = provider.GetAllInstancesWithInjectSplit ();

				// First get instance
				bool hasMore = runner.MoveNext ();

				var instances = runner.Current;

				// Now do injection
				while (hasMore) {
					hasMore = runner.MoveNext ();
				}

				return instances.SingleOrDefault ();
			}
			return null;
		}

		internal bool TryGetUniqueProvider (InjectContext context, out IProvider provider)
		{
			var providers = GetProviderMatchesInternal (context).ToList ();
			if (!providers.Any ()) {
				provider = null;
				return false;
			}
			if (providers.Count > 1) {
				provider = providers.LastOrDefault ().Provider;
			} else {
				provider = providers.Single ().Provider;
			}
			return true;
		}

		IEnumerable<ProviderInfo> GetProviderMatchesInternal (InjectContext context)
		{
			return GetProvidersForContract (context.GetBindingId ()).Where (x => x.Condition == null || x.Condition (context));
		}

		IEnumerable<ProviderInfo> GetProvidersForContract (BindingId bindingId)
		{
			return GetLocalProviders (bindingId).Select (x => x);
		}

		List<ProviderInfo> GetLocalProviders (BindingId bindingId)
		{
			List<ProviderInfo> localProviders;
			if (providers.TryGetValue (bindingId, out localProviders)) {
				return localProviders;
			}
			return new List<ProviderInfo> ();
		}
	}
}

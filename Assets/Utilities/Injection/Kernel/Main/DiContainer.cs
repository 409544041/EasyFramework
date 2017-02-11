using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using UniRx;

namespace UniEasy
{
	public delegate bool BindingCondition ();

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
		readonly ReactiveDictionary<BindingId, List<ProviderInfo>> providers = new ReactiveDictionary<BindingId, List<ProviderInfo>> ();

		public DiContainer ()
		{
			providers.ObserveAdd ().Subscribe (info => {
				MessageBroker.Default.Publish<InjectContext> (new InjectContext (this, info.Key.Type));
			});

			Inject (this);
		}

		public void Inject (object entity)
		{
			var type = entity.GetType ();
			var typeInfo = TypeAnalyzer.GetInfo (type);
			var injectInfos = typeInfo.FieldInjectables.Concat (typeInfo.PropertyInjectables).ToArray ();
			for (int i = 0; i < injectInfos.Length; i++) {
				var bindingId = new BindingId (injectInfos [i].MemberType, injectInfos [i].Identifier);
				var value = Resolve (bindingId);
				injectInfos [i].Entity = entity;
				injectInfos [i].Setter (entity, value);
			}
		}

		public ConcreteBinderGeneric<TContract> Bind<TContract> ()
		{
			var bindInfo = new BindInfo (typeof(TContract));
			return new ConcreteBinderGeneric<TContract> (bindInfo, this);
		}

		public void RegisterProvider (BindingId bindingId, BindingCondition condition, IProvider provider)
		{
			var info = new ProviderInfo (provider, condition);

			if (providers.ContainsKey (bindingId)) {
				providers [bindingId].Add (info);
			} else {
				providers.Add (bindingId, new List<ProviderInfo> { info });
			}
		}

		public object Resolve (BindingId bindingId)
		{
			IProvider provider;
			var result = TryGetUniqueProvider (bindingId, out provider);
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

		internal bool TryGetUniqueProvider (BindingId bindingId, out IProvider provider)
		{
			var providers = GetProviderMatchesInternal (bindingId).ToList ();
			if (!providers.Any ()) {
				provider = null;
				return false;
			}
			if (providers.Count > 1) {
				provider = providers.ToArray () [0].Provider;
			} else {
				provider = providers.Single ().Provider;
			}
			return true;
		}

		IEnumerable<ProviderInfo> GetProviderMatchesInternal (BindingId bindingId)
		{
			return GetProvidersForContract (bindingId).Where (x => x.Condition == null || x.Condition ());
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

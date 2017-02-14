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

		public void Inject (object entity)
		{
			var type = entity.GetType ();
			var typeInfo = TypeAnalyzer.GetInfo (type);
			var injectInfos = typeInfo.FieldInjectables.Concat (typeInfo.PropertyInjectables).ToArray ();
			for (int i = 0; i < injectInfos.Length; i++) {
				var value = Resolve (injectInfos [i].CreateInjectContext (this, entity));
				var injectInfo = injectInfos [i];
				injectInfo.Setter (entity, value);

				MessageBroker.Default.Receive<InjectContext> ()
					.Where (context => context.Container == this && context.GetBindingId () == new BindingId (injectInfo.MemberType, injectInfo.Identifier))
					.Subscribe (context => {
					var val = context.Container.Resolve (context);
					injectInfo.Setter (entity, val);
				});
			}
		}

		public ConcreteIdBinderGeneric<TContract> Bind<TContract> ()
		{
			var bindInfo = new BindInfo (typeof(TContract));
			return new ConcreteIdBinderGeneric<TContract> (bindInfo, this);
		}

		public ConcreteIdBinderNonGeneric Bind (params Type[] contractTypes)
		{
			return Bind ((IEnumerable<Type>)contractTypes);
		}

		public ConcreteIdBinderNonGeneric Bind (IEnumerable<Type> contractTypes)
		{
			var contractTypesList = contractTypes.ToList ();
			var bindInfo = new BindInfo (contractTypesList);
			return new ConcreteIdBinderNonGeneric (bindInfo, this);
		}

		public void RegisterProvider (BindingId bindingId, BindingCondition condition, IProvider provider)
		{
			var info = new ProviderInfo (provider, condition);

			if (providers.ContainsKey (bindingId)) {
				providers [bindingId].Add (info);
			} else {
				providers.Add (bindingId, new List<ProviderInfo> () { info });
			}

			MessageBroker.Default.Publish<InjectContext> (new InjectContext (this, bindingId.Type, bindingId.Identifier));
		}

		public object Resolve (InjectContext context)
		{
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
			var providerInfo = GetProvidersForContract (context.GetBindingId ());
			var output = providerInfo.Where (x => x.Condition == null || x.Condition (context));
			if (output != null) {
				foreach (ProviderInfo info in output.ToList ()) {
					Debug.Log (">>" + info.Condition);
				}
			}
			return providerInfo.Where (x => x.Condition == null || x.Condition (context));
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

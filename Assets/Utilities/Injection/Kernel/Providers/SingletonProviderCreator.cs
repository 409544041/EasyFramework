using System;
using System.Collections.Generic;

namespace UniEasy
{
	public class SingletonProviderCreator
	{
		readonly Dictionary<BindingId, ProviderInfo> providerMap = new Dictionary<BindingId, ProviderInfo> ();
		readonly DiContainer container;

		public SingletonProviderCreator (DiContainer container)
		{
			this.container = container;
		}

		public IProvider GetOrCreateProvider (BindingId bindingId, Func<DiContainer, Type, IProvider> providerCreator)
		{
			ProviderInfo providerInfo;

			if (providerMap.TryGetValue (bindingId, out providerInfo)) {

			} else {
				providerInfo = new ProviderInfo (
					new CachedProvider (
						providerCreator (container, bindingId.Type)));
				providerMap.Add (bindingId, providerInfo);
			}

			return providerInfo.Provider;
		}

		public class ProviderInfo
		{
			public ProviderInfo (CachedProvider provider)
			{
				Provider = provider;
			}

			public CachedProvider Provider {
				get;
				private set;
			}
		}
	}
}

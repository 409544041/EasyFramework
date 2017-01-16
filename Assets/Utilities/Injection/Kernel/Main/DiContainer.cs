using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using UniRx;

namespace UniEasy
{
	public class ProviderInfo
	{
		readonly BindInfo bindInfo;

		public ProviderInfo (BindInfo bindInfo)
		{
			this.bindInfo = bindInfo;
		}
	}

	public class DiContainer
	{
		readonly ReactiveDictionary<BindingId, List<ProviderInfo>> providers = new ReactiveDictionary<BindingId, List<ProviderInfo>> ();

		public DiContainer ()
		{
			Inject (this);

			providers.ObserveAdd ().Subscribe (info => {
				MessageBroker.Default.Publish<ProviderInfo> (info.Value [info.Value.Count - 1]);
			});
		}

		public void Inject (object entity)
		{
			var type = entity.GetType ();
			var typeInfo = TypeAnalyzer.GetInfo (type);
			var injectInfos = typeInfo.FieldInjectables.Concat (typeInfo.PropertyInjectables).ToArray ();
			for (int i = 0; i < injectInfos.Length; i++) {
//				injectInfos [i].Setter (entity, value); 
			}
		}

		public ConcreteBinderGeneric<TContract> Bind<TContract> ()
		{
			var bindInfo = new BindInfo (typeof(TContract));
			return new ConcreteBinderGeneric<TContract> (bindInfo, this);
		}

		public void RegisterProvider (BindingId bindingId, BindInfo bindInfo)
		{
			var info = new ProviderInfo (bindInfo);

			if (providers.ContainsKey (bindingId)) {
				providers [bindingId].Add (info);
			} else {
				providers.Add (bindingId, new List<ProviderInfo> { info });
			}
		}
	}
}

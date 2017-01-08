using UnityEngine;
using System.Collections.Generic;

namespace UniEasy
{
	public class ProviderInfo
	{
		public ProviderInfo ()
		{
		}
	}

	public class DiContainer
	{
		readonly Dictionary<BindingId, List<ProviderInfo>> providers = new Dictionary<BindingId, List<ProviderInfo>> ();

		public DiContainer ()
		{
//			Inject (this);
		}

		//		public void Inject (object entity)
		//		{
		//			var bindingId = new BindingId (entity.GetType (), entity);
		//			Inject (entity, bindingId);
		//		}
		//
		//		public void Inject (object entity, string bindingName)
		//		{
		//			if (!string.IsNullOrEmpty (bindingName)) {
		//				var type = entity.GetType ();
		//				Inject (entity, new BindingId (type, bindingName));
		//			} else
		//				Debug.LogError ("sorry, binding id can not be empty!");
		//		}
		//
		//		public void Inject (object entity, BindingId bindingId)
		//		{
		//			var type = entity.GetType ();
		//			TypeAnalyzer.GetInfo (entity);
		//
		//			if (providers.ContainsKey (type)) {
		//				var typeInfo = providers [type];
		//				if (typeInfo.ContainsKey (bindingId)) {
		//					typeInfo [bindingId] = entity;
		//					Debug.LogError ("already have same binding id object exist, the original object will be replaced");
		//				} else
		//					typeInfo.Add (bindingId, entity);
		//				providers [type] = typeInfo;
		//			} else
		//				providers.Add (type, new Dictionary<BindingId, object> () { { bindingId, entity } });
		//		}

		public BinderGeneric<TContract> Bind<TContract> ()
		{
			return new BinderGeneric<TContract> ();
		}
	}
}

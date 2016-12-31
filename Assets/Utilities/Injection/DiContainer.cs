using UnityEngine;
using System.Collections.Generic;

namespace UniEasy
{
	public class DiContainer
	{
		readonly Dictionary<System.Type, Dictionary<BindingId, object>> providers = new Dictionary<System.Type, Dictionary<BindingId, object>> ();

		public DiContainer ()
		{
			Inject<DiContainer> (this);
		}

		public void Inject<TContract> (TContract entity)
		{
			BindingId bindingId;
			var type = entity.GetType ();
			if (type.IsSubclassOf (typeof(Object)))
				bindingId = new BindingId (type, (entity as Object).name);
			else {
				bindingId = new BindingId (type, null);
				Debug.LogWarning ("");
			}
			
			Inject<TContract> (entity, bindingId);
		}

		public void Inject<TContract> (TContract entity, string bindingName)
		{
			if (!string.IsNullOrEmpty (bindingName)) {
				var type = entity.GetType ();
				Inject<TContract> (entity, new BindingId (type, bindingName));
			} else
				Debug.LogError ("");
		}

		public void Inject<TContract> (TContract entity, BindingId bindingId)
		{
			var type = entity.GetType ();
			TypeAnalyzer.GetInfo (entity);

			if (providers.ContainsKey (type)) {
				var typeInfo = providers [type];
				if (typeInfo.ContainsKey (bindingId)) {
					typeInfo [bindingId] = entity;
					Debug.LogError ("");
				} else
					typeInfo.Add (bindingId, entity);
				providers [type] = typeInfo;
			} else
				providers.Add (type, new Dictionary<BindingId, object> () { { bindingId, entity } });
		}

		public BinderGeneric Bind<TContract> ()
		{
			var type = typeof(TContract);
//			if (providers.ContainsKey (type)) {
//				return new BinderGeneric (providers [type]);
//			}
			return new BinderGeneric ();
		}
	}
}

using System.Collections.Generic;

namespace UniEasy
{
	public class BinderGeneric
	{
//		readonly Dictionary<string, object> entities = Dictionary<string, object> ();

		public BinderGeneric ()
		{
		}

		public BinderGeneric (Dictionary<string, object> entities)
		{
//			this.entities = entities;
		}

		public BindFinalizer To<TContract> ()
		{
//			if (entities.Count > 0) {
//				EasyInjectInfo injectInfo = TypeAnalyzer.GetInfo<TContract> ();
//				new BindFinalizer (typeof(TContract), injectInfo);
//			} else
//				new BindFinalizer (typeof(TContract));
			return null;
		}
	}
}

using System;
using System.Linq;

namespace UniEasy
{
	public class BindFinalizer
	{
		readonly Type type;
		readonly EasyInjectInfo injectInfo = null;

		public BindFinalizer (Type type)
		{
			this.type = type;
		}

		public BindFinalizer (Type type, EasyInjectInfo injectInfo)
		{
			this.type = type;
			this.injectInfo = injectInfo;
		}

		public void NonLazy ()
		{
//			if (injectInfo != null) {
//				var injectables = injectInfo.AllInjectables.ToArray<InjectableInfo> ();
//				for (int i = 0; i < injectables.Count; i++) {
//					if (injectables [i].MemberType == type) {
//						injectables [i].Setter ();
//					}
//				}
//			}
		}
	}
}

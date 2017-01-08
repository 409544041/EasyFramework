using System.Collections.Generic;

namespace UniEasy
{
	public class EasyInjectInfo
	{
		readonly object entity;
		readonly List<InjectableInfo> fieldInjectables;

		public EasyInjectInfo (object entity, List<InjectableInfo> fieldInjectables)
		{
			this.entity = entity;
			this.fieldInjectables = fieldInjectables;
		}

		public IEnumerable<InjectableInfo> AllInjectables {
			get {
				return this.fieldInjectables;
			}
		}
	}
}

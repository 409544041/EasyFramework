using System.Collections.Generic;
using System.Linq;

namespace UniEasy
{
	public class EasyInjectInfo
	{
		readonly List<InjectableInfo> fieldInjectables;
		readonly List<InjectableInfo> propertyInjectables;

		public EasyInjectInfo (List<InjectableInfo> fieldInjectables,
		                       List<InjectableInfo> propertyInjectables)
		{
			this.fieldInjectables = fieldInjectables;
			this.propertyInjectables = propertyInjectables;
		}

		public IEnumerable<InjectableInfo> FieldInjectables {
			get {
				return fieldInjectables;
			}
		}

		public IEnumerable<InjectableInfo> PropertyInjectables {
			get {
				return propertyInjectables;
			}
		}

		public IEnumerable<InjectableInfo> AllInjectables {
			get {
				return this.fieldInjectables.Concat (this.propertyInjectables);
			}
		}
	}
}

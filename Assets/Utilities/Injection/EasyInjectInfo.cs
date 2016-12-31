using System.Collections.Generic;

namespace UniEasy
{
	public class EasyInjectInfo
	{
		readonly object _entity;
		readonly List<InjectableInfo> _fieldInjectables;

		public EasyInjectInfo ()
		{
		}

		public EasyInjectInfo (object entity, List<InjectableInfo> fieldInjectables)
		{
			_entity = entity;
			_fieldInjectables = fieldInjectables;
		}

		public IEnumerable<InjectableInfo> AllInjectables {
			get {
				return _fieldInjectables;
			}
		}
	}
}

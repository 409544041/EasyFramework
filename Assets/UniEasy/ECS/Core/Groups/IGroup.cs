using System.Collections.Generic;
using System;
using UniRx;

namespace UniEasy.ECS
{
	public interface IGroup
	{
		IEventSystem EventSystem { get; set; }

		IPool EntityPool { get; set; }

		string Name { get; set; }

		ReactiveCollection<IEntity> Entities { get; set; }

		IEnumerable<Type> Components { get; set; }

		Predicate<IEntity> Predicate { get; }
	}
}

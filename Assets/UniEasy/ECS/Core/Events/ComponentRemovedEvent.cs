namespace UniEasy.ECS
{
	public class ComponentRemovedEvent
	{
		public IEntity Entity { get; private set; }

		public object Component { get; private set; }

		public ComponentRemovedEvent (IEntity entity, object component)
		{
			Entity = entity;
			Component = component;
		}
	}
}

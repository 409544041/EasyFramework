namespace UniEasy.ECS
{
	public interface ISystem
	{
		IEventSystem EventSystem { get; set; }

		IPoolManager PoolManager { get; set; }
	}
}

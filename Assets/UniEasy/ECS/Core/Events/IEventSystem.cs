using UniRx;

namespace UniEasy.ECS
{
	public interface IEventSystem
	{
		void Publish<T> (T message);

		IObservable<T> Receive<T> ();
	}
}

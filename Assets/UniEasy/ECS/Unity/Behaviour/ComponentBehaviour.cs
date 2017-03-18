using UnityEngine;
using System;
using UniRx;

namespace UniEasy.ECS
{
	public class ComponentBehaviour : MonoBehaviour, IBehaviour, IDisposable
	{
		public IEventSystem EventSystem {
			get {
				return eventSystem == null ? this.Resolve<IEventSystem> () : eventSystem;
			}
			set { eventSystem = value; }
		}

		private IEventSystem eventSystem;

		public IPoolManager PoolManager {
			get {
				return poolManager == null ? this.Resolve<IPoolManager> () : poolManager;
			}
			set { poolManager = value; }
		}

		private IPoolManager poolManager;

		private CompositeDisposable disposer = new CompositeDisposable ();

		public CompositeDisposable Disposer {
			get { return disposer; }
			set { disposer = value; }
		}

		protected virtual void Awake ()
		{
			this.InjectSelf ();
		}

		void Start ()
		{

		}

		public virtual void OnDestroy ()
		{
			Dispose ();
		}

		public virtual void Dispose ()
		{
			Disposer.Dispose ();
		}
	}
}
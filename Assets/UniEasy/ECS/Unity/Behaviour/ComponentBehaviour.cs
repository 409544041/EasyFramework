using UnityEngine;
using UniEasy.DI;
using System;
using UniRx;

namespace UniEasy.ECS
{
	public class ComponentBehaviour : MonoBehaviour, IDisposable
	{
		public IEventSystem EventSystem {
			get {
				return eventSystem == null ? UniEasyInstaller.ProjectContainer.Resolve<IEventSystem> () : eventSystem;
			}
			set { eventSystem = value; }
		}

		private IEventSystem eventSystem;

		public IPoolManager PoolManager {
			get {
				return poolManager == null ? UniEasyInstaller.ProjectContainer.Resolve<IPoolManager> () : poolManager;
			}
			set { poolManager = value; }
		}

		private IPoolManager poolManager;

		private CompositeDisposable disposer = new CompositeDisposable ();

		public CompositeDisposable Disposer {
			get { return disposer; }
			set { disposer = value; }
		}

		[Inject]
		public virtual void Setup ()
		{

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

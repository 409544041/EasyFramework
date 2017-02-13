using UniRx;

namespace UniEasy
{
	public class NonLazyBinder
	{
		public NonLazyBinder (BindInfo bindInfo)
		{
			this.bindInfo.DistinctUntilChanged ().Subscribe (_ => {
				FlushBindings ();
			});
			BindInfo = bindInfo;
		}

		protected ReactiveProperty<BindInfo> bindInfo = new ReactiveProperty<BindInfo> ();

		protected BindInfo BindInfo {
			get {
				return bindInfo.Value;
			}
			private set {
				bindInfo.Value = value;
			}
		}

		public void NonLazy ()
		{
			BindInfo.NonLazy = true;
		}

		protected virtual void FlushBindings ()
		{
		}
	}
}

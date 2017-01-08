namespace UniEasy
{
	public class ScopeBinder : ConditionBinder
	{
		[Inject]
		protected DiContainer container;

		public ScopeBinder (BindInfo bindInfo) : base (bindInfo)
		{
		}

		public ConditionBinder AsSingle ()
		{
			return new ConditionBinder (BindInfo);
		}
	}
}

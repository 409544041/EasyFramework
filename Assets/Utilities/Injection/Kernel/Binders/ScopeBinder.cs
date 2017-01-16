namespace UniEasy
{
	public class ScopeBinder : ConditionBinder
	{
		public ScopeBinder (BindInfo bindInfo) : base (bindInfo)
		{
		}

		public ConditionBinder AsSingle ()
		{
			return new ConditionBinder (BindInfo);
		}
	}
}

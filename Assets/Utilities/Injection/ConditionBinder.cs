namespace UniEasy
{
	public class ConditionBinder : NonLazyBinder
	{
		public ConditionBinder (BindInfo bindInfo) : base (bindInfo)
		{
		}

		public NonLazyBinder When ()
		{
			return new NonLazyBinder (BindInfo);
		}
	}
}

namespace UniEasy
{
	public class ScopeBinder
	{
		public ScopeBinder ()
		{
			
		}

		public ConditionBinder AsSingle ()
		{
			return new ConditionBinder ();
		}
	}
}

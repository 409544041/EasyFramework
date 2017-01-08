namespace UniEasy
{
	public class FromBinderGeneric<TContract>
	{
		public FromBinderGeneric ()
		{
			
		}

		public ScopeBinder From ()
		{
			return new ScopeBinder ();
		}
	}
}

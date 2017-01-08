namespace UniEasy
{
	public class BinderGeneric<TContract>
	{
		public BinderGeneric ()
		{
			
		}

		public FromBinderGeneric<TConcrete> To<TConcrete> ()
			where TConcrete : TContract
		{
			return new FromBinderGeneric<TConcrete> ();
		}
	}
}

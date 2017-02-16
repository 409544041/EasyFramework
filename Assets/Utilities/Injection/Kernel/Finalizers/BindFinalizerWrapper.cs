namespace UniEasy
{
	public class BindFinalizerWrapper : IBindingFinalizer
	{
		IBindingFinalizer subFinalizer;

		public IBindingFinalizer SubFinalizer {
			set {
				subFinalizer = value;
			}
		}

		public void FinalizeBinding (DiContainer container)
		{
			subFinalizer.FinalizeBinding (container);
		}
	}
}

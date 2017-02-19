namespace UniEasy
{
	public interface IInstaller
	{
		void InstallBindings (DiContainer container);

		bool IsEnabled {
			get;
		}
	}
}

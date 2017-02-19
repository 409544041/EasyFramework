namespace UniEasy
{
	public abstract class InstallerBase : IInstaller
	{
		public virtual bool IsEnabled {
			get {
				return true;
			}
		}

		public abstract void InstallBindings (DiContainer container);
	}
}

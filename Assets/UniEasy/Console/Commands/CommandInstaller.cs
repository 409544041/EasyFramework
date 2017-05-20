using UniEasy.DI;

namespace UniEasy.Console
{
	public class CommandInstaller : Installer<CommandInstaller>
	{
		public override void InstallBindings ()
		{
			CommandLibrary.RegisterCommand (HelpCommand.name, HelpCommand.description, HelpCommand.usage, HelpCommand.Execute);
			CommandLibrary.RegisterCommand (QuitCommand.name, QuitCommand.description, QuitCommand.usage, QuitCommand.Execute);
			CommandLibrary.RegisterCommand (LoadSceneCommand.name, LoadSceneCommand.description, LoadSceneCommand.usage, LoadSceneCommand.Execute);
		}
	}
}

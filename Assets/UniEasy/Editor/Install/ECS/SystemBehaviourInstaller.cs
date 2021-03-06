﻿using UnityEditor;

namespace UniEasy.Edit
{
	public class SystemBehaviourInstaller : ScriptAssetInstaller
	{
		[MenuItem ("Assets/Create/UniEasy/SystemBehaviour Installer", false, 22)]
		static public void CreateSystemBehaviour ()
		{
			new SystemBehaviourInstaller ().Create ();
		}

		public override string GetName ()
		{
			return "NewSystemBehaviour.cs";
		}

		public override string GetContents ()
		{
			return "using UnityEngine;" +
			"\nusing UniEasy.ECS;" +
			"\nusing UniEasy;" +
			"\nusing System;" +
			"\nusing UniRx;" +
			"\n" +
			"\npublic class NewSystemBehaviour : SystemBehaviour" +
			"\n{" +
			"\n\tpublic override void Setup ()" +
			"\n\t{" +
			"\n\t\tbase.Setup ();" +
			"\n\t}" +
			"\n" +
			"\n\tvoid Start ()" +
			"\n\t{" +
			"\n\t\t" +
			"\n\t}" +
			"\n}";
		}
	}
}

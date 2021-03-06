﻿using UnityEditor;

namespace UniEasy.Edit
{
	public class ComponentBehaviourInstaller : ScriptAssetInstaller
	{
		[MenuItem ("Assets/Create/UniEasy/ComponentBehaviour Installer", false, 21)]
		static public void CreateComponentBehaviour ()
		{
			new ComponentBehaviourInstaller ().Create ();
		}

		public override string GetName ()
		{
			return "NewComponentBehaviour.cs";
		}

		public override string GetContents ()
		{
			return "using UnityEngine;" +
			"\nusing UniEasy.ECS;" +
			"\nusing UniEasy;" +
			"\nusing System;" +
			"\nusing UniRx;" +
			"\n" +
			"\npublic class NewComponentBehaviour : ComponentBehaviour" +
			"\n{" +
			"\n}";
		}
	}
}

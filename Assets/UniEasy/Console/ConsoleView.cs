﻿using UnityEngine.UI;
using UnityEngine;
using UniEasy.ECS;

namespace UniEasy.Console
{
	public class ConsoleView : ComponentBehaviour
	{
		public RectTransform panel;
		public InputField inputField;
		public Text outputText;
		public Scrollbar scrollbar;
		public ScrollRect outputArea;
	}
}

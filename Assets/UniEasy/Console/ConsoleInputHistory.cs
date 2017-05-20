using System.Collections.Generic;
using UnityEngine;
using System;

namespace UniEasy.Console
{
	public class ConsoleInputHistory
	{
		private int maxCapacity;
		private int currentInput;
		private bool isNavigating;
		private List<string> inputHistory;

		public ConsoleInputHistory (int maxCapacity)
		{
			inputHistory = new List<string> (maxCapacity);
			this.maxCapacity = maxCapacity;
		}

		public string Navigate (bool up)
		{
			bool down = !up;

			if (!isNavigating)
				isNavigating = (up && inputHistory.Count > 0) || (down && currentInput > 0);
			else if (up)
				currentInput++;
			if (down)
				currentInput--;

			currentInput = Mathf.Clamp (currentInput, 0, inputHistory.Count - 1);

			if (isNavigating)
				return inputHistory [currentInput];
			else
				return "";
		}

		public void AddNewInputEntry (string input)
		{
			isNavigating = false;

			if (inputHistory.Count > 0 && input.Equals (inputHistory [0], StringComparison.OrdinalIgnoreCase))
				return;

			if (inputHistory.Count == maxCapacity)
				inputHistory.RemoveAt (maxCapacity - 1);

			inputHistory.Insert (0, input);

			if (currentInput == maxCapacity - 1)
				currentInput = 0;
			else
				currentInput = Mathf.Clamp (++currentInput, 0, inputHistory.Count - 1);
			
			if (!input.Equals (inputHistory [currentInput], StringComparison.OrdinalIgnoreCase))
				currentInput = 0;
		}

		public void Clear ()
		{
			inputHistory.Clear ();
			currentInput = 0;
			isNavigating = false;
		}
	}
}

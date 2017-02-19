using UnityEngine;
using System.Collections.Generic;

namespace UniEasy
{
	public class EasyCsv
	{
		private readonly string[] RowSymbol = { "\n" };
		private readonly string[] ColumnSymbol = { ";" };
		private string[][] values;
		private Dictionary<string, Coordinate> target;
		private Dictionary<string, List<string>> RowDictionary;
		private Dictionary<string, List<string>> ColumnDictionary;

		protected struct Coordinate
		{
			public int row;
			public int column;
		}

		public int RowCount {
			get {
				if (values == null || values.Length <= 0) {
					return 0;
				}
				return values.Length - 1;
			}
		}

		public int ColumnCount {
			get {
				if (values == null || values.Length <= 0) {
					return 0;
				}
				return values [0].Length;
			}
		}

		public EasyCsv (TextAsset textAsset)
		{
			string[] rows = textAsset.text.Split (RowSymbol, System.StringSplitOptions.None);
			values = new string[rows.Length][];
			for (int i = 0; i < rows.Length; i++) {
				values [i] = rows [i].Split (ColumnSymbol, System.StringSplitOptions.None);
				int count = values [i].Length;
				values [i] [count - 1] = values [i] [count - 1].Replace ("\r", "");
			}

			target = new Dictionary<string, Coordinate> ();
			for (int i = 0; i < RowCount; i++) {
				for (int j = 0; j < ColumnCount; j++) {
					string content = GetValue (i, j);
					if (!target.ContainsKey (content)) {
						target.Add (content, new Coordinate () { row = i, column = j });
					} else {
						#if UNITY_EDITOR
						Debug.LogWarning ("Data's name [" + content + "] conflict! please make sure you will not use this data's name join in search.");
						#endif
					}
				}
			}

			RowDictionary = new Dictionary<string, List<string>> ();
			ColumnDictionary = new Dictionary<string, List<string>> ();
		}

		public string GetValue (int row, int column)
		{
			if (row < 0 || column < 0 || values == null)
				return default (string);
			if (values.Length <= 0 || row >= values.Length)
				return default (string);
			if (values [0] == null || column >= values [0].Length)
				return default (string);

			return values [row] [column];
		}

		public List<string> ToRowList (string name)
		{
			if (RowDictionary.ContainsKey (name))
				return RowDictionary [name];
			List<string> list = new List<string> ();
			if (target.ContainsKey (name)) {
				list.AddRange (values [target [name].row]);
				RowDictionary.Add (name, list);
			}
			return list;
		}

		public List<string> ToColumnList (string name)
		{
			if (ColumnDictionary.ContainsKey (name))
				return ColumnDictionary [name];
			List<string> list = new List<string> ();
			if (target.ContainsKey (name)) {
				for (int i = 0; i < RowCount; i++) {
					list.Add (GetValue (i, target [name].column));
				}
				ColumnDictionary.Add (name, list);
			}
			return list;
		}

		public string GetValue (string name, string rowName, string columnName)
		{
			List<string> rowList = this.ToRowList (name);
			List<string> columnList = this.ToColumnList (name);
			if (columnList.Contains (rowName) && rowList.Contains (columnName)) {
				int row = columnList.FindIndex (value => value == rowName);
				int column = rowList.FindIndex (value => value == columnName);
				return GetValue (row, column);
			}
			return default (string);
		}
	}
}

using UnityEngine;
using System;

namespace UniEasy
{
	[Serializable]
	public class EasyBlock : EasyAsset<BlockObject>
	{
		protected override string GetKey (BlockObject item)
		{
			return item.name;
		}
	}
}

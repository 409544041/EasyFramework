using UnityEngine;

[System.Serializable]
public class EasyBlock : EasyAsset<BlockObject>
{
	protected override string GetKey (BlockObject item)
	{
		return item.name;
	}
}

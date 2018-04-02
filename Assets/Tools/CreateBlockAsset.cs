using UnityEditor;
using UnityEngine;

public class CreateBlockAsset {
	[MenuItem("Assets/Create/BlockData")]
	public static void CreateMyAsset()
	{
		BlockData asset = ScriptableObject.CreateInstance<BlockData>();

		AssetDatabase.CreateAsset(asset, "Assets/Block.asset");
		AssetDatabase.SaveAssets();

		EditorUtility.FocusProjectWindow();

		Selection.activeObject = asset;
	}
}

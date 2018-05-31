using UnityEngine;
using System.Collections;
using UnityEditor;

public class MakeScriptableObject {
	[MenuItem("Assets/Create/FightGame Scriptable")]
	public static void CreateMyAsset()
	{
		FightScriptable asset = ScriptableObject.CreateInstance<FightScriptable>();

		AssetDatabase.CreateAsset(asset, "Assets/DummyFightScriptable.asset");
		AssetDatabase.SaveAssets();

		EditorUtility.FocusProjectWindow();

		Selection.activeObject = asset;
	}
}

[CreateAssetMenu(fileName = "Data", menuName = "Inventory/List", order = 1)]
public class FightScriptable : ScriptableObject 
{
	public string objectName = "DummyFightScriptable";
	public Link[] linkList;
}


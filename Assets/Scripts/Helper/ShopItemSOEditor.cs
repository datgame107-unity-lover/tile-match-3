#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShopItemSO))]
public class ShopItemSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ShopItemSO item = (ShopItemSO)target;

        item.itemType = (ShopItemType)EditorGUILayout.EnumPopup("Item Type", item.itemType);

        EditorGUILayout.Space();

        if (item.itemType == ShopItemType.Bundle)
        {
            item.bundleName = EditorGUILayout.TextField("Bundle Name", item.bundleName);
            item.bundlePrice = EditorGUILayout.IntField("Bundle Price", item.bundlePrice);
            item.isPurchased = EditorGUILayout.Toggle("Is Purchased",item.isPurchased);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("abilities"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currencies"), true);
        }
        else if (item.itemType == ShopItemType.Ability)
        {
            item.abilityName = EditorGUILayout.TextField("Ability Name", item.abilityName);
            item.abilityIcon = (Sprite)EditorGUILayout.ObjectField("Ability Icon", item.abilityIcon, typeof(Sprite), false);
            item.description = EditorGUILayout.TextField("Description", item.description);
            item.abilityPrice = EditorGUILayout.IntField("Price", item.abilityPrice);
        }

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
            EditorUtility.SetDirty(item);
    }
}
#endif

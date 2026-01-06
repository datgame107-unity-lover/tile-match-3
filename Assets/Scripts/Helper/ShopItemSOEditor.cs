#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShopItemSO))]
public class ShopItemSOEditor : Editor
{
    // Properties for serialization (Supports Undo/Redo)
    SerializedProperty itemTypeProp;
    SerializedProperty buyWithProp;

    // Bundle specific
    SerializedProperty bundleId;
    SerializedProperty bundleNameProp;
    SerializedProperty abilitiesProp;
    SerializedProperty currenciesProp;
    SerializedProperty isOneTimePurchaseProp;
    SerializedProperty currencyType;
    // Single Item specific (Ability/Currency)
    SerializedProperty itemNameProp;
    SerializedProperty iconProp;
    SerializedProperty descriptionProp;
    SerializedProperty quantityProp;

    private void OnEnable()
    {
        // Link properties to variables in ShopItemSO.cs
        itemTypeProp = serializedObject.FindProperty("itemType");
        buyWithProp = serializedObject.FindProperty("buyWith");

        bundleId = serializedObject.FindProperty("iapProductId");
        bundleNameProp = serializedObject.FindProperty("bundleName");
        abilitiesProp = serializedObject.FindProperty("abilities");
        currenciesProp = serializedObject.FindProperty("currencies");
        isOneTimePurchaseProp = serializedObject.FindProperty("isOneTimePurchase");

        itemNameProp = serializedObject.FindProperty("itemName");
        iconProp = serializedObject.FindProperty("icon");
        descriptionProp = serializedObject.FindProperty("description");
        quantityProp = serializedObject.FindProperty("quantity");
        currencyType = serializedObject.FindProperty("currencyType");
    }

    public override void OnInspectorGUI()
    {
        // Update the serialized object
        serializedObject.Update();

        // 1. Draw Item Type
        EditorGUILayout.PropertyField(itemTypeProp);
        ShopItemType currentType = (ShopItemType)itemTypeProp.enumValueIndex;

        EditorGUILayout.Space(10);

        // 2. Draw Cost (buyWith)
        // Since buyWith is a class (CurrencyData), PropertyField draws it correctly
        // Note: Even Bundles usually need a price. If you strictly want this only for Ability/Currency,
        // you can move this inside the 'else' block below.
        if (currentType == ShopItemType.Ability || currentType == ShopItemType.Currency)
        {
            EditorGUILayout.LabelField("Cost Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(buyWithProp);
            EditorGUILayout.Space(10);
        }
        else if (currentType == ShopItemType.Bundle)
        {
            // If bundles also have a price defined in 'buyWith', uncomment the line below:
            // EditorGUILayout.PropertyField(buyWithProp); 
        }

        // 3. Draw Specific Data
        if (currentType == ShopItemType.Bundle)
        {
            EditorGUILayout.LabelField("Bundle Configuration", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(bundleNameProp);
            EditorGUILayout.PropertyField(isOneTimePurchaseProp);
            EditorGUILayout.PropertyField(bundleId);
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(abilitiesProp);
            EditorGUILayout.PropertyField(currenciesProp);
        }
        else // Ability or Currency
        {
            EditorGUILayout.LabelField("Single Item Configuration", EditorStyles.boldLabel);

            // These use the shared fields from your new ShopItemSO structure
            EditorGUILayout.PropertyField(itemNameProp, new GUIContent("Name"));
            EditorGUILayout.PropertyField(iconProp, new GUIContent("Icon"));

            // Quantity is relevant for Currency (e.g., 500 Gold) or stackable items
            EditorGUILayout.PropertyField(quantityProp, new GUIContent("Quantity / Amount"));
            EditorGUILayout.PropertyField(currencyType, new GUIContent("CurrencyType"));

            if (currentType == ShopItemType.Ability)
            {
                EditorGUILayout.PropertyField(descriptionProp);
            }
        }

        // Apply properties to the actual object
        serializedObject.ApplyModifiedProperties();
    }
}

#endif
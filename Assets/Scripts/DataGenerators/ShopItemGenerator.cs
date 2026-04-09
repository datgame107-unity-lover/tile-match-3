using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ShopItemGenerator : EditorWindow
{
    private class ShopItemSetup
    {
        public string fileName;
        public string itemName;
        public ShopItemType itemType;

        // Dùng cho SingleCurrency
        public PriceData buyWith;
        public CurrencyType singleGrantType;
        public int singleGrantQty;

        // Dùng cho Bundle (IAP)
        public string iapProductId;
        public List<CurrencyData> bundleCurrencies = new List<CurrencyData>();
        public List<AbilityData> bundleAbilities = new List<AbilityData>();
    }

    [MenuItem("Tools/Generate Shop Items (2 Types)")]
    public static void GenerateItems()
    {
        string folderPath = "Assets/ScriptableObjects/Shop";

        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects"))
            AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects/Shop"))
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "Shop");

        List<ShopItemSetup> database = new List<ShopItemSetup>
        {
            // ==========================================
            // VẬT PHẨM SINGLE (MUA BẰNG TIỀN IN-GAME)
            // ==========================================
            new ShopItemSetup { fileName = "Shop_Heart_1", itemName = "+1 Heart", itemType = ShopItemType.Currency,
                buyWith = new PriceData { currencyType = CurrencyType.Flower, price = 50 },
                singleGrantType = CurrencyType.Heart, singleGrantQty = 1 },

            new ShopItemSetup { fileName = "Shop_Heart_5", itemName = "Full Hearts", itemType = ShopItemType.Currency,
                buyWith = new PriceData { currencyType = CurrencyType.Diamond, price = 20 },
                singleGrantType = CurrencyType.Heart, singleGrantQty = 5 },

            new ShopItemSetup { fileName = "Shop_Hint_1", itemName = "Hint x1", itemType = ShopItemType.Currency,
                buyWith = new PriceData { currencyType = CurrencyType.Diamond, price = 20 },
                singleGrantType = CurrencyType.Hint, singleGrantQty = 1 },

            new ShopItemSetup { fileName = "Shop_Hint_3", itemName = "Hint x3", itemType = ShopItemType.Currency,
                buyWith = new PriceData { currencyType = CurrencyType.Diamond, price = 50 },
                singleGrantType = CurrencyType.Hint, singleGrantQty = 3 },

            new ShopItemSetup { fileName = "Shop_Shuffle_1", itemName = "Shuffle x1", itemType = ShopItemType.Currency,
                buyWith = new PriceData { currencyType = CurrencyType.Diamond, price = 20 },
                singleGrantType = CurrencyType.Shuffle, singleGrantQty = 1 },

            new ShopItemSetup { fileName = "Shop_Undo_1", itemName = "Undo x1", itemType = ShopItemType.Currency,
                buyWith = new PriceData { currencyType = CurrencyType.Diamond, price = 15 },
                singleGrantType = CurrencyType.Undo, singleGrantQty = 1 },

            new ShopItemSetup { fileName = "Shop_PowerUp_1", itemName = "PowerUp x1", itemType = ShopItemType.Currency,
                buyWith = new PriceData { currencyType = CurrencyType.Diamond, price = 30 },
                singleGrantType = CurrencyType.PowerUp, singleGrantQty = 1 },

            // ==========================================
            // BUNDLE & IAP (MUA BẰNG TIỀN THẬT)
            // ==========================================
            
            // Gói nạp Kim Cương (Được tính là Bundle chứa 1 loại tiền)
            new ShopItemSetup { fileName = "IAP_Diamond_Small", itemName = "Handful of Diamonds", itemType = ShopItemType.Bundle,
                iapProductId = "iap_diamond_1",
                bundleCurrencies = new List<CurrencyData> {
                    new CurrencyData { currencyType = CurrencyType.Diamond, quantity = 100 }
                }
            },

            new ShopItemSetup { fileName = "IAP_Diamond_Large", itemName = "Chest of Diamonds", itemType = ShopItemType.Bundle,
                iapProductId = "iap_diamond_2",
                bundleCurrencies = new List<CurrencyData> {
                    new CurrencyData { currencyType = CurrencyType.Diamond, quantity = 500 }
                }
            },

            // Các gói Combo hỗn hợp
            new ShopItemSetup { fileName = "Bundle_Starter", itemName = "Starter Bundle", itemType = ShopItemType.Bundle,
                iapProductId = "bundle_starter",
                bundleCurrencies = new List<CurrencyData> {
                    new CurrencyData { currencyType = CurrencyType.Diamond, quantity = 200 },
                    new CurrencyData { currencyType = CurrencyType.Heart, quantity = 5 }
                },
                bundleAbilities = new List<AbilityData> {
                    new AbilityData { abilityType = CurrencyType.Hint, quantity = 2 },
                    new AbilityData { abilityType = CurrencyType.Shuffle, quantity = 2 }
                }
            },

            new ShopItemSetup { fileName = "Bundle_Pro", itemName = "Pro Bundle", itemType = ShopItemType.Bundle,
                iapProductId = "bundle_pro",
                bundleCurrencies = new List<CurrencyData> {
                    new CurrencyData { currencyType = CurrencyType.Heart, quantity = 5 }
                },
                bundleAbilities = new List<AbilityData> {
                    new AbilityData { abilityType = CurrencyType.Hint, quantity = 10 },
                    new AbilityData { abilityType = CurrencyType.Undo, quantity = 10 },
                    new AbilityData { abilityType = CurrencyType.PowerUp, quantity = 5 }
                }
            },

            new ShopItemSetup { fileName = "Bundle_Weekend", itemName = "Weekend Sale", itemType = ShopItemType.Bundle,
                iapProductId = "bundle_weekend",
                bundleCurrencies = new List<CurrencyData> {
                    new CurrencyData { currencyType = CurrencyType.Diamond, quantity = 1000 }
                },
                bundleAbilities = new List<AbilityData> {
                    new AbilityData { abilityType = CurrencyType.PowerUp, quantity = 10 }
                }
            }
        };

        foreach (var data in database)
        {
            ShopItemSO newItem = ScriptableObject.CreateInstance<ShopItemSO>();

            newItem.itemName = data.itemName;
            newItem.itemType = data.itemType;

            // Phân nhánh lưu dữ liệu dựa vào Type
            if (data.itemType == ShopItemType.Bundle)
            {
                newItem.iapProductId = data.iapProductId;
                newItem.currencies = data.bundleCurrencies;
                newItem.abilities = data.bundleAbilities;
            }
            else if (data.itemType == ShopItemType.Currency)
            {
                newItem.buyWith = data.buyWith;
                newItem.currencyType = data.singleGrantType;
                newItem.quantity = data.singleGrantQty;
            }

            string assetPath = $"{folderPath}/{data.fileName}.asset";
            AssetDatabase.CreateAsset(newItem, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"<color=cyan>Đã tạo thành công 12 Shop Item mẫu (SingleCurrency & Bundle IAP) tại: {folderPath}</color>");
    }
}
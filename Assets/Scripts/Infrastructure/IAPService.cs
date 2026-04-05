// Scripts/Infrastructure/IAPService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPService
{
    public static bool IsInitialized { get; private set; }

    private StoreController storeController;
    private ShopService shopService;

    public async Task Initialize(ShopService shopService)
    {
        this.shopService = shopService;

        try
        {
            var options = new InitializationOptions()
                .SetEnvironmentName("production");
            await UnityServices.InitializeAsync(options);

            storeController = UnityIAPServices.StoreController();

            storeController.OnProductsFetched += OnProductsFetched;
            storeController.OnProductsFetchFailed += OnProductsFetchFailed;
            storeController.OnPurchasesFetched += OnPurchasesFetched;
            storeController.OnPurchasePending += OnPurchasePending;
            storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;
            storeController.OnPurchaseFailed += OnPurchaseFailed;
            storeController.OnPurchaseDeferred += OnPurchaseDeferred;
            storeController.OnStoreDisconnected += OnStoreDisconnected;

            await storeController.Connect();
            storeController.FetchProducts(BuildProducts());
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[IAPService] Init failed: {e}");
        }
    }

    public void BuyProduct(string productId)
    {
        if (!IsInitialized)
        {
            Debug.LogWarning("[IAPService] Not initialized yet");
            return;
        }
        storeController.PurchaseProduct(productId);
    }

    // ── Callbacks ─────────────────────────────────────
    private void OnProductsFetched(List<Product> products)
    {
        storeController.FetchPurchases();
    }

    private void OnProductsFetchFailed(ProductFetchFailed obj)
    {
        Debug.LogError($"[IAPService] Products fetch failed: {obj}");
    }

    private void OnPurchasesFetched(Orders orders)
    {
        IsInitialized = true;
    }

    private void OnPurchasePending(PendingOrder order)
    {
        storeController.ConfirmPurchase(order);
    }

    private void OnPurchaseConfirmed(Order order)
    {
        if (order?.Info?.PurchasedProductInfo == null ||
            order.Info.PurchasedProductInfo.Count == 0)
        {
            Debug.LogError("[IAPService] Invalid order info");
            return;
        }

        string productId = order.Info.PurchasedProductInfo[0].productId;
        shopService.GrantIAP(productId);

        EventBus<TransactionCompleteEvent>.Publish(
            new TransactionCompleteEvent());
    }

    private void OnPurchaseFailed(FailedOrder obj)
    {
        Debug.LogError($"[IAPService] Purchase failed: {obj}");
    }

    private void OnPurchaseDeferred(DeferredOrder obj)
    {
        Debug.Log($"[IAPService] Purchase deferred: {obj?.Info}");
    }

    private void OnStoreDisconnected(StoreConnectionFailureDescription obj)
    {
        Debug.LogError($"[IAPService] Disconnected: {obj}");
    }

    // ── Product definitions ───────────────────────────
    private List<ProductDefinition> BuildProducts() => new()
    {
        new ProductDefinition("FIRSTPURCHASE", ProductType.Consumable),
        new ProductDefinition("bundle1",       ProductType.Consumable),
        new ProductDefinition("bundle2",       ProductType.Consumable),
        new ProductDefinition("bundle3",       ProductType.Consumable),
        new ProductDefinition("bundle4",       ProductType.Consumable),
    };
}
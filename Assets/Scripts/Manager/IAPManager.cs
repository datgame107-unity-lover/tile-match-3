using UnityEngine;
using UnityEngine.Purchasing;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Core.Environments;

public enum IAPProductKey
{
    FIRSTPURCHASE,
    bundle1,
    bundle2,
    bundle3,
    bundle4,
}

public class IAPManager : MonoBehaviour
{
    public static IAPManager Instance;
    public static bool IsInitialized { get; private set; } = false;
    private static StoreController storeController;


    private async void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        await (InitIAP());
    }



    private async Task InitIAP()
    {
        var option = new InitializationOptions().SetEnvironmentName("production");
        await UnityServices.InitializeAsync(option);

        storeController = UnityIAPServices.StoreController();
        storeController.OnStoreDisconnected += StoreController_OnStoreDisconnected;
        storeController.OnProductsFetched += StoreController_OnProductsFetched;
        storeController.OnProductsFetchFailed += StoreController_OnProductsFetchFailed;
        storeController.OnPurchasesFetched += StoreController_OnPurchasesFetched;
        storeController.OnPurchasesFetchFailed += StoreController_OnPurchasesFetchFailed;
        storeController.OnPurchasePending += StoreController_OnPurchasePending;
        storeController.OnPurchaseConfirmed += StoreController_OnPurchaseConfirmed;
        storeController.OnPurchaseFailed += StoreController_OnPurchaseFailed;
        storeController.OnPurchaseDeferred += StoreController_OnPurchaseDeferred;

        await storeController.Connect();
        var initialProductToFetch = BuildProductDefinition();
        storeController.FetchProducts(initialProductToFetch);
    }

    private void StoreController_OnPurchaseDeferred(DeferredOrder obj)
    {
        Debug.Log($"Purchase Deffered for product: {obj?.Info}");
    }

    private void StoreController_OnPurchaseFailed(FailedOrder obj)
    {
        Debug.Log(":((");
    }

    private void StoreController_OnPurchaseConfirmed(Order obj)
    {
        Debug.Log("Here");

        if (obj == null)
        {
            Debug.LogError("obj == null");
            return;
        }

        if (obj.Info == null)
        {
            Debug.LogError("obj.Info == null");
            return;
        }

        if (obj.Info.PurchasedProductInfo == null)
        {
            Debug.LogError("PurchasedProductInfo == null");
            return;
        }

        Debug.Log("PurchasedProductInfo.Count = " + obj.Info.PurchasedProductInfo.Count);

        if (obj.Info.PurchasedProductInfo.Count > 0)
        {
            string productId = obj.Info.PurchasedProductInfo[0].productId;
            Debug.Log("ProductId = " + productId);
            ShopManager.Instance.GrantIAP(productId);
            EventManager.OnTransactionComplete?.Invoke();
        }
    }

    private void StoreController_OnPurchasePending(PendingOrder obj)
    {
        Debug.Log($"Pender order:{obj}");
        storeController.ConfirmPurchase(obj);
    }

    private void StoreController_OnPurchasesFetchFailed(PurchasesFetchFailureDescription obj)
    {
        Debug.Log($"Purchases fetch Failed : {obj}");

    }

    private void StoreController_OnPurchasesFetched(Orders obj)
    {
        IsInitialized = true;
    }

    private void StoreController_OnProductsFetchFailed(ProductFetchFailed obj)
    {
        Debug.Log($"Product fetch Failed : {obj}");
    }

    private void StoreController_OnProductsFetched(List<Product> obj)
    {
        storeController.FetchPurchases();

    }

    private void StoreController_OnStoreDisconnected(StoreConnectionFailureDescription obj)
    {
        Debug.Log($"Initialization/Connection Failed : {obj}");

    }

    private List<ProductDefinition> BuildProductDefinition()
    {
        var initialProductToFetch = new List<ProductDefinition>();
        initialProductToFetch.Add(new ProductDefinition("FIRSTPURCHASE", ProductType.Consumable));
        initialProductToFetch.Add(new ProductDefinition("bundle1", ProductType.Consumable));
        initialProductToFetch.Add(new ProductDefinition("bundle2", ProductType.Consumable));
        initialProductToFetch.Add(new ProductDefinition("bundle3", ProductType.Consumable));
        return initialProductToFetch;
    }
    public void BuyProduct(string productName)
    {
        if(!IsInitialized)
        {
            return;
        }
            storeController.PurchaseProduct(productName);
    }
}
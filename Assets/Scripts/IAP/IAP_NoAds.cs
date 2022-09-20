using UnityEngine;
using UnityEngine.Purchasing;

public class IAP_NoAds : MonoBehaviour
{
    private void Start()
    {
        if (DataManager.Instance.noAds)
        {
            gameObject.SetActive(false);
        }
    }
    public void OnPurchaseComplete(Product product)
    {
        DataManager.Instance.noAds = true;
        DataManager.Instance.Save();
    }
    public void OnPurchaseFailed(Product product, PurchaseFailureReason purchaseFailureReason)
    {
    }
}

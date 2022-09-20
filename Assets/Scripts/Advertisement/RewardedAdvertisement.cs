using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class RewardedAdvertisement : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";
    [SerializeField] Button sortBtn;
    [SerializeField] Button spawnValueBtn;
    [SerializeField] Button _doubleBtn;
    [SerializeField] Button multiplierBtn;
    [SerializeField] GameManager gameManager;
    public AdType AdType;
    string _adUnitId = null; // This will remain null for unsupported platforms

    void Awake()
    {
        // Get the Ad Unit ID for the current platform:
#if UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#endif

        //Disable the button until the ad is ready to show:
        sortBtn.interactable = false;
        spawnValueBtn.interactable = false;
        _doubleBtn.interactable = false;
        multiplierBtn.interactable = false;
    }
    private void Start()
    {
        sortBtn.onClick.AddListener(ShowAd);
        spawnValueBtn.onClick.AddListener(ShowAd);
        _doubleBtn.onClick.AddListener(ShowAd);
        multiplierBtn.onClick.AddListener(ShowAd);
    }
    // Load content to the Ad Unit:
    public void LoadAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }

    // If the ad successfully loads, add a listener to the button and enable it:
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);

        if (adUnitId.Equals(_adUnitId))
        {
            // Enable the button for users to click:
            if (DataManager.Instance.currentLevel >= 3)
            {
                sortBtn.interactable = true;
            }
            if (DataManager.Instance.currentLevel >= 5)
            {
                spawnValueBtn.interactable = true;
            }
            if (DataManager.Instance.currentLevel >= 7)
            {
                _doubleBtn.interactable = true;
            }
            if (DataManager.Instance.currentLevel >= 9)
            {
                multiplierBtn.interactable = true;
            }
        }
    }

    // Implement a method to execute when the user clicks the button:
    public void ShowAd()
    {
        // Disable the button:
        sortBtn.interactable = false;
        spawnValueBtn.interactable = false;
        _doubleBtn.interactable = false;
        multiplierBtn.interactable = false;

        // Then show the ad:
        Advertisement.Show(_adUnitId, this);
    }

    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");
            // Grant a reward.
            if (AdType == AdType.Sort)
            {
                gameManager.SortBlocks();
            }
            else if (AdType == AdType.SpawnValue)
            {
                gameManager.IncreaseMinMaxSpawnValue();
            }
            else if (AdType == AdType.Double)
            {
                gameManager.DoubleAllBlocks();
            }
            else if (AdType == AdType.Multiplier)
            {
                gameManager.IncreaseScoreMultiplier();
            }

            // Load another ad:
            Advertisement.Load(_adUnitId, this);
        }
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }

    void OnDestroy()
    {
        // Clean up the button listeners:
        sortBtn.onClick.RemoveAllListeners();
        _doubleBtn.onClick.RemoveAllListeners();
        multiplierBtn.onClick.RemoveAllListeners();
        spawnValueBtn.onClick.RemoveAllListeners();
    }
}

public enum AdType
{
    Sort,
    Double,
    SpawnValue,
    Multiplier
}
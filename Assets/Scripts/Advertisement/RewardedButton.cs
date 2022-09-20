using UnityEngine;
using UnityEngine.UI;

public class RewardedButton : MonoBehaviour
{
    [SerializeField] RewardedAdvertisement RewardedAd;
    [SerializeField] AdType _AdType;
    private Button _button => GetComponent<Button>();

    private void Start()
    {
        _button.onClick.AddListener(DefineAdType);
    }

    private void DefineAdType()
    {
        RewardedAd.AdType = _AdType;
        FindObjectOfType<AudioManager>().Play("Click");
    }
}

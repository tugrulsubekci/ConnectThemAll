using Google.Play.Review;
using System.Collections;
using UnityEngine;

public class IAR_Manager : MonoBehaviour
{
    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;
    private string url = "https://play.google.com/store/apps/details?id=com.TworuleGames.ConnectThemAll";

    public void RateUsButton()
    {
        StartCoroutine(nameof(RequestAndLaunchReview));
    }
    private IEnumerator RequestAndLaunchReview()
    {
        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Application.OpenURL(url);
            yield break;
        }
        _playReviewInfo = requestFlowOperation.GetResult();

        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Application.OpenURL(url);
            yield break;
        }
    }
}

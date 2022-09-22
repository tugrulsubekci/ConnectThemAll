using Google.Play.AppUpdate;
using Google.Play.Common;
using System.Collections;
using UnityEngine;

public class IAU_Manager : MonoBehaviour
{
    AppUpdateManager appUpdateManager;
    private void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        appUpdateManager = new AppUpdateManager();
        StartCoroutine(nameof(CheckForUpdate));
#endif
    }
    private IEnumerator CheckForUpdate()
    {
        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
          appUpdateManager.GetAppUpdateInfo();

        // Wait until the asynchronous operation completes.
        yield return appUpdateInfoOperation;

        if (appUpdateInfoOperation.IsSuccessful)
        {
            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();
            // Check AppUpdateInfo's UpdateAvailability, UpdatePriority,
            // IsUpdateTypeAllowed(), etc. and decide whether to ask the user
            // to start an in-app update.

            if(appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            {
                // Creates an AppUpdateOptions for an immediate flow that allows
                // asset pack deletion.
                var appUpdateOptions =
                  AppUpdateOptions.ImmediateAppUpdateOptions(allowAssetPackDeletion: true);

                StartCoroutine(StartImmediateUpdate(appUpdateInfoResult, appUpdateOptions));
            }
        }
        else
        {
            // Log appUpdateInfoOperation.Error.
        }
    }

    private IEnumerator StartImmediateUpdate(AppUpdateInfo appUpdateInfoResult, AppUpdateOptions appUpdateOptions)
    {
        // Creates an AppUpdateRequest that can be used to monitor the
        // requested in-app update flow.
        var startUpdateRequest = appUpdateManager.StartUpdate(
          // The result returned by PlayAsyncOperation.GetResult().
          appUpdateInfoResult,
          // The AppUpdateOptions created defining the requested in-app update
          // and its parameters.
          appUpdateOptions);
        yield return startUpdateRequest;

        // If the update completes successfully, then the app restarts and this line
        // is never reached. If this line is reached, then handle the failure (for
        // example, by logging result.Error or by displaying a message to the user).
    }
}

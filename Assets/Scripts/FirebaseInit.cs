using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using System;
using UnityEngine;

public class FirebaseInit : MonoBehaviour
{
    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    protected bool firebaseInitialized = false;
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError(
                  "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase()
    {
        Debug.Log("Enabling data collection.");
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        Debug.Log("Initiate on-device conversion measurement.");
        FirebaseAnalytics.InitiateOnDeviceConversionMeasurementWithEmailAddress("test@testemail.com");

        Debug.Log("Set user properties.");
        // Set the user's sign up method.
        FirebaseAnalytics.SetUserProperty(
          FirebaseAnalytics.UserPropertySignUpMethod,
          "Google");
        // Set the user ID.
        FirebaseAnalytics.SetUserId("uber_user_510");
        // Set default session duration values.
        FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 300, 0));
        firebaseInitialized = true;
    }
}
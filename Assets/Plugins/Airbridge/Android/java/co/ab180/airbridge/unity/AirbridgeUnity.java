package co.ab180.airbridge.unity;

import android.content.Intent;
import android.net.Uri;
import android.util.Log;

import com.unity3d.player.UnityPlayer;

import org.jetbrains.annotations.NotNull;
import org.json.JSONObject;

import co.ab180.airbridge.Airbridge;
import co.ab180.airbridge.AirbridgeCallback;
import co.ab180.airbridge.event.Event;

public class AirbridgeUnity {

    private static String startDeeplink;
    private static String deeplinkCallbackObjectName;

    public static void startTracking() {
        Airbridge.startTracking();
    }

    public static void setUserId(String id) {
        Airbridge.getCurrentUser().setId(id);
    }

    public static void setUserEmail(String email) {
        Airbridge.getCurrentUser().setEmail(email);
    }

    public static void setUserPhone(String phone) {
        Airbridge.getCurrentUser().setPhone(phone);
    }

    public static void setUserAlias(String key, String value) {
        Airbridge.getCurrentUser().setAlias(key, value);
    }

    public static void setUserAttribute(String key, int value) {
        Airbridge.getCurrentUser().setAttribute(key, value);
    }

    public static void setUserAttribute(String key, long value) {
        Airbridge.getCurrentUser().setAttribute(key, value);
    }

    public static void setUserAttribute(String key, float value) {
        Airbridge.getCurrentUser().setAttribute(key, value);
    }

    public static void setUserAttribute(String key, boolean value) {
        Airbridge.getCurrentUser().setAttribute(key, value);
    }

    public static void setUserAttribute(String key, String value) {
        Airbridge.getCurrentUser().setAttribute(key, value);
    }

    public static void clearUserAttributes() {
        Airbridge.getCurrentUser().clearAttributes();
    }

    public static void expireUser() {
        Airbridge.expireUser();
    }

    public static void clickTrackingLink(String trackingLink, String deeplink, String fallback) {
        Airbridge.click(trackingLink, deeplink, fallback, null);
    }

    public static void impressionTrackingLink(String trackingLink) {
        Airbridge.impression(trackingLink);
    }

    public static void trackEvent(String jsonString) {
        try {
            JSONObject object = new JSONObject(jsonString);
            Event event = AirbridgeEventParser.from(object);
            Airbridge.trackEvent(event);
        } catch (Exception e) {
            Log.e("AirbridgeUnity", "Error occurs while parsing data json string", e);
        }
    }

    public static void setDeeplinkCallback(String objectName) {
        deeplinkCallbackObjectName = objectName;
        if (startDeeplink != null && !startDeeplink.isEmpty()) {
            UnityPlayer.UnitySendMessage(deeplinkCallbackObjectName, "OnTrackingLinkResponse", startDeeplink);
        }
    }

    @SuppressWarnings("WeakerAccess")
    public static void processDeeplinkData(Intent intent) {
        Airbridge.getDeeplink(intent, new AirbridgeCallback.SimpleCallback<Uri>() {

            @Override
            public void onSuccess(Uri uri) {
                if (deeplinkCallbackObjectName != null && !deeplinkCallbackObjectName.isEmpty()) {
                    UnityPlayer.UnitySendMessage(deeplinkCallbackObjectName, "OnTrackingLinkResponse", uri.toString());
                    startDeeplink = null;
                } else {
                    if (startDeeplink == null) {
                        startDeeplink = uri.toString();
                    } else {
                        startDeeplink = null;
                    }
                }
            }

            @Override
            public void onFailure(@NotNull Throwable throwable) {

            }
        });
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShareButton : MonoBehaviour
{
    public string shareMessage;
    public string gameResults;

    public void Share()
    {
        string message = shareMessage + "\n\n" + gameResults;

        StartCoroutine(ShareText(message));
    }

    IEnumerator ShareText(string message)
    {
        yield return new WaitForEndOfFrame();

        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
        //intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), message);
        intentObject.Call<AndroidJavaObject>("setType", "text/plain");
        AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Compartilhar via");
        currentActivity.Call("startActivity", chooser);
    }
}

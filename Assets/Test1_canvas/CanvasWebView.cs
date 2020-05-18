using System.Collections;
using Extensions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CanvasWebView : MonoBehaviour
{
    public string url;
    public WebViewObject webViewObject;
    public RectTransform rectTransform;

    private IEnumerator Start()
    {
        webViewObject.Init(
            msg =>
            {
                Debug.Log($"CallFromJS[{msg}]");
            },
            err: (msg) =>
            {
                Debug.Log($"CallOnError[{msg}]");
            },
            started: (msg) =>
            {
                Debug.Log($"CallOnStarted[{msg}]");
            },
            ld: (msg) =>
            {
                Debug.Log($"CallOnLoaded[{msg}]");
            },
            //ua: "custom user agent string",
            enableWKWebView: true);

        webViewObject.SetRectTransformMargin(rectTransform);

        var src = System.IO.Path.Combine(Application.streamingAssetsPath, url);
        var dst = System.IO.Path.Combine(Application.persistentDataPath, url);
        byte[] result;

        if (src.Contains("://")) {  // for Android
            var www = UnityWebRequest.Get(src);
            yield return www.SendWebRequest();
            result = www.downloadHandler.data;
        } else {
            result = System.IO.File.ReadAllBytes(src);
        }

        System.IO.File.WriteAllBytes(dst, result);
        webViewObject.LoadURL("file://" + dst.Replace(" ", "%20"));
        webViewObject.SetVisibility(true);
    }

    public void HideShowWebView()
    {
        var active = webViewObject.gameObject.activeSelf;
        webViewObject.gameObject.SetActive(!active);
        webViewObject.SetVisibility(!active);
    }

    public void GoBack()
    {
        webViewObject.GoBack();
    }
}

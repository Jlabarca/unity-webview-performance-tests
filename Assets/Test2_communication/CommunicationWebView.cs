using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CommunicationWebView : MonoBehaviour
{
    public string url;
    public WebViewObject webViewObject;
    public RectTransform rectTransform;
    public Text message;

    private IEnumerator Start()
    {
        webViewObject.Init(
            msg =>
            {
                Debug.Log($"CallFromJS[{msg}]");
                message.text = msg;
            },
            err: (msg) =>
            {
                Debug.Log($"CallOnError[{msg}]");
                message.text = $"ERROR: {msg}";
            },
            started: (msg) =>
            {
                Debug.Log($"CallOnStarted[{msg}]");
            },
            ld: (msg) =>
            {
                Debug.Log(string.Format("CallOnLoaded[{0}]", msg));
#if UNITY_EDITOR_OSX || !UNITY_ANDROID
                // NOTE: depending on the situation, you might prefer
                // the 'iframe' approach.
                // cf. https://github.com/gree/unity-webview/issues/189
#if true
                webViewObject.EvaluateJS(@"
                  if (window && window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.unityControl) {
                    window.Unity = {
                      call: function(msg) {
                        window.webkit.messageHandlers.unityControl.postMessage(msg);
                      }
                    }
                  } else {
                    window.Unity = {
                      call: function(msg) {
                        window.location = 'unity:' + msg;
                      }
                    }
                  }
                ");
#else
                webViewObject.EvaluateJS(@"
                  if (window && window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.unityControl) {
                    window.Unity = {
                      call: function(msg) {
                        window.webkit.messageHandlers.unityControl.postMessage(msg);
                      }
                    }
                  } else {
                    window.Unity = {
                      call: function(msg) {
                        var iframe = document.createElement('IFRAME');
                        iframe.setAttribute('src', 'unity:' + msg);
                        document.documentElement.appendChild(iframe);
                        iframe.parentNode.removeChild(iframe);
                        iframe = null;
                      }
                    }
                  }
                ");
#endif
#endif
                webViewObject.EvaluateJS(@"Unity.call('ua=' + navigator.userAgent)");
            },
            enableWKWebView: true);
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        webViewObject.bitmapRefreshCycle = 1;
#endif

      //webViewObject.SetRectTransformMargin(rectTransform);

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

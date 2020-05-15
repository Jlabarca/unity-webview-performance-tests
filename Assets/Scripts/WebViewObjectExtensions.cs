using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Extensions
{
    public static class WebViewObjectExtensions
    {
        public static void SetRectTransformMargin(this WebViewObject webViewObject, RectTransform rectTransform)
        {
            var canvas = rectTransform.GetComponentInParent<Canvas>();
            var camera = canvas.worldCamera;
            var corners = new Vector3[4];

            rectTransform.GetWorldCorners(corners);

            var screenCorner1 = RectTransformUtility.WorldToScreenPoint(camera, corners[1]);
            var screenCorner3 = RectTransformUtility.WorldToScreenPoint(camera, corners[3]);

            var rect = new Rect();

            rect.x = screenCorner1.x;
            rect.width = screenCorner3.x - rect.x;
            rect.y = screenCorner3.y;
            rect.height = screenCorner1.y - rect.y;

            webViewObject.SetMargins
            (
                (int) rect.xMin,
                Screen.height - (int) rect.yMax,
                Screen.width - (int) rect.xMax,
                (int) rect.yMin
            );
        }
    }
}

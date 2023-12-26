using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlanetariumApp.UI
{

    public class SafeAreaHandler : MonoBehaviour
    {
        RectTransform rectTransform;
        Rect safeRect;
        Vector2 minAnchor;
        Vector2 maxAnchor;


        private IEnumerator Start()
        {

            yield return new WaitForSeconds(0.2f);
            AdjustCanvas();
        }

        public void AdjustCanvas()
        {
            ScreenOrientation screenOrientation = Screen.orientation;

            rectTransform = GetComponent<RectTransform>();
            safeRect = Screen.safeArea;

            minAnchor = safeRect.position;
            maxAnchor = minAnchor + safeRect.size;

            if (screenOrientation == ScreenOrientation.LandscapeLeft || screenOrientation == ScreenOrientation.LandscapeLeft || screenOrientation == ScreenOrientation.LandscapeRight)
            {
                minAnchor.x /= Screen.width;
                minAnchor.y = 0;


                maxAnchor.x /= Screen.width;
                maxAnchor.y = 1;
            }
            else
            {
                minAnchor.x = 0;
                minAnchor.y /= Screen.height;


                maxAnchor.x = 1;
                maxAnchor.y /= Screen.height;
            }

            rectTransform.anchorMin = minAnchor;
            rectTransform.anchorMax = maxAnchor;
        }
    }

}
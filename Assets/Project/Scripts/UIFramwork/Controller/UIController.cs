using BaseFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Game.BaseFramework
{
    [Serializable]
    public class UIScreen
    {
        public ScreenType screenType;
        public UIScreenView screenView;
    }

    public enum ScreenType
    {
        Register,
        Login,
        Mainmenu,
        Tableselection
    }
    public class UIController : IndestructibleSingleton<UIController>
    {
        public ScreenType StartScreen;
        public List<UIScreen> Screens;

        [SerializeField]
        List<ScreenType> currentScreens;
        [HideInInspector]
        public ScreenType previousScreen;
        public static float AspectRatio;

        public override void OnAwake()
        {
            base.OnAwake();
            AspectRatio = Screen.width / (Screen.height * 1f);
        }

        private IEnumerator Start()
        {
            Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
            currentScreens = new List<ScreenType>();

            yield return null;
            ShowScreen(StartScreen);

            yield return new WaitForSeconds(1f);

            //SavedDataHandler.Instance.SetFirstLaunch();
        }

        public void ShowNextScreen(ScreenType screenType, float Delay = 0f)
        {
            if (currentScreens.Count > 0)
            {
                HideScreen(currentScreens.Last());
            }
            else
            {
                Delay = 0;
            }

            StartCoroutine(ExecuteAfterDelay(Delay, () =>
            {
                ShowScreen(screenType);
            }));
        }

        public void ShowScreen(ScreenType screenType)
        {
            getScreen(screenType).Show();

            currentScreens.Add(screenType);
        }

        public void HideScreen(ScreenType screenType)
        {
            getScreen(screenType).Hide();

            currentScreens.Remove(screenType);
        }

        public UIScreenView getScreen(ScreenType screenType)
        {
            return Screens.Find(screen => screen.screenType == screenType).screenView;
        }

        public ScreenType getCurrentScreen()
        {
            return currentScreens.Last();
        }

        public ScreenType GetLastOpenScreen()
        {
            return currentScreens[currentScreens.Count - 1];
        }
        IEnumerator ExecuteAfterDelay(float Delay, Action CallbackAction)
        {
            yield return new WaitForSeconds(Delay);

            CallbackAction();
        }
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.BaseFramework
{
    public class SplashUI : UIScreenView
    {

        public Image ProgressBar;

        private IEnumerator Start()
        {
            
            yield return new WaitForSeconds(0.2f);

            Show();
        }

        public override void OnScreenShowCalled()
        {
            base.OnScreenShowCalled();
            StartCoroutine(LoadScene());
        }

        public override void OnScreenHideCalled()
        {
            base.OnScreenHideCalled();
        }

        IEnumerator LoadScene()
        {

            yield return new WaitForSeconds(2f);
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(1);
            asyncOperation.allowSceneActivation = false;
            while (!asyncOperation.isDone)
            {
                //Output the current progress
                ProgressBar.fillAmount = asyncOperation.progress;

                if (asyncOperation.progress >= 0.9f)
                {
                    yield return new WaitForSeconds(1f);
                    Hide();
                    yield return new WaitForSeconds(1f);
                    asyncOperation.allowSceneActivation = true;
                }

                yield return null;
            }
        }

    }
}
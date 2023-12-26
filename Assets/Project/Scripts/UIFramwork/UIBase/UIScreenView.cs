using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game.BaseFramework
{
    public class UIScreenView : UIBase
    {
        [HideInInspector]
        public Image Background;
        [HideInInspector]
        public RectTransform Parent;

        UIAnimator _uiAnimator;


        public override void OnAwake()
        {
            base.OnAwake();
            Background = transform.Find(BACKGROUND).GetComponent<Image>();
            Parent = transform.Find(PARENT).GetComponent<RectTransform>();
            _uiAnimator = GetComponent<UIAnimator>();
        }

        public override void OnScreenShowCalled()
        {
            ToggleCanvas(true);
            //ToggleRaycaster(true);
            base.OnScreenShowCalled();
        }

        public override void OnScreenShowAnimationCompleted()
        {
            base.OnScreenShowAnimationCompleted();

            BackKeyRoutine = StartCoroutine(BackKeyUpdateRoutine());
        }
        public override void OnBack()
        {
            base.OnBack();
        }

        Coroutine BackKeyRoutine;

        public override void OnScreenHideCalled()
        {
            base.OnScreenHideCalled();

            if (BackKeyRoutine != null)
            {
                StopCoroutine(BackKeyRoutine);
            }

            ToggleRaycaster(false);

            if (_uiAnimator == null)
            {
                OnScreenHideAnimationCompleted();
            }
        }

        public override void OnScreenHideAnimationCompleted()
        {
            base.OnScreenHideAnimationCompleted();
            ToggleCanvas(false);

        }

        IEnumerator BackKeyUpdateRoutine()
        {
            while (true)
            {
                if (Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    OnBack();
                }

                yield return null;
            }
        }

        public void StartStopBackKey(bool isActive)
        {
            if (BackKeyRoutine != null)
                StopCoroutine(BackKeyRoutine);
            if (isActive)
                BackKeyRoutine = StartCoroutine(BackKeyUpdateRoutine());
        }
    }
}
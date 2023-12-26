using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Game.BaseFramework
{
    public class FadeElementAnimatable : ElementAnimatable
    {
        CanvasGroup _canvasGroup;


        public override void IdleAnimation()
        {
        }

        public override void Initialize(UIScreenView screenView)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        public override void ResetAnimator()
        {
            _canvasGroup.alpha = 0;
        }

        public override void ShowAnimation(float time, Ease ease, float delay, ShowAnimationCompleted callback = null)
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.DOFade(1, time).SetEase(ease).SetDelay(delay).onComplete = () =>
            {
                if (callback != null)
                {
                    callback();
                }
            };
        }
        public override void HideAnimation(float time, Ease ease, float delay, AnimationTransition animationTransition = AnimationTransition.Reverse, HideAnimationCompleted callback = null)
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.DOFade(0, time).SetEase(ease).SetDelay(delay).onComplete = () =>
            {
                if (callback != null)
                {
                    callback();
                }
            };
        }
    }
}
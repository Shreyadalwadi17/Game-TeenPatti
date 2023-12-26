using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game.BaseFramework
{

    public class ScaleAnimatable : ScreenAnimatable
    {


        RectTransform _TargetRectTransform;
        Vector3 _initialScale;


        public override void Initialize(UIScreenView screenView)
        {

            _TargetRectTransform = screenView.Parent.GetComponent<RectTransform>();
            _initialScale = _TargetRectTransform.localScale;

            _TargetRectTransform.localScale = Vector3.zero;
        }

        public override void ResetAnimator()
        {
            _TargetRectTransform.localScale = Vector3.zero;
        }

        public override void ShowAnimation(float time, Ease ease, float delay, ShowAnimationCompleted callback)
        {
            ResetAnimator();

            _TargetRectTransform.DOScale(_initialScale, time).SetEase(ease).SetDelay(delay).onComplete = () =>
            {
                if (callback != null)
                {
                    callback();
                }
            };
        }

        public override void HideAnimation(float time, Ease ease, float delay, AnimationTransition animationTransition, HideAnimationCompleted callback)
        {

            _TargetRectTransform.DOScale(Vector3.zero, time).SetEase(ease).SetDelay(delay).onComplete = () =>
            {
                if (callback != null)
                {
                    callback();
                }
            };


        }

    }
}
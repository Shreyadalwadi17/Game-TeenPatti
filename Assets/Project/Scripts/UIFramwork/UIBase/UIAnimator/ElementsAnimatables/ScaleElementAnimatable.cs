using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Game.BaseFramework
{
    public class ScaleElementAnimatable : ElementAnimatable
    {

        Transform _TargetTransform;
        Vector3 _initialScale;


        public override void Initialize(UIScreenView screenView)
        {

            _TargetTransform = transform;
            _initialScale = _TargetTransform.localScale;

            _TargetTransform.localScale = Vector3.zero;

            gameObject.SetActive(false);

        }

        public override void ResetAnimator()
        {
            _TargetTransform.localScale = Vector3.zero;

            gameObject.SetActive(true);

        }

        public override void ShowAnimation(float time, Ease ease, float delay, ShowAnimationCompleted callback)
        {


            _TargetTransform.DOScale(_initialScale, time).SetEase(ease).SetDelay(delay).OnStart(ResetAnimator).onComplete = () =>
            {

                if (callback != null)
                {
                    callback();
                }

            };
        }

        public override void HideAnimation(float time, Ease ease, float delay, AnimationTransition animationTransition, HideAnimationCompleted callback)
        {

            _TargetTransform.DOScale(0, time).SetEase(ease).SetDelay(delay).onComplete = () =>
            {

                if (callback != null)
                {
                    callback();
                }

                gameObject.SetActive(false);

            };


        }

        public override void IdleAnimation()
        {
        }

    }
}


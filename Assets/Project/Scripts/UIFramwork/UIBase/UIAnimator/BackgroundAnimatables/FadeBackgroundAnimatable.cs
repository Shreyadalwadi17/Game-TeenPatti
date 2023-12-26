using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Game.BaseFramework
{
    public class FadeBackgroundAnimatable : BackgroundAnimatable
    {

        Image _TargetImage;
        float _initialAlpha;


        public override void Initialize(UIScreenView screenView)
        {
            _TargetImage = screenView.Background.GetComponent<Image>();
            _initialAlpha = _TargetImage.color.a;

            _TargetImage.color = new Color(_TargetImage.color.r, _TargetImage.color.g, _TargetImage.color.b, 0);
        }

        public override void ResetAnimator()
        {
            _TargetImage.color = new Color(_TargetImage.color.r, _TargetImage.color.g, _TargetImage.color.b, 0);
        }

        public override void ShowAnimation(float time, Ease ease, float delay, ShowAnimationCompleted callback)
        {

            ResetAnimator();

            _TargetImage.DOFade(_initialAlpha, time).SetEase(ease).SetDelay(delay).onComplete = () =>
            {

                if (callback != null)
                {
                    callback();
                }

            };
        }

        public override void HideAnimation(float time, Ease ease, float delay, AnimationTransition animationTransition, HideAnimationCompleted callback)
        {

            _TargetImage.DOFade(0, time).SetEase(ease).SetDelay(delay).onComplete = () =>
            {

                if (callback != null)
                {
                    callback();
                }

            };


        }

    }
}
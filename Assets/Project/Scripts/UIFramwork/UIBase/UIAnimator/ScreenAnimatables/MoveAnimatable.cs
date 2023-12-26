using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game.BaseFramework
{

    public class MoveAnimatable : ScreenAnimatable
    {

        public AnimationDirection direction;

        RectTransform _TargetRectTransform;
        Vector2 _initialPosition;

        float multiplier = 1f;


        public override void Initialize(UIScreenView screenView)
        {

            _TargetRectTransform = screenView.Parent.GetComponent<RectTransform>();
            _initialPosition = _TargetRectTransform.anchoredPosition;

            float offset = _TargetRectTransform.rect.width > _TargetRectTransform.rect.height ? _TargetRectTransform.rect.width : _TargetRectTransform.rect.height;
            //float offset = Screen.width > Screen.height ? Screen.width : Screen.height;
            offset *= multiplier;

            switch (direction)
            {
                case AnimationDirection.Top:
                    _TargetRectTransform.anchoredPosition = new Vector2(_initialPosition.x, _initialPosition.y + (offset));
                    break;

                case AnimationDirection.Bottom:
                    _TargetRectTransform.anchoredPosition = new Vector2(_initialPosition.x, _initialPosition.y - (offset));

                    break;

                case AnimationDirection.Left:
                    _TargetRectTransform.anchoredPosition = new Vector2(_initialPosition.x + (offset), _initialPosition.y);

                    break;

                case AnimationDirection.Right:
                    _TargetRectTransform.anchoredPosition = new Vector2(_initialPosition.x - (offset), _initialPosition.y);

                    break;
            }
        }

        public override void ResetAnimator()
        {
            float offset = _TargetRectTransform.rect.width > _TargetRectTransform.rect.height ? _TargetRectTransform.rect.width : _TargetRectTransform.rect.height;
            //float offset = Screen.width > Screen.height ? Screen.width : Screen.height;
            offset *= multiplier;


            switch (direction)
            {
                case AnimationDirection.Top:
                    _TargetRectTransform.anchoredPosition = new Vector2(_initialPosition.x, _initialPosition.y + (offset));
                    break;

                case AnimationDirection.Bottom:
                    _TargetRectTransform.anchoredPosition = new Vector2(_initialPosition.x, _initialPosition.y - (offset ));

                    break;

                case AnimationDirection.Left:
                    _TargetRectTransform.anchoredPosition = new Vector2(_initialPosition.x + (offset), _initialPosition.y);

                    break;

                case AnimationDirection.Right:
                    _TargetRectTransform.anchoredPosition = new Vector2(_initialPosition.x - (offset), _initialPosition.y);

                    break;
            }
        }

        public override void ShowAnimation(float time, Ease ease, float delay, ShowAnimationCompleted callback)
        {
            ResetAnimator();

            switch (direction)
            {
                case AnimationDirection.Top:
                    _TargetRectTransform.DOAnchorPosY(_initialPosition.y, time).SetEase(ease).SetDelay(delay).onComplete = () =>
                    {
                        if (callback != null)
                        {
                            callback();
                        }
                    };
                    break;

                case AnimationDirection.Bottom:
                    _TargetRectTransform.DOAnchorPosY(_initialPosition.y, time).SetEase(ease).SetDelay(delay).onComplete = () =>
                    {
                        if (callback != null)
                        {
                            callback();
                        }
                    };

                    break;

                case AnimationDirection.Left:
                    _TargetRectTransform.DOAnchorPosX(_initialPosition.x, time).SetEase(ease).SetDelay(delay).onComplete = () =>
                    {
                        if (callback != null)
                        {
                            callback();
                        }
                    };

                    break;

                case AnimationDirection.Right:
                    _TargetRectTransform.DOAnchorPosX(_initialPosition.x, time).SetEase(ease).SetDelay(delay).onComplete = () =>
                    {
                        if (callback != null)
                        {
                            callback();
                        }
                    };

                    break;

                case AnimationDirection.NoMovement:

                    _TargetRectTransform.DOAnchorPosX(_initialPosition.x, time).SetEase(ease).SetDelay(delay).onComplete = () =>
                    {
                        if (callback != null)
                        {
                            callback();
                        }
                    };

                    break;
            }
        }

        public override void HideAnimation(float time, Ease ease, float delay, AnimationTransition animationTransition, HideAnimationCompleted callback)
        {

            Vector2 destination = Vector2.zero;
            float offset = _TargetRectTransform.rect.width > _TargetRectTransform.rect.height ? _TargetRectTransform.rect.width : _TargetRectTransform.rect.height;// Screen.width > Screen.height ? Screen.width : Screen.height;
            offset *= multiplier;


            switch (direction)
            {
                case AnimationDirection.Top:

                    if (animationTransition == AnimationTransition.Forward)
                    {
                        destination = new Vector2(_initialPosition.x, _initialPosition.y + (offset));
                    }
                    else
                    {
                        destination = new Vector2(_initialPosition.x, _initialPosition.y - (offset));
                    }

                    break;

                case AnimationDirection.Bottom:

                    if (animationTransition == AnimationTransition.Forward)
                    {
                        destination = new Vector2(_initialPosition.x, _initialPosition.y - (offset));
                    }
                    else
                    {
                        destination = new Vector2(_initialPosition.x, _initialPosition.y + (offset));
                    }
                    break;

                case AnimationDirection.Left:

                    if (animationTransition == AnimationTransition.Forward)
                    {
                        destination = new Vector2(_initialPosition.x + (offset), _initialPosition.y);

                    }
                    else
                    {
                        destination = new Vector2(_initialPosition.x - (offset), _initialPosition.y);
                    }

                    break;

                case AnimationDirection.Right:


                    if (animationTransition == AnimationTransition.Forward)
                    {
                        destination = new Vector2(_initialPosition.x - (offset), _initialPosition.y);
                    }
                    else
                    {
                        destination = new Vector2(_initialPosition.x + (offset), _initialPosition.y);
                    }

                    break;

                case AnimationDirection.NoMovement:

                    if (animationTransition == AnimationTransition.Forward)
                    {
                        destination = new Vector2(_initialPosition.x , _initialPosition.y);
                    }
                    else
                    {
                        destination = new Vector2(_initialPosition.x, _initialPosition.y);
                    }

                    break;
            }

            _TargetRectTransform.DOAnchorPos(destination, time).SetEase(ease).SetDelay(delay).onComplete = () =>
            {
                if (callback != null)
                {
                    callback();
                }
            }; ;


        }

    }
}
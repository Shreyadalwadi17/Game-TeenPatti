using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Game.BaseFramework
{

    public class MoveElementAnimatable : ElementAnimatable
    {

        public AnimationDirection animationDirection;

        RectTransform _Parent;
        RectTransform _TargetTransform;
        Vector3 _initialPosition;

        float multiplier = 2f;
        public void SetInitPos()
        {
            _initialPosition = _TargetTransform.anchoredPosition3D;
        }
        public override void Initialize(UIScreenView screenView)
        {
            _Parent = screenView.GetComponent<RectTransform>();
            _TargetTransform = GetComponent<RectTransform>();
            _initialPosition = _TargetTransform.anchoredPosition3D;

            float offset = _Parent.rect.width > _Parent.rect.height ? _Parent.rect.width : _Parent.rect.height;
            //float offset = Screen.width > Screen.height ? Screen.width : Screen.height;
            offset *= multiplier;


            switch (animationDirection)
            {
                case AnimationDirection.Top:
                    _TargetTransform.anchoredPosition = new Vector2(_initialPosition.x, _initialPosition.y + offset);
                    break;

                case AnimationDirection.Bottom:
                    _TargetTransform.anchoredPosition = new Vector2(_initialPosition.x, _initialPosition.y - offset);
                    break;

                case AnimationDirection.Left:
                    _TargetTransform.anchoredPosition = new Vector2(_initialPosition.x - offset, _initialPosition.y);
                    break;

                case AnimationDirection.Right:
                    _TargetTransform.anchoredPosition = new Vector2(_initialPosition.x + offset, _initialPosition.y);
                    break;
            }

            gameObject.SetActive(false);

        }

        public override void ResetAnimator()
        {
            float offset = _Parent.rect.width > _Parent.rect.height ? _Parent.rect.width : _Parent.rect.height;
            //float offset = Screen.width > Screen.height ? Screen.width : Screen.height;
            offset *= multiplier;

            switch (animationDirection)
            {
                case AnimationDirection.Top:
                    _TargetTransform.anchoredPosition = new Vector2(_initialPosition.x, _initialPosition.y + offset);
                    break;

                case AnimationDirection.Bottom:
                    _TargetTransform.anchoredPosition = new Vector2(_initialPosition.x, _initialPosition.y - offset);
                    break;

                case AnimationDirection.Left:
                    _TargetTransform.anchoredPosition = new Vector2(_initialPosition.x - offset, _initialPosition.y);
                    break;

                case AnimationDirection.Right:
                    _TargetTransform.anchoredPosition = new Vector2(_initialPosition.x + offset, _initialPosition.y);
                    break;
            }

            gameObject.SetActive(true);

        }

        public override void ShowAnimation(float time, Ease ease, float delay, ShowAnimationCompleted callback)
        {
            ResetAnimator();
            _TargetTransform.DOAnchorPos(_initialPosition, time).SetEase(ease).SetDelay(delay).OnStart(ResetAnimator).onComplete = () =>
            {

                if (callback != null)
                {
                    callback();
                }

            };
        }

        public override void HideAnimation(float time, Ease ease, float delay, AnimationTransition animationTransition, HideAnimationCompleted callback)
        {
            float offset = _Parent.rect.width > _Parent.rect.height ? _Parent.rect.width : _Parent.rect.height;
            //float offset = Screen.width > Screen.height ? Screen.width : Screen.height;
            offset *= multiplier;

            Vector2 _endPosition = Vector2.zero;

            switch (animationDirection)
            {

                case AnimationDirection.Top:

                    if (animationTransition == AnimationTransition.Forward)
                    {
                        _endPosition = new Vector2(_initialPosition.x, _initialPosition.y + offset);

                    }
                    else
                    {
                        _endPosition = new Vector2(_initialPosition.x, _initialPosition.y - offset);
                    }

                    break;

                case AnimationDirection.Bottom:
                    if (animationTransition == AnimationTransition.Forward)
                    {
                        _endPosition = new Vector2(_initialPosition.x, _initialPosition.y - offset);

                    }
                    else
                    {
                        _endPosition = new Vector2(_initialPosition.x, _initialPosition.y + offset);
                    }
                    break;

                case AnimationDirection.Left:

                    if (animationTransition == AnimationTransition.Forward)
                    {
                        _endPosition = new Vector2(_initialPosition.x - offset, _initialPosition.y);
                    }
                    else
                    {
                        _endPosition = new Vector2(_initialPosition.x + offset, _initialPosition.y);
                    }

                    break;

                case AnimationDirection.Right:

                    if (animationTransition == AnimationTransition.Forward)
                    {
                        _endPosition = new Vector2(_initialPosition.x + offset, _initialPosition.y);
                    }
                    else
                    {
                        _endPosition = new Vector2(_initialPosition.x - offset, _initialPosition.y);
                    }
                    break;
            }

            _TargetTransform.DOAnchorPos(_endPosition, time).SetEase(ease).SetDelay(delay).onComplete = () =>
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


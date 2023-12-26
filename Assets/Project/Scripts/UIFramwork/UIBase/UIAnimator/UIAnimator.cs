using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

namespace Game.BaseFramework
{

    [RequireComponent(typeof(ScreenAnimatable))]
    public class UIAnimator : MonoBehaviour
    {


        public Ease InAnimationEase = Ease.OutExpo;
        public Ease OutAnimationEase = Ease.OutExpo;
        public float InTime = 0.7f;
        public float OutTime = 0.5f;



        public List<Animatable> animatables;
        UIScreenView _UIScreenView;



        private void Start()
        {
            if (!_UIScreenView)
            {
                Initialize();
            }
        }

        private void OnDestroy()
        {
            CleanUp();
        }

        private void Initialize()
        {
            _UIScreenView = GetComponentInParent<UIScreenView>();
            _UIScreenView.OnScreenStateChanged += OnScreenStateChanged;

            if (animatables != null)
            {
                animatables.Clear();
            }

            animatables = new List<Animatable>(GetComponentsInChildren<Animatable>());
            animatables.Sort();

            foreach (Animatable animatable in animatables)
            {
                animatable.Initialize(_UIScreenView);
            }
        }

        private void CleanUp()
        {
            if (_UIScreenView)
            {
                _UIScreenView.OnScreenStateChanged -= OnScreenStateChanged;
                animatables.Clear();
            }
        }

        private void OnScreenStateChanged(CanvasState _state)
        {

            switch (_state)
            {
                case CanvasState.Active:

                    foreach (Animatable animatable in animatables)
                    {
                        if (animatable is ScreenAnimatable)
                        {
                            animatable.ShowAnimation(InTime, InAnimationEase, animatable.AnimaionLayer * InTime, _UIScreenView.OnScreenShowAnimationCompleted);
                        }
                        else
                        {
                            animatable.ShowAnimation(InTime, InAnimationEase, animatable.AnimaionLayer * InTime * 0.2f, null);
                        }
                    }

                    break;

                case CanvasState.Inactive:


                    animatables.Reverse();
                    foreach (Animatable animatable in animatables)
                    {
                        if (animatable is ScreenAnimatable)
                        {
                            animatable.HideAnimation(OutTime, OutAnimationEase, animatable.AnimaionLayer * OutTime, animatable.AnimationTransition, _UIScreenView.OnScreenHideAnimationCompleted);
                        }
                        else
                        {
                            animatable.HideAnimation(OutTime, OutAnimationEase, 0, animatable.AnimationTransition, null);
                        }
                    }
                    animatables.Reverse();

                    break;
            }
        }
    }
}
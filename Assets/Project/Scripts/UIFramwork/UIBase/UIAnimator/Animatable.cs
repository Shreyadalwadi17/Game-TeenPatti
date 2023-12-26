using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Game.BaseFramework
{
    public enum AnimationDirection
    {
        Top,
        Bottom,
        Left,
        Right,
        NoMovement
    }

    public enum AnimationTransition
    {
        Forward,
        Reverse
    }



    [DisallowMultipleComponent]
    [System.Serializable]
    public abstract class Animatable : MonoBehaviour, IComparer<Animatable>, IComparable<Animatable>
    {

        [Header("Animation configration :")]
        public int AnimaionLayer;
        public AnimationTransition AnimationTransition = AnimationTransition.Reverse;

        public delegate void ShowAnimationCompleted();
        public ShowAnimationCompleted OnShowAnimationCompleted;

        public delegate void HideAnimationCompleted();
        public HideAnimationCompleted OnHideAnimationCompleted;

        public abstract void Initialize(UIScreenView screenView);

        public abstract void ResetAnimator();


        public abstract void ShowAnimation(float time, Ease ease, float delay, ShowAnimationCompleted callback = null);
        public abstract void HideAnimation(float time, Ease ease, float delay, AnimationTransition animationTransition = AnimationTransition.Reverse, HideAnimationCompleted callback = null);


        public int Compare(Animatable x, Animatable y)
        {
            return x.AnimaionLayer.CompareTo(y.AnimaionLayer);
        }

        public int CompareTo(Animatable other)
        {
            return AnimaionLayer.CompareTo(other.AnimaionLayer);

        }
    }




    public abstract class ScreenAnimatable : Animatable
    {

        public ScreenAnimatable()
        {
            AnimaionLayer = 0;
        }

    }


    public abstract class BackgroundAnimatable : Animatable
    {

        public BackgroundAnimatable()
        {
            AnimaionLayer = 0;
        }

    }

    public abstract class ElementAnimatable : Animatable
    {

        public ElementAnimatable()
        {
            AnimaionLayer = 1;
        }

        public abstract void IdleAnimation();
    }
}


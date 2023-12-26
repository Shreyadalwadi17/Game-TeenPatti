using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace Game.BaseFramework
{
    //!!!!!!!!!!!              STILL IN DEVELOPMENT  17 - 1 - 23             !!!!!!!!!!!!!!
    public class AnimationAnimatable : ScreenAnimatable
    {
        Animator _screenAnimator;


        //Triggers to be used : "Show" - For showing a screen, "Hide" - For Hiding a screen, "Reset" - For resetting to default state. 
        string Show = "Show", Hide = "Hide", Reset = "Reset";

        int ShowHash, HideHash, ResetHash;

        public override void Initialize(UIScreenView screenView)
        {
            _screenAnimator = screenView.GetComponent<Animator>();

            if (_screenAnimator == null)
            {
                screenView.AddComponent<Animator>();

                Debug.Log("Screen Animator is added externally. Might create problems");
            }

            ShowHash = Animator.StringToHash(Show);
            HideHash = Animator.StringToHash(Hide);
            ResetHash = Animator.StringToHash(Reset);
        }
        public override void ResetAnimator()
        {
            _screenAnimator.SetTrigger(ResetHash);
        }
        public override void ShowAnimation(float time, Ease ease, float delay, ShowAnimationCompleted callback = null)
        {
            StartCoroutine(ShowAnimationRoutine(callback));
        }

        public override void HideAnimation(float time, Ease ease, float delay, AnimationTransition animationTransition = AnimationTransition.Reverse, HideAnimationCompleted callback = null)
        {

            StartCoroutine(HideAnimationRoutine(callback));
        }

        IEnumerator ShowAnimationRoutine(ShowAnimationCompleted callback = null)
        {
            _screenAnimator.SetTrigger(ShowHash);

            yield return null;

            AnimatorStateInfo currInfo = _screenAnimator.GetCurrentAnimatorStateInfo(0);

            yield return new WaitForSeconds(currInfo.length);

            callback?.Invoke();

        }

        IEnumerator HideAnimationRoutine(HideAnimationCompleted callback = null)
        {
            _screenAnimator.SetTrigger(HideHash);

            yield return null;

            AnimatorStateInfo currInfo = _screenAnimator.GetCurrentAnimatorStateInfo(0);

            yield return new WaitForSeconds(currInfo.length);

            callback?.Invoke();

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.BaseFramework
{
    public enum CanvasState
    {
        Active,
        Inactive
    }

    public enum Direction
    {
        Forward,
        Reverse
    }

    public abstract class UIBase : MonoBehaviour
    {
        public static string BACKGROUND = "Background";
        public static string   PARENT = "Parent";

        public delegate void ScreenStateChanged(CanvasState _state);
        public event ScreenStateChanged OnScreenStateChanged;

        public Canvas BaseCanvas { get; }
        public GraphicRaycaster BaseRaycaster { get; }

        public CanvasState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;

                NotifyStateChanged(_state);
            }
        }
        CanvasState _state;
        Canvas _canvas;
        GraphicRaycaster _raycaster;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _raycaster = GetComponent<GraphicRaycaster>();

            ResetCanvas();
            OnAwake();
        }

        public virtual void OnAwake() { }

        void ToggleCanvasState(bool hasToEnable)
        {

            if (hasToEnable)
            {
                State = CanvasState.Active;
            }
            else
            {
                State = CanvasState.Inactive;
            }
        }

        void ResetCanvas()
        {
            _raycaster.enabled = false;
            _canvas.enabled = false;
            _state = CanvasState.Inactive;
        }

        void NotifyStateChanged(CanvasState state)
        {
            if (OnScreenStateChanged != null)
            {
                OnScreenStateChanged(state);
            }
        }

        public virtual void OnBack()
        {

        }

        public virtual void Show()
        {
            OnScreenShowCalled();
            ToggleCanvasState(true);
        }
        public virtual void Hide()
        {
            OnScreenHideCalled();
            ToggleCanvasState(false);
        }


        public void ToggleCanvas(bool value)
        {
            ToggleRaycaster(value);
            _canvas.enabled = value;
        }
        public void ToggleRaycaster(bool value)
        {
            //Debug.LogError(gameObject.name + " Raycast : "+value );
            _raycaster.enabled = value;
            
        }

        public virtual void OnScreenShowCalled()
        {
        }

        public virtual void OnScreenHideCalled() { }

        public virtual void OnScreenShowAnimationCompleted() {
        }

        public virtual void OnScreenHideAnimationCompleted()
        {
        }


    }
}
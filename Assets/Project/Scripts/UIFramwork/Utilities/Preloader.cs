
using BaseFramework;
using UnityEngine;

public class Preloader : IndestructibleSingleton<Preloader> 
{
    [SerializeField] Transform loader;
    Canvas canvas;

    bool isON;

    public override void OnAwake()
    {
        canvas = this.GetComponent<Canvas> ();
        canvas.enabled = false;
    }

    /*public void Awake () {
        
        canvas = this.GetComponent<Canvas> ();
        canvas.enabled = false;
    }*/

    public void Show () {
        if (isON) return;
        isON = true;
        canvas.enabled = true;
        TickTimer.OnUpdateTick += TickTimer_OnUpdateTick;
        Invoke ("Hide", 5);
    }

    void TickTimer_OnUpdateTick () {
        loader.Rotate (loader.forward, 5f * -1);
    }

    public void Hide () {
        if (!isON) return;
        isON = false;
        canvas.enabled = false;
        TickTimer.OnUpdateTick -= TickTimer_OnUpdateTick;
    }
}
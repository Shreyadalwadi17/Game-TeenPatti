using System;
using UnityEngine;
/// <summary>
/// This is the only update used in the entire game.
/// </summary>
public class TickTimer : MonoBehaviour {
    //this event will fire on every MAX_TIME_TICK
    public static event Action OnTick = delegate { };
    //this event will fire on every frame update.
    public static event Action OnUpdateTick;
    private const float MAX_TIMER_TICK = 1f;
    private float tickTimer;

    private void Update () {
        // if (!canvas.enabled) return;
        tickTimer += Time.deltaTime;

        if (OnUpdateTick != null) OnUpdateTick ();

        if (tickTimer >= MAX_TIMER_TICK) {
            tickTimer -= MAX_TIMER_TICK;
            OnTick ();
        }
    }
}
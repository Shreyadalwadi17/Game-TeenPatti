using UnityEngine;
using System.Collections;

public class DelayedActionHelper : MonoBehaviour
{

    public float delayTime = 2.0f; // default delay time is 2 seconds

    public void DelayedAction(System.Action action)
    {
        StartCoroutine(DelayCoroutine(action, delayTime));
    }

    IEnumerator DelayCoroutine(System.Action action, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        action();
    }
}

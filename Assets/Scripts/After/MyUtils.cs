using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyUtils : MonoBehaviour
{
    public void WaitAndDo(float seconds, Action action)
    {
        StartCoroutine(WaitAndDoCoroutine(seconds, action));
    }
    
    IEnumerator WaitAndDoCoroutine(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action.Invoke();
    }
}

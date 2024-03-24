using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GK_Save : MonoBehaviour
{
    [SerializeField] Transform rightHand;
    [SerializeField] Transform leftHand;
    [SerializeField] Transform ball;
    private bool save = false;
    private float animTime = 0.5f;
    
    Vector3 firstPos;

    // Start is called before the first frame update
    void Start()
    {
        firstPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(save)
        {
            ball.position = (leftHand.position + rightHand.position) * 0.5f;
        }
    }

    public void SetSaving(float waitingTime, bool save = true)
    {
        Debug.Log("simulationTime2: " + waitingTime);
        if(waitingTime == 0 || !save)
        {
            this.save = save;
            return;
        }
        StartCoroutine(WaitAndDo(waitingTime, () => {
            this.save = save;
            ball.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            ball.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }));
    }

    IEnumerator WaitAndDo(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action.Invoke();
    }

    public void ResetGoalkeeper()
    {
        transform.position = firstPos;
        save = false;
    }
}

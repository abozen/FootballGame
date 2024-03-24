using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GK_Dive : MonoBehaviour
{
    //Gate info
    public float gateSize;
    [SerializeField] private Transform gate;
    private Vector3 gatePos;
    float leftPostX;
    float rightPostX;
    float gateHeight;
    int gatePart;


    //Ball info
    Vector3 futureBallPos;

    //Goalkeeper info
    [SerializeField] Animator animator;
    [SerializeField] Transform goalkeeper;
    [SerializeField] float goalkeeperSize;
    [SerializeField] private float gkSpeed;
    private bool isSaving;
    private int savingState;



    Coroutine animatorDefault;
    Coroutine getCloser;
    [SerializeField] float distForDiving;
    [SerializeField] private float animTime;
    private float animTimeRT = 0.6f;
    private float animTimeRM = 0.7f;
    private float animTimeRB = 0.7f;
    private float animTimeMT = 0.6f;
    private float animTimeMM = 0.45f;
    private float animTimeMB = 0.5f;

    [SerializeField] private GameObject test;


    // Start is called before the first frame update
    void Start()
    {
        //gate
        gatePos = gate.position;
        leftPostX = gatePos.x - gateSize * 0.5f;
        rightPostX = gatePos.x + gateSize * 0.5f;
        gateHeight = gateSize / 4 + 1f;

        //gk

    }



    public void Dive(float simulationTime = 0, float sideStepTime = 0, bool isWaited = false)
    {
        if (Mathf.Abs(goalkeeper.position.x - futureBallPos.x) > distForDiving && !isWaited)
        {
            GetCloserHelper(simulationTime);
            return;
        }

        if (!isWaited)
        {
            DivingHelper(simulationTime, sideStepTime);
            return;
        }

        switch (gatePart)
        {
            case 1:
                animator.SetBool("isShotRB", true);
                SetAnimFalseHelper("isShotRB", true);
                break;
            case 2:
                animator.SetBool("isShotBM", true);
                SetAnimFalseHelper("isShotBB", true);
                break;
            case 3:
                animator.SetBool("isShotLB", true);
                SetAnimFalseHelper("isShotLB", true);
                break;
            case 4:
                animator.SetBool("isShotRM", true);
                SetAnimFalseHelper("isShotRM", true);
                break;
            case 5:
                animator.SetBool("isShotMM", true);
                SetAnimFalseHelper("isShotMM", true);
                break;
            case 6:
                animator.SetBool("isShotLM", true);
                SetAnimFalseHelper("isShotLM", true);
                break;
            case 7:
                animator.SetBool("isShotRT", true);
                SetAnimFalseHelper("isShotRT", true);
                break;
            case 8:
                animator.SetBool("isShotTM", true);
                SetAnimFalseHelper("isShotTM", true);
                break;
            case 9:
                animator.SetBool("isShotLT", true);
                SetAnimFalseHelper("isShotLT", true);
                break;
            default:
                // code block
                break;
        }

    }
    private void DivingHelper(float simulationTime, float sideStepTime)
    {
        gatePart = WhichPartHorizontal() + WhichPartVertical();
        gatePart = test.GetComponent<Savability>().UpdateGatePart(gatePart);
        SetAnimTime(gatePart);

        //state 1: score -- 2: save -- 3: out
        savingState = test.GetComponent<Savability>().GetSavablityState(futureBallPos);

        if (savingState == 1 || savingState == 3)
        {
            isSaving = false;
        }
        else
        {
            isSaving = true;
        }
        Debug.Log("SavableState: " + savingState);
        goalkeeper.GetComponent<GK_Save>().SetSaving(simulationTime - sideStepTime, isSaving);
        float waitingTime = Mathf.Max(0.01f, simulationTime - sideStepTime - animTime);
        Debug.Log("waitingTime: " + waitingTime);
        StartCoroutine(WaitAndDo(waitingTime, () => Dive(0, 0, true)));
    }

    private void GetCloserHelper(float waitingTime = 0)
    {
        if (getCloser != null)
            StopCoroutine(getCloser);
        getCloser = StartCoroutine(GetCloser(waitingTime));

    }

    IEnumerator GetCloser(float simulationTime = 0)
    {
        int direction;
        float startTime = Time.time;

        if (goalkeeper.position.x < futureBallPos.x)
        {
            animator.SetBool("isLSS", true);
            direction = 1;
        }
        else
        {
            animator.SetBool("isRSS", true);
            direction = -1;
        }

        while (Mathf.Abs(goalkeeper.position.x - futureBallPos.x) >= distForDiving)
        {
            goalkeeper.position += Vector3.right * gkSpeed * direction * Time.deltaTime;

            yield return null;
        }

        float sideStepTime = Time.time - startTime;
        Debug.Log("sidestepTime: " + sideStepTime);

        Dive(simulationTime, sideStepTime);
    }

    void SetAnimFalseHelper(string animatonName, bool isIdle = false)
    {
        if (animatorDefault != null && false)
            StopCoroutine(animatorDefault);

        animatorDefault = StartCoroutine(SetAnimFalse(animatonName, isIdle));
    }

    IEnumerator SetAnimFalse(string animatonName, bool isIdle = false)
    {
        yield return new WaitForSeconds(0.2f);
        animator.SetBool(animatonName, false);
        if (isIdle)
            SetIdleAnim();
    }

    void SetIdleAnim()
    {
        animator.SetBool("isShotLB", false);
        animator.SetBool("isShotRB", false);
        animator.SetBool("isShotLT", false);
        animator.SetBool("isShotRT", false);
        animator.SetBool("isShotTM", false);
        animator.SetBool("isShotMM", false);
        animator.SetBool("isShotBM", false);
        animator.SetBool("isRSS", false);
        animator.SetBool("isLSS", false);
        animator.SetBool("isShotLM", false);
        animator.SetBool("isShotRM", false);
    }

    void SetAnimTime(int gatePart)
    {
        switch (gatePart)
        {
            case 1:
                animTime = animTimeRB;
                break;
            case 2:
                animTime = animTimeMB;
                break;
            case 3:
                animTime = animTimeRB;
                break;
            case 4:
                animTime = animTimeRM;
                break;
            case 5:
                animTime = animTimeMM;
                break;
            case 6:
                animTime = animTimeRM;
                break;
            case 7:
                animTime = animTimeRT;
                break;
            case 8:
                animTime = animTimeMT;
                break;
            case 9:
                animTime = animTimeRT;
                break;
            default:
                // code block
                break;
        }
    }

    int WhichPartHorizontal()
    {
        if (futureBallPos.x > goalkeeper.position.x + goalkeeperSize)
        {
            return 3;
        }
        else if (futureBallPos.x < goalkeeper.position.x - goalkeeperSize)
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }
    int WhichPartVertical()
    {
        if (futureBallPos.y >= gateHeight * 0.7f)
        {
            return 6;
        }
        else if (futureBallPos.y >= gateHeight * 0.3f)
        {
            return 3;
        }
        else
        {
            return 0;
        }
    }

    IEnumerator WaitAndDo(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action.Invoke();
    }

    public void SetFutureBallPos(Vector3 ballPos)
    {
        futureBallPos = ballPos;
    }

}

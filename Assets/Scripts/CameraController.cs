using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    bool isDriblingCamState1 = true;
    bool isDriblingCamState2 = false;
    bool isShootingCamState = false;
    public Transform target;
    public Transform goal;


    public float smoothSpeed = 0.125f;
    public Vector3 offset;  // offset for dribling cam state
    public Vector3 offset2; // offset for shooting cam state
    private Vector3 ballToGoalVec, vec;
    float slope;
    public float distanceToTarget = 2f;

    private float maxSlope = 1f;

    private float ballToGoalX;
    private float ballToGoalZ;

    float distGoalToBall; // distance between goal and ball
    private float smilarityEfficient;

    public Transform midVec;

    private Vector3 driblingVec;

    public float disEff = 1f;
    public float camSpeed = 1f;

    Rigidbody ballRb;

    Coroutine startCo;


    bool starEffectDone = true;



    // Start is called before the first frame update
    void Start()
    {
        ballToGoalVec = target.position - goal.position;
        ballRb = target.gameObject.GetComponent<Rigidbody>();
        //startCo = StartCoroutine(AttackStart());
        
        //offset = new Vector3(0, transform.position.y - target.position.y, 0);
    }

    void LateUpdate()
    {

        if (starEffectDone)
        {
            ballToGoalVec = target.position - goal.position;
            //slope = Mathf.Clamp(ballToGoalVec.x / ballToGoalVec.z, -maxSlope, maxSlope);
            ballToGoalX = (goal.position.x - target.position.x);
            ballToGoalZ = (goal.position.z - target.position.z);
            distGoalToBall = Vector3.Distance(target.position, goal.position);

            smilarityEfficient = distanceToTarget / distGoalToBall;

            midVec.position = goal.position - new Vector3(-ballToGoalVec.x * 0.8f, 0, -ballToGoalVec.z * 0.8f);

            //ballToGoalVec = new Vector3(ballToGoalVec.x / ballToGoalVec.z, 0, 1);
            if (isDriblingCamState1)
            {
                Vector3 desiredPosition = target.position - new Vector3(distanceToTarget * smilarityEfficient * ballToGoalX, 0,
                                                                         distanceToTarget * smilarityEfficient * ballToGoalZ) + offset;
                //Vector3 desiredPosition = target.position + ballToGoalVec * 0.1f + offset;
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
                transform.position = desiredPosition;

                transform.LookAt(midVec);
            }
            else if (isDriblingCamState2)
            {
                //couldnt make it YET
                driblingVec = ballRb.velocity;
                float a = Mathf.Sqrt(driblingVec.x * driblingVec.x + driblingVec.z * driblingVec.x);
                smilarityEfficient = a != 0 ? distanceToTarget / a : 0.001f;

                Vector3 desiredPosition = target.position - new Vector3(smilarityEfficient * driblingVec.x + 0.001f, 0,
                                                                          smilarityEfficient * driblingVec.z + 0.001f) + offset;


                transform.position = Vector3.MoveTowards(transform.position, desiredPosition, camSpeed * Time.deltaTime);

                transform.LookAt(target);
            }
            else if (isShootingCamState)
            {
                Vector3 desiredPosition = target.position - new Vector3(0.35f * distanceToTarget * smilarityEfficient * ballToGoalX, 0,
                                                                         0.35f * distanceToTarget * smilarityEfficient * ballToGoalZ) + offset2;

                transform.position = desiredPosition;

                transform.LookAt(target);
            }

        }

    }

    IEnumerator AttackStart()
    {
        if(startCo != null)
            StopCoroutine(startCo);

        Time.timeScale = 0f;
        transform.eulerAngles = new Vector3(45, 0, 0);
        transform.position = new Vector3(0, 100, -30);//startPos.position;
        float step = 15f * Time.deltaTime;
        Vector3 endPos = target.position;

        while (Vector3.Distance(transform.position, target.position) > 1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);

            yield return null;
        }
        
        starEffectDone = true;
        Time.timeScale = 1f;

    }

    public void SetCamState(float camState)
    {
        if (camState == 1)
        {
            isDriblingCamState1 = true;
            isShootingCamState = false;
            isDriblingCamState2 = false;
        }
        else if (camState == 2)
        {
            isShootingCamState = true;
            isDriblingCamState1 = false;
            isDriblingCamState2 = false;
        }
        else if (camState == 3)
        {
            isShootingCamState = false;
            isDriblingCamState1 = false;
            isDriblingCamState2 = true;
        }
        else
        {
            isDriblingCamState1 = false;
            isShootingCamState = false;
            isDriblingCamState2 = false;
        }
    }

    public void SetDriblingVector(Vector3 vec)
    {
        driblingVec = new Vector3(vec.x, 0, vec.z);

    }
}

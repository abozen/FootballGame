using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class GoalkeeperScript : MonoBehaviour
{
    public Transform ball;
    public Transform gate;
    private Vector3 gatePos; // pos in field
    public float gateSize; // length of soccer gate;
    public float pitchSize;

    Vector3 gateArea; // area on which will move goalkeeper
    Transform goalkeeper;

    [SerializeField]
    float speed; // goalkeeper speed

    float distance;
    float move;

    Vector3 calculatedBallPos; // calculated ball pos after the shoot

    private bool isDriblingState = true; // is player dribling the ball

    private float distanceOfBall; // distance between ball and goalkeeper when ball came into gate
    public float accaptebleDistance; // accaptebel distanceOFBall for saving the ball
    public float goalkeeperCapability;

    public Transform rightHand;
    public Transform rightHandBone;
    public Transform leftHand;
    public Transform leftHandBone;
    public Transform tempCalc;

    public Rigidbody ballRb;

    Vector3 distanceOfRightHand;
    Vector3 distanceOfLeftHand;

    Animator animator;
    float noiseValue;
    Vector3 jumpDirection;

    private Rigidbody rb;
    public float jumpForce = 3f;

    public GameObject leftHandMover;
    TwoBoneIKConstraint leftConstraintScript;

    public GameObject rightHandMover;
    TwoBoneIKConstraint rightConstraintScript;

    public GameObject player;

    bool isShot = false;

    public float midTolerance = 0.2f;

    bool ballRight = false;
    bool ballLeft = false;
    bool ballMid = false;

    Coroutine sideStep;
    Coroutine saveTheBall;
    [SerializeField]
    float sideStepSpeed = 1f;

    bool sideStepped = false;

    // times that gk anims activate with ball after anim starts
    public float animTime = 0.1f;
    float midAnimTime = 1.6f;
    float othersAnimTime = 0.7f;

    float shotTime; // time that ball will be in the goal just after shoot

    public bool isBallOnHand = false;

    bool isShotBM = false;
    bool isShotMM = false;
    bool isShotTM = false;
    bool isShotRB = false;
    bool isShotRM = false;
    bool isShotRT = false;
    bool isShotLB = false;
    bool isShotLT = false;
    bool isShotLM = false;

    [SerializeField] float savebleDistance = 4.5f;


    void Start()
    {
        distanceOfRightHand = transform.position - rightHand.position;
        distanceOfLeftHand = transform.position - leftHand.position;
        leftConstraintScript = leftHandMover.GetComponent<TwoBoneIKConstraint>();
        rightConstraintScript = rightHandMover.GetComponent<TwoBoneIKConstraint>();

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        gatePos = gate.position;
        goalkeeper = gameObject.transform;

        gateArea = new Vector3(gatePos.x - gateSize * 0.5f, gatePos.x + gateSize * 0.5f, gateSize / 4 + 1f);

        goalkeeper.position = new Vector3(gatePos.x, gatePos.y, gatePos.z); // put goalkepper in middle of the gate
        noiseValue = Mathf.PerlinNoise(Time.time, 0f);
        calculatedBallPos = Vector3.zero;
    }

    void Update()
    {
        tempCalc.position = calculatedBallPos;
        if (isDriblingState)
        {
            // move goalkeeper within soccer gate
            goalkeeper.position = new Vector3(Mathf.Clamp((ball.position.x / pitchSize) * gateSize, gateArea.x, gateArea.y),
                                                goalkeeper.position.y,
                                                goalkeeper.position.z);
            transform.LookAt(ball);
        }
        else
        {
            if (isShot)
            {
                if (calculatedBallPos.x - goalkeeper.position.x > 0)
                {
                    leftHand.position = new Vector3(calculatedBallPos.x, calculatedBallPos.y, leftHandBone.position.z);

                }
                else
                {
                    rightHand.position = new Vector3(ball.position.x, ball.position.y, rightHandBone.position.z);
                }

            }
        }
        if (!isShot)
        {
            leftConstraintScript.weight = 0f;
            rightConstraintScript.weight = 0f;
        }
        if (isBallOnHand)
        {
            Vector3 vecToOtherHand = rightHandBone.position - leftHandBone.position;
            ball.position = rightHandBone.position - vecToOtherHand * 0.5f;
        }



    }
    IEnumerator SideStep()
    {
        sideStepped = false;
        float direction = 0f;
        animator.applyRootMotion = false;
        if (calculatedBallPos.x - goalkeeper.position.x > 0)
        {
            animator.SetBool("isLSS", true);
            direction = -1f;

        }
        else if (calculatedBallPos.x - goalkeeper.position.x < 0)
        {
            animator.SetBool("isRSS", true);
            direction = 1f;
        }

        while (direction * (calculatedBallPos.x - goalkeeper.position.x) < 0)
        {
            goalkeeper.position += new Vector3(-direction * sideStepSpeed * Time.deltaTime, 0, 0);
            //Debug.Log("goalkeepre " + goalkeeper.position + " ball pos" + calculatedBallPos);
            yield return null;
        }

        sideStepped = true;
        animator.applyRootMotion = false;
        SetIdleAnim();

    }

    IEnumerator SaveTheBall()
    {
        Debug.Log(" shottime" + shotTime);
        float a = 0f;
        if (shotTime > animTime)
        {
            Debug.Log("------bubuububub  " + shotTime + "    ----  " + animTime);
            yield return new WaitForSeconds(shotTime - animTime);
            a = 1f;
        }

        Debug.Log(calculatedBallPos.y + " - " + (gateArea.z) + "---farkı " + (calculatedBallPos.x - transform.position.x));
        if (CanSaveTheBall())                               // so keeper can save the ball
        {
            StartCoroutine(BallToHandHelper(othersAnimTime));
        }
        QuitSideStep();
        SetShotPos();
        if (isShotBM)
        {

            animator.applyRootMotion = false;
            goalkeeper.position = new Vector3(calculatedBallPos.x, goalkeeper.position.y, goalkeeper.position.z);
            animator.SetBool("isShotBM", true);
            ballMid = true;
        }
        else if (isShotMM)
        {
            animator.applyRootMotion = false;
            goalkeeper.position = new Vector3(calculatedBallPos.x, goalkeeper.position.y, goalkeeper.position.z);
            animator.SetBool("isShotMM", true);
            ballMid = true;
        }
        else if (isShotTM)
        {
            animator.applyRootMotion = false;
            goalkeeper.position = new Vector3(calculatedBallPos.x, goalkeeper.position.y, goalkeeper.position.z);
            animator.SetBool("isShotTM", true);
            ballMid = true;
        }
        else if (isShotLB)
        {
            animator.SetBool("isShotLB", true);
            ballLeft = true;
        }
        else if (isShotRB)
        {
            animator.SetBool("isShotRB", true);
            ballRight = true;
        }
        else if (isShotRT)
        {
            animator.SetBool("isShotRT", true);
            ballRight = true;
        }
        else if (isShotLT)
        {
            animator.SetBool("isShotLT", true);
            ballLeft = true;
        }
        else if (isShotLM)
        {
            animator.SetBool("isShotLM", true);
            ballLeft = true;
        }
        else if (isShotRM)
        {
            animator.SetBool("isShotRM", true);
            ballLeft = true;
        }

        yield return null;
    }


    private void SetShotPos() // which side of the goal, ball will be 
    {
        isShotBM = false;
        isShotMM = false;
        isShotTM = false;
        isShotRB = false;
        isShotRM = false;
        isShotRT = false;
        isShotLB = false;
        isShotLT = false;
        isShotLM = false;
        animTime = othersAnimTime;

        if (Mathf.Abs(calculatedBallPos.x - goalkeeper.position.x) < midTolerance)
        {

            if (calculatedBallPos.y < gateArea.z * 0.3f)
            {
                isShotBM = true;
            }
            else if (calculatedBallPos.y <= gateArea.z * 0.8f)
            {
                isShotMM = true;
            }
            else if (calculatedBallPos.y < gateArea.z)
            {
                isShotTM = true;
            }
        }
        else if (calculatedBallPos.x - goalkeeper.position.x > 0f)
        {
            if (calculatedBallPos.y <= gateArea.z * 0.3f)
            {
                isShotLB = true;
            }
            else if (calculatedBallPos.y < gateArea.z * 0.7f)
            {
                isShotLM = true;
            }
            else if (calculatedBallPos.y > gateArea.z)
            {
                //animTime = midAnimTime;
                isShotLT = true;
            }
        }
        else if (calculatedBallPos.x - goalkeeper.position.x < 0f)
        {
            if (calculatedBallPos.y <= gateArea.z * 0.3f)
            {
                isShotRB = true;
            }
            else if (calculatedBallPos.y < gateArea.z * 0.7f)
            {
                isShotRM = true;
            }
            else if (calculatedBallPos.y < gateArea.z)
            {
                //animTime = midAnimTime;
                isShotRT = true;
            }
        }
    }


    private void QuitSideStep()
    {
        if (!sideStepped)
        {
            StopCoroutine(sideStep); // stop side step 
            SetIdleAnim();
        }
    }

    public void TriggerHand()
    {
        if (ballRight)
        {
            rightConstraintScript.weight = 1f;
        }
        else if (ballLeft)
        {
            leftConstraintScript.weight = 1f;
        }
        else if (ballMid)
        {

        }
    }

    public void SetIdleAnim()
    {
        isShot = false;
        ballRight = false;
        ballLeft = false;
        ballMid = false;
        animator.applyRootMotion = true;
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
        goalkeeper.eulerAngles = new Vector3(0, 180, 0);

    }

    private bool CanSaveTheBall()
    {
        if (Mathf.Abs(calculatedBallPos.x - transform.position.x) < savebleDistance
                    && calculatedBallPos.y < gateArea.z)
        {
            if (calculatedBallPos.y > gateArea.z - 0.7f
                && Mathf.Abs(calculatedBallPos.x - transform.position.x) > midTolerance)
            {
                return false;
            }
            return true;
        }
        return false;
    }

    IEnumerator BallToHandHelper(float seconds)
    {
        if (seconds > 0)
            yield return new WaitForSeconds(seconds);
        BallToHand();
    }


    public void BallToHand()
    {
        Vector3 vecToOtherHand = rightHandBone.position - leftHandBone.position;
        ballRb.isKinematic = true;
        ball.position = rightHandBone.position - vecToOtherHand * 0.5f;
        isBallOnHand = true;
        //ball.parent = goalkeeper;
    }

    public void CalculateTheBallPosHelper(Vector3 force, bool isCurving, float radius, float torqueForce, float curveForce)
    {
        CalculateTheBallPos(ballRb, force, isCurving, radius, torqueForce, curveForce);
    }

    public Vector3 CalculateTheBallPos(Rigidbody sourceRigidbody, Vector3 force, bool isCurving, float radius, float torqueForce, float curveForce) // calculate the ball pos when ball is in the same z coordinate
    {
        isDriblingState = false;

        Physics.autoSimulation = false;
        //Get current Position
        Vector3 defaultPos = sourceRigidbody.position;
        float count = 0;

        // Debug.Log("Predicting Future Pos from::: x " + defaultPos.x + " y:"
        //     + defaultPos.y + " z:" + defaultPos.z);

        sourceRigidbody.AddForce(force, ForceMode.Impulse);

        sourceRigidbody.AddTorque(new Vector3(0f, torqueForce, 0f), ForceMode.Impulse);

        //Simulate where it will be in x seconds

        //Physics.Simulate(Time.fixedDeltaTime);
        Debug.Log(sourceRigidbody.angularVelocity + " --- " + sourceRigidbody.velocity);
        while (sourceRigidbody.position.z < gatePos.z - 0.5f && count < 1000)
        {

            if (isCurving)
            {
                var directionx = Vector3.Cross(sourceRigidbody.angularVelocity, sourceRigidbody.velocity);
                var magnitude = 4 / 3f * Mathf.PI * 0.1f * Mathf.Pow(radius, 3);
                sourceRigidbody.AddForce(magnitude * directionx * curveForce);

                //Debug.Log(sourceRigidbody.angularVelocity  +" --- "+ sourceRigidbody.velocity +" --- "+ magnitude * directionx);
            }
            Physics.Simulate(Time.fixedDeltaTime);
            count++;
        }
        Debug.Log("simulation time: " + count);
        //Get future position
        shotTime = count * Time.fixedDeltaTime;
        Vector3 futurePos = sourceRigidbody.position;

        // Debug.Log("DONE Predicting Future Pos::: x " + futurePos.x + " y:"
        //     + futurePos.y + " z:" + futurePos.z);

        //Re-enable Physics AutoSimulation and Reset position
        Physics.autoSimulation = true;
        sourceRigidbody.velocity = Vector3.zero;
        //sourceRigidbody.useGravity = true;
        sourceRigidbody.position = defaultPos;



        calculatedBallPos = futurePos;
        Debug.Log(calculatedBallPos);
        jumpDirection = new Vector3(noiseValue - 0.5f, 0, 0);

        isShot = true;

        sideStep = StartCoroutine(SideStep()); // start the side stepping before jump
        saveTheBall = StartCoroutine(SaveTheBall());

        return futurePos;
    }

    public void ResetGoalKeeper()
    {
        isBallOnHand = false;
        goalkeeper.position = new Vector3(gatePos.x, gatePos.y, gatePos.z);
        isDriblingState = true;
        ballRb.isKinematic = false;
        ball.parent = player.transform;
        SetIdleAnim();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ball" && isShot)
        {
            //SaveTheBall();
        }
    }

    public bool IsBallComingToGate() // checks if ball is coming to gate
    {
        if (calculatedBallPos.x > gateArea.x && calculatedBallPos.x < gateArea.y && calculatedBallPos.y < gateArea.z)
        {
            return true;
        }
        return false;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class DeffensePlayerScript : MonoBehaviour
{
    [SerializeField]
    public Transform ball;
    Rigidbody ballRb;

    [SerializeField]
    public Transform goal;

    [SerializeField]
    float tolerance = 1f;

    Vector3 vecToBall; // vector from ball to goal
    Vector3 midVecToBall; // middle position of vecToBall


    public float speed = 5f; // speed of deffense player

    public float tackleDistance = 0.2f; // enoughf distance to tackle
    public float minDistanceToGoal = 5f; // minimum distance between deffender and goal

    public float kickForce = 3f;

    public GameObject tackleObject;
    TwoBoneIKConstraint tackleConstraintScript;
    public Transform tackleTarget;

    bool isBallClose = false; //is ball close enough to go to it

    Animator animator;

    bool isTackled = false;

    Vector3 moveTo;

    public int deffType; // Type Of Deffence

    int deffenceNum;
    int forwardNum = 0;

    int animNum;
    int exAnimNum;

    bool mid = false;
    bool rightBack = false;
    bool rightForward = false;
    bool leftBack = false;
    bool leftForward = false;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        tackleConstraintScript = tackleObject.GetComponent<TwoBoneIKConstraint>();
        ballRb = ball.gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        vecToBall = goal.position - ball.position;

        var step = speed * Time.deltaTime; // calculate distance to move

        if (deffType == 0) // run to the ball
        {
            moveTo = new Vector3(ball.position.x, transform.position.y, ball.position.z);
            transform.position = Vector3.MoveTowards(transform.position, moveTo, step * 1.5f); // move toward ball
            SetAnimator(ball.position, tolerance, 2f);
        }
        else if (deffType == 1) // be in the middle of goal and ball
        {
            midVecToBall = goal.position - new Vector3(vecToBall.x * 0.8f, 0, vecToBall.z * 0.8f);
            moveTo = new Vector3(midVecToBall.x, transform.position.y, midVecToBall.z);
            transform.position = Vector3.MoveTowards(transform.position, moveTo, step);
            SetAnimator(midVecToBall, tolerance, 1f);
        }
        else if (deffType == 2) //stand front of a forward player
        {
            if (forwardNum == 0)
            {
                deffType = 3;
            }
        }
        else if (deffType == 3) // be in the middle of goal and ball as a second player
        {
            midVecToBall = goal.position - new Vector3(vecToBall.x * 0.6f, 0, vecToBall.z * 0.5f);
            moveTo = new Vector3(midVecToBall.x, transform.position.y, midVecToBall.z);
            transform.position = Vector3.MoveTowards(transform.position, moveTo, step * 1.5f);
            SetAnimator(midVecToBall, tolerance, 1f);
        }

        if (!isTackled)
            transform.LookAt(ball);


        if (Mathf.Abs(Vector3.Distance(midVecToBall, goal.position)) < minDistanceToGoal)
            isBallClose = true;



    }

    public void SetAnimator(Vector3 target, float tolerance, float state)
    {
        SetPosBools(target, tolerance, state);
        if (exAnimNum == animNum)
            return;

        switch (animNum)
        {
            case 0:
                SetAllAnimationsFalse();
                break;
            case 1:
                SetAllAnimationsFalse();
                animator.SetBool("rightBack", true);
                break;
            case 2:
                SetAllAnimationsFalse();
                animator.SetBool("rightForward", true);
                break;
            case 3:
                SetAllAnimationsFalse();
                animator.SetBool("leftBack", true);
                break;
            case 4:
                SetAllAnimationsFalse();
                animator.SetBool("leftForward", true);
                break;
            case 5:
                SetAllAnimationsFalse();
                animator.SetBool("run", true);
                break;

        }
        exAnimNum = animNum;
    }

    private void SetPosBools(Vector3 target, float tolerance, float state)
    {

        if (state == 2)
        {
            animNum = 5;
            return;
        }

        if (Mathf.Abs(transform.position.x - target.x) < tolerance && Mathf.Abs(transform.position.z - target.z) < tolerance)
        {
            animNum = 0;
            return;
        }
        else if (transform.position.x - target.x > 0 && transform.position.z - target.z < 0)
        {
            animNum = 1;
            return;
        }
        else if (transform.position.x - target.x > 0 && transform.position.z - target.z > 0)
        {
            animNum = 2;
            return;
        }
        else if (transform.position.x - target.x < 0 && transform.position.z - target.z < 0)
        {
            animNum = 3;
            return;
        }
        else if (transform.position.x - target.x < 0 && transform.position.z - target.z > 0)
        {
            animNum = 4;
            return;
        }
        animNum = -1;
    }

    private void SetAllAnimationsFalse()
    {
        animator.SetBool("leftBack", false);
        animator.SetBool("rightBack", false);
        animator.SetBool("rightForward", false);
        animator.SetBool("leftForward", false);
        animator.SetBool("run", false);
        animator.SetBool("tackle", false);
        tackleConstraintScript.weight = 0f;
    }
    public void Tackle()
    {
        SetAllAnimationsFalse();

        isTackled = true;
        //tackleConstraintScript.weight = 1f;
        animator.SetBool("tackle", true);
    }
    public void AfterTackle()
    {
        float shootAngle = transform.eulerAngles.y / 180 * Mathf.PI;

        Vector3 calculatedForce = new Vector3(-kickForce * Mathf.Cos(shootAngle), kickForce,
                                                -kickForce * Mathf.Cos(Mathf.PI / 2 - shootAngle));
        ballRb.AddForce(calculatedForce);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ball")
        {
            isBallClose = true;
            deffType = 0;
        }
    }

    public void SetActiveOpposite()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }


}

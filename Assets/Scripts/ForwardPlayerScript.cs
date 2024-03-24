using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardPlayerScript : MonoBehaviour
{

    public Transform deff1; // deffence player1
    public Transform deff2; // deffence player2
    public Transform goal;

    Vector3 midDeff; // mid point of deffence players 

    public float speed;
    public float shootPower;

    public GameObject ball;
    Rigidbody ballRb;

    [SerializeField]
    Transform rightFoot; // transform of right foot

    Animator animator;

    bool isKicked = false;

    // Start is called before the first frame update
    void Start()
    {
        ballRb = ball.GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //midDeff = deff1.transform.position - (deff1.transform.position - deff2.transform.position) * 0.5f;
        //transform.position =  Vector3.MoveTowards(transform.position, midDeff, speed * Time.deltaTime);
        transform.LookAt(goal);
    }

    void Shoot()
    {
        Vector3 shootVec = shootPower * (goal.position - ball.transform.position);
        ballRb.isKinematic = false;
        ballRb.AddForce(shootVec);
        animator.SetBool("shoot", false);
    }

    void RecieveBall()
    {   
        ballRb.isKinematic = true;
        ball.transform.position = rightFoot.position;
        animator.SetBool("shoot", true);
    }
    
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Ball" && !isKicked)
        {
            RecieveBall();
            isKicked = true;
        }
    }
}

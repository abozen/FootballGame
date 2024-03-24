using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScript : MonoBehaviour
{
    [SerializeField]
    GameObject ball;

    BallController ballController;
    // Start is called before the first frame update
    void Start()
    {
        ballController = ball.GetComponent<BallController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Ball")
        {
            Debug.Log(other.gameObject.transform.position);
        }
    }
}

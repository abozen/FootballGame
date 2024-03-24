using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandScript : MonoBehaviour
{
    public GameObject goalkeeper;
    GoalkeeperScript goalkeeperScript;
    // Start is called before the first frame update
    void Start()
    {
        goalkeeperScript = goalkeeper.GetComponent<GoalkeeperScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ball")
        {
            //goalkeeperScript.BallToHand();
        }
    }
}

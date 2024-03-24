using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTriggerScript : MonoBehaviour
{
    GameObject goalkeeper;
    GoalkeeperScript goalkeeperScript;
    // Start is called before the first frame update
    void Start()
    {
        goalkeeper = transform.parent.gameObject;
        goalkeeperScript = goalkeeper.GetComponent<GoalkeeperScript>();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Ball")
        {
            goalkeeperScript.TriggerHand();
        }
    }
}

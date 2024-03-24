using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField]
    GameObject player;

    BallController ballController;

    float randomRotation;

    public int forwardNum;
    public int deffenceNum;

    public GameObject deffencePref;
    public GameObject forwardPref;

    public GameObject[] deffPlayers;
    public GameObject[] forwPlayers;


    public Transform ball;
    public Transform goal;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        ballController = player.transform.Find("Soccer Ball").GetComponent<BallController>();
        SetLevel();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("t"))
        {
            
            NewRandomPosition();
        }
    }

    public void SetLevel()
    {
        NewRandomPosition();
        CreatePlayers();
        SetDeffencePlayers();
        SetForwardPlayers();
    }

    private void CreatePlayers()
    {
        forwardNum = Random.Range(0, 3);
        deffenceNum = Random.Range(forwardNum, forwardNum + 2);
        
        deffPlayers = new GameObject[deffenceNum];
        forwPlayers = new GameObject[forwardNum];
        for (int i = 0; i < deffenceNum; i++)
        {
            Vector3 randomPos = new Vector3(Random.Range(-30, 30), 0, Random.Range(10, 50));
            deffPlayers[i] =  Instantiate(deffencePref, randomPos, Quaternion.identity);
        }
        for (int i = 0; i < forwardNum; i++)
        {
            Vector3 randomPos = new Vector3(Random.Range(-30, 30), 0, Random.Range(10, 50));
            forwPlayers[i] =  Instantiate(forwardPref, randomPos, Quaternion.identity);
            
        }
    }

    private void SetDeffencePlayers()
    {
        int closest = 0; // the one who is closest to the ball
        float minDistance = Vector3.Distance(deffPlayers[0].transform.position, player.transform.position);
        for (int i = 1; i < deffenceNum; i++)
        {
            float currenntDistance = Vector3.Distance(deffPlayers[i].transform.position, player.transform.position);
            if(currenntDistance < minDistance)
            {
                closest = i;
                minDistance = currenntDistance;
            }
        }

        for (int i = 0; i < deffenceNum; i++)
        {
            DeffensePlayerScript currentScript = deffPlayers[i].GetComponent<DeffensePlayerScript>();
            if(i == closest)
            {
                currentScript.deffType = 1;
            }else
            {
                currentScript.deffType = 2;
            }
            currentScript.ball = ball;
            currentScript.goal = goal;
        }
    }

    private void SetForwardPlayers()
    {
        for (int i = 0; i < forwardNum; i++)
        {
            ForwardPlayerScript currentScript = forwPlayers[i].GetComponent<ForwardPlayerScript>();

            currentScript.deff1 = deffPlayers[i].transform;
            currentScript.deff2 = deffPlayers[i + 1].transform;
            currentScript.ball = ball.gameObject;
            currentScript.goal = goal;
        }
    }
    

    public void NewRandomPosition()
    {
        if(ballController.isShot)
            {
                ballController.ResetBall();
            }
            randomRotation = Random.Range(-60, 60);
            ballController.firstCamRot = new Vector3(ballController.firstCamRot.x, randomRotation, ballController.firstCamRot.z);
            
            player.transform.position = new Vector3(Random.Range(-30, 30), 0, Random.Range(10, 35));
            player.transform.eulerAngles = new Vector3(0, randomRotation, 0);
    }
}

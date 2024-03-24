using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BallDribbling ballDribbling;
    [SerializeField] private BallShooting ballShooting;



    public void SetBallState(int ballState)
    {
        // 1: shooting  2: dribbling
        if(ballState == 1)
        {
            ballDribbling.isDriblingState = false;
            ballShooting.isShootingState = true;
        }else if(ballState == 2)    
        {
            ballDribbling.isDriblingState = true;
            ballShooting.isShootingState = false;
        }
    }
}

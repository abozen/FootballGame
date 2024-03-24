using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Savability : MonoBehaviour
{
    private float savableDistanceX = 4;
    private float savableDistanceX2 = 0.5f;
    private float savableDistanceY = 3.1f;

    private float gateWidth = 13f;
    private float gateHeight = 4.3f;

    [SerializeField] private Transform goalkeeper;
    [SerializeField] private int goalkeeperRating;

    private bool canSave = true;


    public int GetSavablityState(Vector3 futureBallPos)
    {
        IsSavable(futureBallPos);
        Debug.Log("savability2: " + canSave);

        if (!canSave && IsBallComingToGate(futureBallPos))
        {
            return 1; // score
        }
        else if (canSave && IsBallComingToGate(futureBallPos))
        {
            return 2; // save the ball
        }
        return 3; // ball is out

    }

    public bool IsBallComingToGate(Vector3 futureBallPos)
    {
        if (futureBallPos.x >= -gateWidth * 0.5f && futureBallPos.x <= gateWidth * 0.5f
            && futureBallPos.y <= gateHeight)
        {
            return true;
        }
        return false;
    }
    public bool IsSavable(Vector3 futureBallPos)
    {
        Debug.Log("Future ball position: " + futureBallPos);
        Debug.Log("Goalkeeper position: "+ goalkeeper.position);
        if ((futureBallPos.x < goalkeeper.position.x + savableDistanceX && futureBallPos.x > goalkeeper.position.x - savableDistanceX
            && futureBallPos.y < savableDistanceY)
            || (futureBallPos.x < goalkeeper.position.x + savableDistanceX2 && futureBallPos.x > goalkeeper.position.x - savableDistanceX2
            && futureBallPos.y < gateHeight))
        {
            canSave = true;
            return true;
        }
        canSave = false;
        Debug.Log(canSave);
        return false;
    }

    public int UpdateGatePart(int gatePart)
    {
        int newGatePart = gatePart;
        if (!IsSavableRating() && gatePart % 3 != 2) // not coming to mid
        {
            canSave = false;
            Debug.Log("savability1: " + canSave);
            if (gatePart == 1 || gatePart == 3)
            {
                newGatePart = gatePart + 3;
            }
            else if (gatePart == 4 || gatePart == 6)
            {
                if (Random.Range(0, 2) == 1)
                {
                    newGatePart = gatePart + 3;
                }
                else
                {
                    newGatePart = gatePart - 3;
                }
            }
            else if (gatePart == 7 || gatePart == 9)
            {
                newGatePart = gatePart - 3;
            }
        }else
            canSave = true;
        return newGatePart;
    }

    public bool IsSavableRating()
    {
        if (Random.Range(0, 100) <= goalkeeperRating)
            return true;
        return false;
    }


}

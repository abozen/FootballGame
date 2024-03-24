using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class Test : MonoBehaviour
{
    GK_Dive gk_Dive;
    [SerializeField] GameObject goalkeeper;
    [SerializeField] Transform gate;
    [SerializeField] Rigidbody ballRb;
    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        gk_Dive = goalkeeper.GetComponent<GK_Dive>();
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetMouseButtonDown(0))
        // {
        //     Vector3 mousePos = Input.mousePosition;
        //     transform.position = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, transform.position.z - Camera.main.transform.position.z));
        //     gk_Dive.SetFutureBallPos(transform.position);
        //     gk_Dive.Dive();
        //     //transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, transform.position.z);
        // }
    }
    public void CalculateTheBallPosHelper(Vector3 force, bool isCurving, float radius, float torqueForce, float curveForce)
    {
        CalculateTheBallPos(ballRb, force, isCurving, radius, torqueForce, curveForce);
    }
    public Vector3 CalculateTheBallPos(Rigidbody sourceRigidbody, Vector3 force, bool isCurving, float radius, float torqueForce, float curveForce) // calculate the ball pos when ball is in the same z coordinate
    {
        Physics.autoSimulation = false;

        float simulationTime = 0;

        //Get current Position
        Vector3 defaultPos = sourceRigidbody.position;

        sourceRigidbody.AddForce(force, ForceMode.Impulse);

        sourceRigidbody.AddTorque(new Vector3(0f, torqueForce, 0f), ForceMode.Impulse);

        //Simulate where it will be in x seconds

        while (sourceRigidbody.position.z < gate.position.z)
        {
            if (isCurving)
            {
                var directionx = Vector3.Cross(sourceRigidbody.angularVelocity, sourceRigidbody.velocity);
                var magnitude = 4 / 3f * Mathf.PI * 0.1f * Mathf.Pow(radius, 3);
                sourceRigidbody.AddForce(magnitude * directionx * curveForce);

            }
            Physics.Simulate(Time.fixedDeltaTime);
            simulationTime += Time.fixedDeltaTime;
        }

        Vector3 futurePos = sourceRigidbody.position;

        //Re-enable Physics AutoSimulation and Reset position
        sourceRigidbody.velocity = Vector3.zero;
        //sourceRigidbody.useGravity = true;
        sourceRigidbody.position = defaultPos;
        
        Physics.autoSimulation = true;

        //transform.position = futurePos;
        gk_Dive.SetFutureBallPos(futurePos);
        gk_Dive.Dive(simulationTime);
        Debug.Log("simulation time: " + simulationTime);
        return futurePos;
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

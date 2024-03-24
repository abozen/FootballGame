using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class BallController : MonoBehaviour
{

    private Rigidbody rb;

    private Vector2 startPos, endPos, direction, mPos;

    private float touchTimeStart, touchTimeFinish, timeInterval, timeM;

    [SerializeField]
    float throwForceX = 1f;
    [SerializeField]
    float throwForceY = 0.2f;
    [SerializeField]
    float throwForceZ = 600f;

    [SerializeField]
    float torqueForce = 3f;
    [SerializeField]
    float radius = 0.25f;

    [SerializeField]
    float minBallSpeed = 600;

    [SerializeField]
    float maxBallSpeed = 2000f;

    [SerializeField]
    float curveTolerance = 3f;

    [SerializeField]
    float dribleForceX = 0.003f;

    [SerializeField]
    Transform goal;

    [SerializeField]
    Transform mainCamera;

    [SerializeField]
    GameObject camera;

    CameraController cameraController;

    private Vector3 firstBallPos, calculatedForce;
    public Vector3 firstCamRot;

    private float forceX, forceY, forceZ, shootAngle;

    private bool isShooting = false;
    public bool isShot = false;
    private bool isDriblingState;
    private bool isShootingState = false;


    public float maxViewAngle = 60f;

    private Coroutine camRotate;
    private Coroutine curveCo;

    public float maxDirblingSpeed = 5f;

    public GameObject goalkeeper;
    private GoalkeeperScript goalkeeperScript;

    public Rigidbody futureBallRb;

    float maxXShot; // max and min values of X when shooting
    float minXShot;
    float diffXShot; //diffrence of them



    bool isCurving;

    public float curveForce;
    public float minCurve;
    public float maxCurve;
    public float maxVecMagCurve; // maximum vector magnitude for curve

    [SerializeField] Transform shootPoint;
    Vector3 shootPointScale;



    // Start is called before the first frame update
    void Start()
    {
        cameraController = camera.GetComponent<CameraController>();
        rb = GetComponent<Rigidbody>();
        firstBallPos = transform.position;

        firstCamRot = mainCamera.rotation.eulerAngles;
        isDriblingState = true;


        goalkeeperScript = goalkeeper.GetComponent<GoalkeeperScript>();

        shootPointScale = shootPoint.localScale;
    }

    private void Update()
    {
        DriblingState();
        //LookingAroundState();
        //ShootingState();

        //reset the pos of the ball
        if (Input.GetKeyDown("r"))
        {
            ResetBall();
        }



    }

    void LookingAroundState()
    {
        if (isShootingState && Input.GetButtonDown("Fire1") && !EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.mousePosition.y > Screen.height * 0.7f)
            {
                isShooting = false;

                startPos = Input.mousePosition;

            }

        }
        else if (isShootingState && Input.GetButton("Fire1") && !EventSystem.current.IsPointerOverGameObject())
        {
            if (!isShooting && ((Input.mousePosition.x - startPos.x) / Screen.width) * 180 < maxViewAngle
                            && ((Input.mousePosition.x - startPos.x) / Screen.width) * 180 > -maxViewAngle
                            && Input.mousePosition.y > Screen.height * 0.7f)
            {
                mainCamera.eulerAngles = new Vector3(mainCamera.rotation.eulerAngles.x,
                                                             firstCamRot.y + ((Input.mousePosition.x - startPos.x) / Screen.width) * 180,
                                                             mainCamera.rotation.eulerAngles.z);
            }
        }

        if (Input.GetButtonUp("Fire1") && !EventSystem.current.IsPointerOverGameObject())
        {
            if (!isShooting && isShootingState)
            {
                if (camRotate != null)
                    StopCoroutine(camRotate);
                camRotate = StartCoroutine(SetCamToDefaultY(Mathf.Sign(startPos.x - Input.mousePosition.x)));
            }
        }
    }

    void ShootingState()
    {

        if (Input.GetButtonDown("Fire1") && !EventSystem.current.IsPointerOverGameObject())
        {
            if (isShootingState && Input.mousePosition.y <= Screen.height * 0.7f)
            {
                isShooting = true;
                firstBallPos = transform.position;
                touchTimeStart = Time.time;
                startPos = Input.mousePosition;

                maxXShot = startPos.x;
                minXShot = startPos.x;

            }
        }
        else if (Input.GetButton("Fire1") && isShooting)
        {

            if (Input.mousePosition.x > maxXShot)
            {
                maxXShot = Input.mousePosition.x;
            }
            else if (Input.mousePosition.x < minXShot)
            {
                minXShot = Input.mousePosition.x;
            }

        }
        else if (Input.GetButtonUp("Fire1") && isShooting)
        {

            if (isShooting)
            {
                touchTimeFinish = Time.time;
                timeInterval = touchTimeFinish - touchTimeStart;
                endPos = Input.mousePosition;
                direction = endPos - startPos;
                rb.isKinematic = false;

                forceX = direction.x * throwForceX;
                forceY = direction.y * throwForceY;
                forceZ = throwForceZ / timeInterval;

                forceZ = forceZ > maxBallSpeed ? maxBallSpeed : forceZ;
                forceZ = forceZ < minBallSpeed ? minBallSpeed : forceZ;

                shootAngle = mainCamera.transform.eulerAngles.y / 180 * Mathf.PI;
                isShooting = false;
                isShot = true;

                calculatedForce = new Vector3(forceZ * Mathf.Cos(Mathf.PI / 2 - shootAngle) + forceX * Mathf.Cos(shootAngle), forceY,
                                                -forceX * Mathf.Cos(Mathf.PI / 2 - shootAngle) + forceZ * Mathf.Cos(shootAngle));

                //goalkeeperScript.CalculateTheBallPosHelper(calculatedForce);
                rb.AddForce(calculatedForce);


                diffXShot = maxXShot - minXShot;



                cameraController.SetCamState(0);

            }
        }

    }


    void DriblingState()
    {
        if (isDriblingState && Input.GetButtonDown("Fire1") && Input.mousePosition.y <= Screen.height / 2 && !EventSystem.current.IsPointerOverGameObject())
        {
            touchTimeStart = Time.time;
            startPos = Input.mousePosition;
        }
        else if (isDriblingState && Input.GetButtonUp("Fire1") && !EventSystem.current.IsPointerOverGameObject())
        {
            touchTimeFinish = Time.time;
            timeInterval = touchTimeFinish - touchTimeStart;
            endPos = Input.mousePosition;
            direction = endPos - startPos;
            rb.isKinematic = false;
            forceX = direction.x * dribleForceX;
            forceY = 0;
            forceZ = direction.y * dribleForceX;
            shootAngle = mainCamera.transform.eulerAngles.y / 180 * Mathf.PI;



            calculatedForce = new Vector3(forceZ * Mathf.Cos(Mathf.PI / 2 - shootAngle) + forceX * Mathf.Cos(shootAngle),
                                            forceY,
                                            -forceX * Mathf.Cos(Mathf.PI / 2 - shootAngle) + forceZ * Mathf.Cos(shootAngle));



            // if(((direction.x > 0 && rb.velocity.x < maxDirblingSpeed) || (direction.x < 0 && rb.velocity.x > -maxDirblingSpeed))
            //     && ((direction.y > 0 && rb.velocity.z < maxDirblingSpeed) || (direction.y < 0 && rb.velocity.z > -maxDirblingSpeed)))
            rb.AddForce(calculatedForce);

            //cameraController.SetDriblingVector(calculatedForce);


        }

    }

    public void ResetBall()
    {
        rb.isKinematic = true;
        transform.position = firstBallPos;
        rb.velocity = Vector3.zero;
        isShot = false;
        cameraController.SetCamState(0);
        StopCoroutine(curveCo);
        isCurving = false;
    }

    public void SetStateToDribble()
    {
        isDriblingState = true;
        isShootingState = false;

    }
    public void SetStateToShooting()
    {
        isDriblingState = false;
        isShootingState = true;

        Time.timeScale = 0.2f;

        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        firstCamRot = mainCamera.eulerAngles;
    }

    public void SetFirstBallPos()
    {
        firstBallPos = transform.position;
    }

    IEnumerator SetCamToDefaultY(float direction)
    {
        //extra value to solve 0-360 problem of euler angles
        float addNum;
        if (direction == -1)
        {
            addNum = mainCamera.rotation.eulerAngles.y < firstCamRot.y ? 360 : 0;
        }
        else
        {
            addNum = mainCamera.rotation.eulerAngles.y > firstCamRot.y ? -360 : 0;
        }

        while ((mainCamera.rotation.eulerAngles.y + addNum) * direction < firstCamRot.y * direction)
        {
            //Rotate to default position
            mainCamera.eulerAngles = new Vector3(mainCamera.rotation.eulerAngles.x,
                                                mainCamera.rotation.eulerAngles.y + direction * 0.1f,
                                                mainCamera.rotation.eulerAngles.z);


            //reset addNum in case 
            if (direction == -1 && mainCamera.rotation.eulerAngles.y > firstCamRot.y)
            {
                addNum = 0;
            }
            else if (direction == 1 && mainCamera.rotation.eulerAngles.y < firstCamRot.y)
            {
                addNum = 0;
            }
            yield return null;
        }

        mainCamera.eulerAngles = firstCamRot;

    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnMouseDown()
    {
        if (isShootingState)
        {
            isShooting = true;
            firstBallPos = transform.position;
            touchTimeStart = Time.realtimeSinceStartup;
            startPos = Input.mousePosition;
            //shootPoint.position = startPos;
            //shootPoint.gameObject.SetActive(true);
            //StartCoroutine(SetShootPoint());
        }
    }

    private void OnMouseUp()
    {
        if (isShooting)
        {
            Time.timeScale = 1f;
            Vector3 ballScreenPos = Camera.main.WorldToScreenPoint(transform.position);
            touchTimeFinish = Time.realtimeSinceStartup;
            timeInterval = touchTimeFinish - touchTimeStart;
            endPos = Input.mousePosition;
            isCurving = false;
            direction = new Vector2(ballScreenPos.x - startPos.x, ballScreenPos.y - startPos.y);
            Debug.Log("dist: " + Vector3.Distance(endPos, startPos));
            curveForce = Mathf.Clamp(Vector3.Distance(endPos, startPos) * maxCurve / maxVecMagCurve, minCurve, maxCurve);
            if (Vector3.Distance(endPos, startPos) > curveTolerance && endPos.y >= startPos.y)
            {
                torqueForce = endPos.x > startPos.x ? -10f : 10f;
                isCurving = true;
            }
            else
                torqueForce = 0f;

            rb.isKinematic = false;

            forceX = direction.x * throwForceX * timeInterval;
            forceY = direction.y * throwForceY * timeInterval;
            forceZ = throwForceZ * timeInterval;

            forceZ = forceZ > maxBallSpeed ? maxBallSpeed : forceZ;
            forceZ = forceZ < minBallSpeed ? minBallSpeed : forceZ;

            shootAngle = mainCamera.transform.eulerAngles.y / 180 * Mathf.PI;
            isShooting = false;
            isShot = true;

            shootPoint.gameObject.SetActive(false);
            shootPoint.localScale = shootPointScale;
            shootPoint.eulerAngles = Vector3.zero;

            calculatedForce = new Vector3(forceZ * Mathf.Cos(Mathf.PI / 2 - shootAngle) + forceX * Mathf.Cos(shootAngle), forceY,
                                            -forceX * Mathf.Cos(Mathf.PI / 2 - shootAngle) + forceZ * Mathf.Cos(shootAngle));

            Debug.Log(timeInterval);
            goalkeeperScript.CalculateTheBallPosHelper(calculatedForce, isCurving, radius, torqueForce, curveForce);

            rb.AddForce(calculatedForce, ForceMode.Impulse);
            rb.AddTorque(new Vector3(0f, torqueForce, 0f), ForceMode.Impulse);
            Debug.Log(rb.angularVelocity + " B " + rb.velocity);
            curveCo = StartCoroutine(Curve());


            diffXShot = maxXShot - minXShot;

            cameraController.SetCamState(0);
        }

    }
    IEnumerator Curve()
    {
        float time = 0f;
        int count = 0;
        while (time < 3f)
        {
            var direction = Vector3.Cross(rb.angularVelocity, rb.velocity);
            var magnitude = 4 / 3f * Mathf.PI * 0.1f * Mathf.Pow(radius, 3);
            rb.AddForce(magnitude * direction * curveForce);
            //rb.AddTorque(transform.up * torqueForce);

            time += Time.fixedDeltaTime;
            count++;
            yield return new WaitForSeconds(Time.fixedDeltaTime - Time.deltaTime);
        }
        Debug.Log("curve: " + count);
    }


    IEnumerator SetShootPoint()
    {
        float timeInterval = 0;
        StartCoroutine(SetShootPointAngle());
        float startTime = Time.realtimeSinceStartup;
        while (isShooting && throwForceZ * timeInterval < maxBallSpeed)
        {
            timeInterval = Time.realtimeSinceStartup - startTime;


            shootPoint.localScale += Vector3.one * Time.deltaTime / Time.timeScale;

            yield return null;
        }
    }

    IEnumerator SetShootPointAngle()
    {
        Vector2 rotateTo;

        float y = 0;
        float x = 0;
        float xDirection = 0;
        float yDirection = 0;

        bool once1 = true;
        bool once2 = true;
        while (isShooting)
        {
            rotateTo = new Vector2(Input.mousePosition.x - startPos.x, Input.mousePosition.y - startPos.y);

            if (once1 && Input.mousePosition.x - startPos.x != 0)
            {
                xDirection = Mathf.Sign(Input.mousePosition.x - startPos.x);
                once1 = false;
            }
            if (once2 && Input.mousePosition.y - startPos.y != 0)
            {
                yDirection = Mathf.Sign(Input.mousePosition.y - startPos.y);
                once2 = false;
            }

            y = yDirection < 0 ? Mathf.Max(rotateTo.y, y) : Mathf.Min(rotateTo.y, y);
            x = xDirection < 0 ? Mathf.Max(rotateTo.x, x) : Mathf.Min(rotateTo.x, x);

            shootPoint.eulerAngles = new Vector3(-rotateTo.y * 0.1f, rotateTo.x, 0) * 0.1f;

            yield return null;
        }
    }
}
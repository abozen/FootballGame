using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShooting : MonoBehaviour
{
    public bool isShootingState = true;

    private bool isShooting;
    private bool isShot;
    private Vector3 firstBallPos;
    private float touchTimeStart;
    private Vector2 startPos;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float curveTolerance;
    [SerializeField] private float maxCurve;
    [SerializeField] private float minCurve;

    [SerializeField] private float maxVecMagCurve;
    [SerializeField] private float throwForceX;
    [SerializeField] private float throwForceY;
    [SerializeField] private float throwForceZ;
    [SerializeField] private float maxBallSpeed;
    [SerializeField] private float minBallSpeed;
    private Vector3 shootPointScale;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float curveForce;
    private Coroutine curveCo;
    private float radius = 0.25f;
    [SerializeField] private CameraController cameraController;


    [SerializeField] GameObject Test;
    Vector3 firstPos;


    // Start is called before the first frame update
    void Start()
    {
        firstPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnMouseDown()
    {
        if (isShootingState)
        {
            isShooting = true;
            firstBallPos = transform.position;
            touchTimeStart = Time.realtimeSinceStartup;
            startPos = Input.mousePosition;
            shootPoint.position = startPos;
            shootPoint.gameObject.SetActive(true);
            //StartCoroutine(SetShootPoint());
        }
    }

    private void OnMouseUp()
    {
        if (isShooting)
        {
            Time.timeScale = 1f;
            Vector3 ballScreenPos = Camera.main.WorldToScreenPoint(transform.position);
            float touchTimeFinish = Time.realtimeSinceStartup;
            float timeInterval = touchTimeFinish - touchTimeStart;
            Vector2 endPos = Input.mousePosition;
            bool isCurving = false;
            Vector2 direction = new Vector2(ballScreenPos.x - startPos.x, ballScreenPos.y - startPos.y);

            curveForce = Mathf.Clamp(Vector3.Distance(endPos, startPos) * maxCurve / maxVecMagCurve, minCurve, maxCurve);
            float torqueForce = endPos.x > startPos.x ? -10f : 10f;
            if (Vector3.Distance(endPos, startPos) > curveTolerance && endPos.y >= startPos.y)
            {
                isCurving = true;
            }
            else
                torqueForce = 0f;

            rb.isKinematic = false;

            float forceX = direction.x * throwForceX * timeInterval;
            float forceY = direction.y * throwForceY * timeInterval;
            float forceZ = throwForceZ * timeInterval;

            forceZ = forceZ > maxBallSpeed ? maxBallSpeed : forceZ;
            forceZ = forceZ < minBallSpeed ? minBallSpeed : forceZ;

            float shootAngle = mainCamera.transform.eulerAngles.y / 180 * Mathf.PI;
            isShooting = false;
            isShot = true;

            // shootPoint.gameObject.SetActive(false);
            // shootPoint.localScale = shootPointScale;
            // shootPoint.eulerAngles = Vector3.zero;

            Vector3 calculatedForce = new Vector3(forceZ * Mathf.Cos(Mathf.PI / 2 - shootAngle) + forceX * Mathf.Cos(shootAngle), forceY,
                                            -forceX * Mathf.Cos(Mathf.PI / 2 - shootAngle) + forceZ * Mathf.Cos(shootAngle));

            Test.GetComponent<Test>().CalculateTheBallPosHelper(calculatedForce, isCurving, radius, torqueForce, curveForce);

            rb.AddForce(calculatedForce, ForceMode.Impulse);
            rb.AddTorque(new Vector3(0f, torqueForce, 0f), ForceMode.Impulse);

            curveCo = StartCoroutine(Curve());

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
    }
    
    public void ResetBall()
    {
        transform.position = firstPos;
    }
}

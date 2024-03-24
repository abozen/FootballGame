using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BallDribbling : MonoBehaviour
{
    public bool isDriblingState = true;

    private float touchTimeStart;
    private Vector2 startPos;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private float dribleForceX;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isDriblingState)
            DriblingState();
    }

    void DriblingState()
    {
        if (Input.GetButtonDown("Fire1") && Input.mousePosition.y <= Screen.height / 2 && !EventSystem.current.IsPointerOverGameObject())
        {
            touchTimeStart = Time.time;
            startPos = Input.mousePosition;
        }
        else if (Input.GetButtonUp("Fire1") && !EventSystem.current.IsPointerOverGameObject())
        {
            float touchTimeFinish = Time.time;
            float timeInterval = touchTimeFinish - touchTimeStart;
            Vector2 endPos = Input.mousePosition;
            Vector2 direction = endPos - startPos;
            rb.isKinematic = false;
            float forceX = direction.x * dribleForceX;
            float forceY = 0;
            float forceZ = direction.y * dribleForceX;
            float shootAngle = mainCamera.transform.eulerAngles.y / 180 * Mathf.PI;

            Vector3 calculatedForce = new Vector3(forceZ * Mathf.Cos(Mathf.PI / 2 - shootAngle) + forceX * Mathf.Cos(shootAngle),
                                            forceY,
                                            -forceX * Mathf.Cos(Mathf.PI / 2 - shootAngle) + forceZ * Mathf.Cos(shootAngle));


            rb.AddForce(calculatedForce);
        }

    }
}

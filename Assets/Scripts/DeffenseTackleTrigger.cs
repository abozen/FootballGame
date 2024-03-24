using UnityEngine;

public class DeffenseTackleTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Ball")
        {
            transform.parent.gameObject.GetComponent<DeffensePlayerScript>().Tackle();
        }
    }
}

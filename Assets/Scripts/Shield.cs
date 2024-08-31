using UnityEngine;

public class Shield : MonoBehaviour
{
    public float pushForce;

    public void Activate(float activationTime)
    {
        gameObject.SetActive(true);
        Invoke("Deactivate", activationTime);
    }

    void Deactivate()
    {
        gameObject.SetActive(false);

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Enemy>() != null)
        {
            Debug.Log("Pushing away enemy!!");
            other.GetComponent<Enemy>().Push(-other.transform.forward * pushForce);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("SHIELD COLLIDING WITH " + collision.gameObject.name);
    }
}

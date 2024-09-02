using UnityEngine;

public class Shield : MonoBehaviour
{
    public float pushForce;

    public void Activate(float activationTime)
    {
        gameObject.SetActive(true);
        Invoke("Deactivate", activationTime);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Enemy>() != null)
        {
            other.GetComponent<Enemy>().Push(-other.transform.forward * pushForce);
        }
    }

}

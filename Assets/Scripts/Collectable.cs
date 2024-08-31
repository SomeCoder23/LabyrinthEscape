using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    //can later make pill class inherit from this
    public float rotateSpeed;
    public Vector3 rotateDirection;
    public AudioClip collectSound;

    int direction = 1;

    private void FixedUpdate()
    {
        transform.Rotate(rotateDirection, rotateSpeed * direction * Time.fixedDeltaTime);
    }

    public void Collect()
    {
        SoundManager.instance.PlaySound(collectSound);
        ScoreManager.instance.AddPoint();
        Destroy(this.gameObject);
    }
}

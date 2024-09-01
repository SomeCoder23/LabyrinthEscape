using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public Transform target;
    [SerializeField]
    float height;

    void Update()
    {
        if (target == null)
            return;

        transform.position = new Vector3(target.position.x, height, target.position.z);
    }

    public float getHeight()
    {
        return height;
    }

    public void setHeight(float newHeight)
    {
        height = newHeight;
    }
}

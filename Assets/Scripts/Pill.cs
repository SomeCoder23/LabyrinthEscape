using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pill : MonoBehaviour
{
    [SerializeField]
    PillType type;

    public AudioClip collectSound;
    [SerializeField]
    float rotateSpeed;
    [SerializeField]
    float activeTime = 3;

    private void FixedUpdate()
    {
        transform.Rotate(Vector3.one, rotateSpeed * Time.fixedDeltaTime);
    }

    public float getActiveTime()
    {
        return activeTime;
    }

    public void Use()
    {
        switch (type)
        {
            case PillType.Grow: Debug.Log("Took GROW pill!");
                ScaleController.instance.Grow(activeTime);
                break;

            case PillType.Shrink: Debug.Log("Took SHRINK pill!");
                ScaleController.instance.Shrink(activeTime); 
                break;

            case PillType.Random: Debug.Log("Took RANDOM pill!");
                int action = Random.Range(0, 2);
                if (action == 0) type = PillType.Grow;
                else type = PillType.Shrink;
                Use(); return;

            case PillType.Invisible: Debug.Log("Took INVISIBLE pill!"); break;

            case PillType.Shield: Debug.Log("Took SHIELD pill!");
                ThirdPersonController player = FindObjectOfType<ThirdPersonController>();
                if (player != null)
                    player.ActivateShield(activeTime);
                else Debug.Log("SHIELD NULL");
                break;
        }

        SoundManager.instance.PlaySound(collectSound);
    }

}

public enum PillType
{
    Grow,
    Shrink,
    Random,
    Invisible,
    Shield
}

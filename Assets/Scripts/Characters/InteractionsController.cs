using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionsController : MonoBehaviour
{
    public Shield shield;
    public float pushForce;
    public AudioClip outOfBoundsAudio;

    bool onDrugs = false, shieldOn = false;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Portal"))
        {
            SoundManager.instance.NewLevel();
            ScaleController.instance.ReturnToNormal();
            GameUIManager.instance.ActivateLoadScreen();
            if (shieldOn)
                shield.Deactivate();
            DeactivateDrug();
            hit.gameObject.transform.parent.gameObject.SetActive(false);
            MazeGenerator.instance.ResetAndGenerate();
            return;
        }

        //Enemy enemy = hit.gameObject.GetComponent<Enemy>();

        //if (enemy != null)
        //{
        //    Debug.Log("COLLIDED WITH ENEMY!!");
        //    if (ScaleController.instance.getSize() == Size.Large)
        //    {
        //        //bool isNextTo = Mathf.Abs(transform.position.y - hit.point.y) <= maxVerticalOffset;
        //        bool isSideCollision = Mathf.Abs(hit.normal.y) < 0.5f;

        //        if (isSideCollision)
        //        {
        //            Debug.Log("Side collision detected");
        //            enemy.Push(hit.moveDirection * pushForce);
        //        }
        //        else
        //        {
        //            hit.collider.enabled = false;
        //            enemy.Die();
        //        }
        //    }
        //    else if (!shieldOn)
        //    {
        //        GameOver();
        //    }
        //}
    }

    void GameOver()
    {
        if (!MazeGenerator.instance.generatingMaze)
        {
            SoundManager.instance.Lose();
            GameUIManager.instance.Lose();
            ThirdPersonController player = GetComponent<ThirdPersonController>();
            if (player != null)
                player.StopMoving();
        }
    }

    void DeactivateDrug()
    {
        onDrugs = false;
        shieldOn = false;
    }

    public void ActivateShield(float activationTime)
    {
        shield.Activate(activationTime);
        shieldOn = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();

        if (enemy != null)
        {
            if (ScaleController.instance.getSize() == Size.Large)
            {
                enemy.Die();
            }
            else if (!shieldOn)
                GameOver();
        }
        else if (!onDrugs && other.gameObject.GetComponent<Pill>() != null)
        {
            onDrugs = true;
            Pill pill = other.gameObject.GetComponent<Pill>();
            pill.Use();
            Invoke("DeactivateDrug", pill.getActiveTime() + 0.03f);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.GetComponent<Collectable>() != null)
        {
            other.gameObject.GetComponent<Collectable>().Collect();
        }
        else if (other.gameObject.CompareTag("Bounds"))
        {
            SoundManager.instance.PlaySound(outOfBoundsAudio);
            MazeGenerator.instance.SetPlayerPosition();
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionsController : MonoBehaviour
{
    public Shield shield;
    public float pushForce;
    public AudioClip outOfBoundsAudio;

    bool onDrugs = false, shieldOn = false, gameOver = false;

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
    }

    void GameOver()
    {
        if (!gameOver && !MazeGenerator.instance.generatingMaze)
        {
            gameOver = true;
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

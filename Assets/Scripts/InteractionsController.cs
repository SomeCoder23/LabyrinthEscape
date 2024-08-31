using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionsController : MonoBehaviour
{
    public float pushForce;

    bool onDrugs = false;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Portal"))
        {
            Debug.Log("Deactivating beam!");
            ScoreManager.instance.ActivateLoadScreen();
            hit.gameObject.transform.parent.gameObject.SetActive(false);
            SoundManager.instance.NewLevel();
            ScaleController.instance.ResetSize(false);
            MazeGenerator.instance.ResetAndGenerate();
            return;
        }

        Enemy enemy = hit.gameObject.GetComponent<Enemy>();

        if (enemy != null)
        {
            if (ScaleController.instance.getSize() == Size.Large)
            {
                //bool isNextTo = Mathf.Abs(transform.position.y - hit.point.y) <= maxVerticalOffset;
                bool isSideCollision = Mathf.Abs(hit.normal.y) < 0.5f;

                if (isSideCollision)
                {
                    Debug.Log("Side collision detected");
                    enemy.Push(hit.moveDirection * pushForce);
                }
                else enemy.Die();
            }
            else
            {
                GameOver();
            }
        }
    }

    void GameOver()
    {
        if (!MazeGenerator.instance.generatingMaze)
        {
            SoundManager.instance.Lose();
            ScoreManager.instance.Lose();
            ThirdPersonController player = GetComponent<ThirdPersonController>();
            if (player != null)
                player.StopMoving();
        }
    }

    void DeactivateDrug()
    {
        onDrugs = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!onDrugs && other.gameObject.GetComponent<Pill>() != null)
        {
            onDrugs = true;
            Pill pill = other.gameObject.GetComponent<Pill>();
            pill.Use();
            Invoke("DeactivateDrug", pill.getActiveTime());
            Destroy(other.gameObject);
        }
        else if (other.gameObject.GetComponent<Collectable>() != null)
        {
            other.gameObject.GetComponent<Collectable>().Collect();
        }
        else if (other.gameObject.CompareTag("Bounds"))
        {
            Debug.Log("PLAYER OUTSIDE BOUNDS!");
            MazeGenerator.instance.SetPlayerPosition();
        }
    }

}

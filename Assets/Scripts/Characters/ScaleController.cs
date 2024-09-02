using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScaleController : MonoBehaviour
{
    #region Singleton
    public static ScaleController instance;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("More than one Scale Controller!!");
            return;
        }
        instance = this;
    }

    #endregion
    [Header("Player Settings")]
    public float minSize = 0.3f;
    public float maxSize = 10f;
    public float scaleTime = 1;

    [Space]
    [Header("Audio Clips")]
    public AudioClip growSound;
    public AudioClip shrinkSound;

    [Space]
    [Header("Camera Settings")]
    public float maxCamHeight;
    public float minCamHeight;

    float normalSize, camHeight;
    Size size = Size.Normal;
    CamController cam;
    CharacterController controller;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        normalSize = transform.localScale.x;
        cam = FindObjectOfType<CamController>();
        camHeight = cam.getHeight();
    }

    public void Grow(float resetTime)
    {
        size = Size.Large;
        SoundManager.instance.PlaySound(growSound);
        StartCoroutine(ChangeSize(maxSize));
        Invoke("ResetSize", resetTime + scaleTime);
        //controller.height += 0.5f;
    }

    public void Shrink(float resetTime)
    {
        size = Size.Small;
        controller.height -= 0.5f;
        controller.radius -= 0.08f;
        SoundManager.instance.PlaySound(shrinkSound);
        StartCoroutine(ChangeSize(minSize));
        Invoke("ResetSize", resetTime + scaleTime);
    }

    IEnumerator ChangeSize(float newSize)
    {
        float newScale, elapsedTime = 0;
        AdjustCam();
        while(elapsedTime < scaleTime)
        {
            yield return new WaitForSeconds(0.01f);
            newScale = Mathf.Lerp(transform.localScale.x, newSize, elapsedTime);
            transform.localScale = new Vector3(newScale, newScale, newScale);
            elapsedTime += 0.01f;
        }
        SoundManager.instance.StopSound();
        StopCoroutine(ChangeSize(newSize));
    }

    void ResetSize()
    {
        if (size == Size.Normal)
            return;
        ResetController();
        StartCoroutine(ChangeSize(normalSize));
    }

    public void ReturnToNormal()
    {
        if (size == Size.Normal)
            return;
        StopAllCoroutines();
        ResetController(false);
        transform.localScale = new Vector3(normalSize, normalSize, normalSize);
        AdjustCam();
    }

    void ResetController(bool withSound = true)
    {
        if (size == Size.Small)
        {
            controller.height += 0.5f;
            controller.radius += 0.08f;
            if(withSound) SoundManager.instance.PlaySound(growSound);
        }
        else
        {
            //controller.height -= 0.5f;
            if (withSound) SoundManager.instance.PlaySound(shrinkSound);
        }

        size = Size.Normal;
    }

    void AdjustCam()
    {
        switch (size)
        {
            case Size.Small: cam.setHeight(minCamHeight);  break;
            case Size.Normal: cam.setHeight(camHeight); break;
            case Size.Large: cam.setHeight(maxCamHeight); break;
        }
    }

    public Size getSize()
    {
        return size;
    }
}

public enum Size
{
    Small,
    Normal,
    Large
}

﻿
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;


/*
    This file has a commented version with details about how each line works. 
    The commented version contains code that is easier and simpler to read. This file is minified.
*/


/// <summary>
/// Main script for third-person movement of the character in the game.
/// Make sure that the object that will receive this script (the player) 
/// has the Player tag and the Character Controller component.
/// </summary>
public class ThirdPersonController : MonoBehaviour
{
    [Tooltip("Speed ​​at which the character moves. It is not affected by gravity or jumping.")]
    public float velocity = 5f;
    [Tooltip("This value is added to the speed value while the character is sprinting.")]
    public float sprintAdittion = 3.5f;
    [Tooltip("The higher the value, the higher the character will jump.")]
    public float jumpForce = 18f;
    [Tooltip("Stay in the air. The higher the value, the longer the character floats before falling.")]
    public float jumpTime = 0.85f;
    [Space]
    [Tooltip("Force that pulls the player down. Changing this value causes all movement, jumping and falling to be changed as well.")]
    public float gravity = 9.8f;

    public Slider distanceSlider;

    float jumpElapsedTime = 0;

    // Player states
    bool isJumping = false;
    bool isSprinting = false;

    // Inputs
    float inputHorizontal;
    float inputVertical;
    bool inputJump;
    bool inputSprint;

    Animator animator;
    CharacterController cc;
    GameObject beam;
    Vector3 distance;
    float maxDistance;

    bool dead = false;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        beam = GameObject.FindGameObjectWithTag("Portal");

        // Message informing the user that they forgot to add an animator
        if (animator == null)
            Debug.LogWarning("Hey buddy, you don't have the Animator component in your player. Without it, the animations won't work.");
    }


    // Update is only being used here to identify keys and trigger animations
    void Update()
    {
        if (dead) return;
        // Input checkers
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");
        inputJump = Input.GetAxis("Jump") == 1f;
        inputSprint = Input.GetAxis("Fire3") == 1f;

        if ( cc.isGrounded && animator != null )
        {
            
            // Run
            float minimumSpeed = 0.9f;
            animator.SetBool("run", cc.velocity.magnitude > minimumSpeed );

            // Sprint
            isSprinting = cc.velocity.magnitude > minimumSpeed && inputSprint;
            animator.SetBool("sprint", isSprinting );

        }

        // Jump animation
        if( animator != null )
            animator.SetBool("air", cc.isGrounded == false );

        // Handle can jump or not
        if ( inputJump && cc.isGrounded )
        {
            isJumping = true;
        }

        HeadHittingDetect();

        distance = beam.transform.position - transform.position;
        distanceSlider.value = distanceSlider.maxValue - (Mathf.Clamp01(distance.magnitude / maxDistance) * 100);

    }


    // With the inputs and animations defined, FixedUpdate is responsible for applying movements and actions to the player
    private void FixedUpdate()
    {

        if (dead) 
            return;
        // Sprinting velocity boost or crounching desacelerate
        float velocityAdittion = 0;
        if ( isSprinting )
            velocityAdittion = sprintAdittion;

        // Direction movement
        float directionX = inputHorizontal * (velocity + velocityAdittion) * Time.deltaTime;
        float directionZ = inputVertical * (velocity + velocityAdittion) * Time.deltaTime;
        float directionY = 0;
        //Debug.Log("MOVEMENT=> DIRECTION X = " + directionX + " DIRECTION Z = " + directionZ);

        // Jump handler
        if ( isJumping )
        {

            // Apply inertia and smoothness when climbing the jump
            // It is not necessary when descending, as gravity itself will gradually pulls
            directionY = Mathf.SmoothStep(jumpForce, jumpForce * 0.30f, jumpElapsedTime / jumpTime) * Time.deltaTime;

            // Jump timer
            jumpElapsedTime += Time.deltaTime;
            if (jumpElapsedTime >= jumpTime)
            {
                isJumping = false;
                jumpElapsedTime = 0;
            }
        }

        // Add gravity to Y axis
        directionY = directionY - gravity * Time.deltaTime;

        
        // --- Character rotation --- 

        //Vector3 forward = Camera.main.transform.forward;
        //Vector3 right = Camera.main.transform.right;

        //forward.y = 0;
        //right.y = 0;

        //forward.Normalize();
        //right.Normalize();
        //Debug.Log("ROTATION => FORWARD = " + forward + " RIGHT = " + right);

        // Relate the front with the Z direction (depth) and right with X (lateral movement)
        Vector3 forward = Vector3.forward * directionZ;
        Vector3 right = Vector3.right * directionX;

        if (directionX != 0 || directionZ != 0)
        {
            float angle = Mathf.Atan2(forward.x + right.x, forward.z + right.z) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.15f);
        }

        // --- End rotation ---

        
        Vector3 verticalDirection = Vector3.up * directionY;
        Vector3 horizontalDirection = forward + right;

        Vector3 moviment = verticalDirection + horizontalDirection;
        cc.Move( moviment );

    }


    //This function makes the character end his jump if he hits his head on something
    void HeadHittingDetect()
    {
        float headHitDistance = 1.1f;
        Vector3 ccCenter = transform.TransformPoint(cc.center);
        float hitCalc = cc.height / 2f * headHitDistance;

        // Uncomment this line to see the Ray drawed in your characters head
        // Debug.DrawRay(ccCenter, Vector3.up * headHeight, Color.red);

        if (Physics.Raycast(ccCenter, Vector3.up, hitCalc))
        {
            jumpElapsedTime = 0;
            isJumping = false;
        }
    }


    public void SetMaxDistance(float max)
    {
        maxDistance = max;
    }

    public void StopMoving(bool stop = true)
    {
        dead = stop;
    }


}

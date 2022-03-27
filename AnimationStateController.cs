using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror; // Can be removed if not using Multiplayer

public class AnimationStateController : NetworkBehaviour
{
    private Animator animator;
    [SerializeField] private float acceleration = 2.0f;
    [SerializeField] private float deceleration = 2.0f;
    [SerializeField] private float maximumWalkVelocity = 0.5f;
    [SerializeField] private float maximumRunVelocity = 2.0f;

    private float velocityZ = 0.0f;
    private float velocityX = 0.0f;

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    void changeVelocity(bool forwardPressed, bool backwardsPressed, bool rightPressed, bool leftPressed, bool runPressed, float currentMaxVelocity)
    {
        // if forward, increase velocityZ
        if (forwardPressed && velocityZ < currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * acceleration;
        }

        // if backwards, decrease velocityZ
        if (backwardsPressed && velocityZ > -currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * acceleration;
        }

        // if right, increase velocityX
        if (rightPressed && velocityX < currentMaxVelocity)
        {
            velocityX += Time.deltaTime * acceleration;
        }

        // if left, decrease velocityX
        if (leftPressed && velocityX > -currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * acceleration;
        }

        // if not moving start decreasing velocityZ & velocityX
        if (!forwardPressed && velocityZ > 0.0f)
        {
            velocityZ -= Time.deltaTime * deceleration;
        }

        if (!backwardsPressed && velocityZ < 0.0f)
        {
            velocityZ += Time.deltaTime * deceleration;
        }

        if (!leftPressed && velocityX < 0.0f)
        {
            velocityX += Time.deltaTime * deceleration;
        }

        if (!rightPressed && velocityX > 0.0f)
        {
            velocityX -= Time.deltaTime * deceleration;
        }
    }

    void lockOrResetVelocity(bool forwardPressed, bool backwardsPressed, bool rightPressed, bool leftPressed, bool runPressed, float currentMaxVelocity)
    {
        //if not moving velocityZ & velocityX = 0.0f
        if (!forwardPressed && !backwardsPressed && velocityZ != 0.0f && (velocityZ > -0.05f && velocityZ < 0.05f))
        {
            velocityZ = 0.0f;
        }

        if (!leftPressed && !rightPressed && velocityX != 0.0f && (velocityX > -0.05f && velocityX < 0.05f))
        {
            velocityX = 0.0f;
        }

        // forward max velocity
        if (forwardPressed && runPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ = currentMaxVelocity;
        }
        else if (forwardPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * deceleration;
            if (velocityZ > currentMaxVelocity && velocityZ < (currentMaxVelocity + 0.05f))
            {
                velocityZ = currentMaxVelocity;
            }
        }
        else if (forwardPressed && velocityZ < currentMaxVelocity && velocityZ > (currentMaxVelocity - 0.05f))
        {
            velocityZ = currentMaxVelocity;
        }

        // backwards max velocity
        if (backwardsPressed && runPressed && velocityZ < -currentMaxVelocity)
        {
            velocityZ = -currentMaxVelocity;
        }
        else if (backwardsPressed && velocityZ < -currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * deceleration;
            if (velocityZ < -currentMaxVelocity && velocityZ > (-currentMaxVelocity - 0.05f))
            {
                velocityZ = -currentMaxVelocity;
            }
        }
        else if (backwardsPressed && velocityZ > -currentMaxVelocity && velocityZ < (-currentMaxVelocity + 0.05f))
        {
            velocityZ = -currentMaxVelocity;
        }

        // left max velocity
        if (leftPressed && runPressed && velocityX < -currentMaxVelocity)
        {
            velocityX = -currentMaxVelocity;
        }
        else if (leftPressed && velocityX < -currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * deceleration;
            if (velocityX < -currentMaxVelocity && velocityX > (-currentMaxVelocity - 0.05f))
            {
                velocityX = -currentMaxVelocity;
            }
        }
        else if (leftPressed && velocityX > -currentMaxVelocity && velocityX < (-currentMaxVelocity + 0.05f))
        {
            velocityX = -currentMaxVelocity;
        }

        // right max velocity
        if (rightPressed && runPressed && velocityX > currentMaxVelocity)
        {
            velocityX = currentMaxVelocity;
        }
        else if (rightPressed && velocityX > currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * deceleration;
            if (velocityX > currentMaxVelocity && velocityX < (currentMaxVelocity + 0.05f))
            {
                velocityX = currentMaxVelocity;
            }
        }
        else if (rightPressed && velocityX < currentMaxVelocity && velocityX > (currentMaxVelocity - 0.05f))
        {
            velocityX = currentMaxVelocity;
        }
    }

    private void Update()
    {
        if (isLocalPlayer) // Only run on local player, can be removed if not using Multiplayer
        {
            bool forwardPressed = Input.GetKey(KeyCode.W);
            bool backwardsPressed = Input.GetKey(KeyCode.S);
            bool rightPressed = Input.GetKey(KeyCode.D);
            bool leftPressed = Input.GetKey(KeyCode.A);
            bool runPressed = Input.GetKey(KeyCode.LeftShift);

            // Current max velocity
            float currentMaxVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity;

            // Handle changes in velocity
            changeVelocity(forwardPressed, backwardsPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);
            lockOrResetVelocity(forwardPressed, backwardsPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);

            // Change BlendTree float
            animator.SetFloat("Vertical", velocityX);
            animator.SetFloat("Horizontal", velocityZ);

            // Debug
            //Debug.Log("X: " + velocityX + " Z: " + velocityZ);
            //Debug.Log("Current Max Velocity " + currentMaxVelocity);
        }
    }
}

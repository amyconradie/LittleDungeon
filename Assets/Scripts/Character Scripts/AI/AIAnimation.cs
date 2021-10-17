using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class AIAnimation : MonoBehaviour
{
    private Animator animator;
    float timeSinceLastCall;
    private Vector3 lastUpdatePos = Vector3.zero;
    private Vector3 dist;
    private float currentSpeed;

    void Start()
    {
        this.animator = GetComponent<Animator>();
    }

    void Update() // Updates 1x per frame, use this to register input
    {

        timeSinceLastCall += Time.deltaTime;

        //update animator if ai has moved in the last second
        if (timeSinceLastCall >= 0.5f)
        {
            // calculate distance and speed
            dist = (transform.position - lastUpdatePos).normalized;
            currentSpeed = dist.magnitude / Time.deltaTime;
            lastUpdatePos = transform.position;

            // update animator
            animator.SetFloat("Horizontal", dist.x);
            animator.SetFloat("Vertical", dist.y);
            animator.SetFloat("Speed", currentSpeed);

            // reset timer back to 0
            timeSinceLastCall = 0;   
        }
        
    }

}

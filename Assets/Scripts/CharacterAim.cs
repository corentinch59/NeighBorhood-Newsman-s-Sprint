using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class CharacterAim : MonoBehaviour
{
    private PlayerInput playerinput;
    private TargetingSystem<TargetableObject> targetingSystem;
    private bool isAiming;

    public float maxDistance;
    public float radius;
    public GameObject mainCamera;
    public GameObject aimCamera;
    public Animator anim;

    private void Awake()
    {
        playerinput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        targetingSystem = new TargetingSystem<TargetableObject>();
    }

    public void onAim(InputAction.CallbackContext ctx)
    {
        if (mainCamera.activeInHierarchy)
        {
            mainCamera.SetActive(false);
            aimCamera.SetActive(true);
            anim.SetBool("Throw", true);
            Time.timeScale = 0.5f;
            isAiming = true;
        }
        else
        {
            aimCamera.SetActive(false);
            mainCamera.SetActive(true);
            anim.SetBool("Throw", false);
            anim.speed = 5;
            Time.timeScale = 1;
            isAiming = false;

            ThrowLetter();
        }
    }

    public void Update()
    {
        if (isAiming)
        {
            // Populate the list of targets within a range of 50 units from the player
            targetingSystem.PopulateTargetsInRange(transform.position, 50f);

            // Get the closest target to the player's position and set it as the current target
            TargetableObject closestTarget = targetingSystem.GetClosestTarget(Camera.main, maxDistance, radius);
        }
    }

    public void ThrowLetter()
    {
        Debug.Log("i threw my letter");
    }

}

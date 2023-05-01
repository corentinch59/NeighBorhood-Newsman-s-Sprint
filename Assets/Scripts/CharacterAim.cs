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

            // Create a circle with the correct radius and alpha
            GameObject circle = new GameObject("AimCircle");
            circle.transform.SetParent(aimCamera.transform);
            circle.transform.localScale = Vector3.one * 3f; // Start larger than the target size
            SpriteRenderer sr = circle.AddComponent<SpriteRenderer>();
            sr.sprite = Resources.Load<Sprite>("CircleSprite");
            sr.color = new Color(1f, 1f, 1f, 0f); // Start with zero alpha
            sr.sortingOrder = 10;
            float targetSize = radius * 2f * 100f;
            circle.transform.DOScale(new Vector3(targetSize, targetSize, 1f), 0.5f)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);
            sr.DOFade(0.5f, 0.5f); // Fade in the circle
        }
        else
        {
            aimCamera.SetActive(false);
            mainCamera.SetActive(true);
            anim.SetBool("Throw", false);
            anim.speed = 5;
            Time.timeScale = 1;
            isAiming = false;

            // Destroy the circle with an animation
            GameObject circle = GameObject.Find("AimCircle");
            if (circle != null)
            {
                SpriteRenderer sr = circle.GetComponent<SpriteRenderer>();
                sr.DOFade(0f, 0.5f); // Fade out the circle
                circle.transform.DOScale(Vector3.one * 3f, 0.5f) // Grow the circle before destroying
                    .SetEase(Ease.InBack)
                    .OnComplete(() => Destroy(circle))
                    .SetUpdate(true);
            }
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

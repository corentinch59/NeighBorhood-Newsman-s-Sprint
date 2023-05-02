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
    private GameObject sphereIndicator; // New variable to store the sphere indicator

    public float maxDistance;
    public float radius;
    public GameObject mainCamera;
    public GameObject aimCamera;
    public Animator anim;
    public GameObject projectilePrefab; // Prefab for the projectile
    public Material sphereMaterial; // Material for the sphere indicator

    public CharacterMovement characterMovement;
    private void Awake()
    {
        playerinput = GetComponent<PlayerInput>();
        characterMovement = GetComponent<CharacterMovement>();
    }

    private void Start()
    {
        targetingSystem = new TargetingSystem<TargetableObject>();
        // sphereIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere); // Create a sphere object
        // Destroy(sphereIndicator.GetComponent<SphereCollider>());
        // sphereIndicator.GetComponent<Renderer>().material = sphereMaterial; // Assign the sphere material
        // sphereIndicator.SetActive(false); // Hide the sphere initially
    }

    public void onAim(InputAction.CallbackContext ctx)
    {
        if (mainCamera.activeInHierarchy)
        {
            targetingSystem.PopulateTargetsInRange(transform.position, 200);
            mainCamera.SetActive(false);
            aimCamera.SetActive(true);
            anim.SetBool("Throw", true);
            Time.timeScale = 0.5f;
            isAiming = true;
            // sphereIndicator.SetActive(true); // Show the sphere
        }
        else
        {
            aimCamera.SetActive(false);
            mainCamera.SetActive(true);
            anim.SetBool("Throw", false);
            anim.speed = 5;
            Time.timeScale = 1;
            isAiming = false;
            // sphereIndicator.SetActive(false); // Hide the sphere

            // Hide target indicators

            // Create a ray from the mouse cursor on screen in the direction of the camera
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

            RaycastHit hit;
            // Perform the raycast
            if(Physics.Raycast(ray, out hit))
            {
                // If the raycast hit the sphere, perform the throw
                if(hit.collider.gameObject.tag == "target")
                {
                    ThrowLetter(hit.collider.gameObject.transform.position);
                }
            }
            targetingSystem.HideTargetIndicators();
        }
    }

    public void Update()
    {
        if (isAiming)
        {
            // Populate the list of targets within a range of 50 units from the player

            // Generate target indicators
            targetingSystem.GenerateTargetIndicators(sphereMaterial,3.5f); // adjust the second parameter for the size of the spheres

            // Get the closest target to the player's position and set it as the current target
            // TargetableObject closestTarget = targetingSystem.GetClosestTarget(Camera.main, maxDistance, 50f);
        }
    }

    public void ThrowLetter(Vector3 position)
    {
        TargetableObject target = targetingSystem.GetTarget(position);
        if (target != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity); // Instantiate the projectile
            Vector3 direction = (target.GetPosition() - transform.position).normalized; // Calculate direction

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            // If the rigidbody component doesn't exist, add it
            if(rb == null)
            {
                rb = projectile.AddComponent<Rigidbody>();
            }

            rb.velocity = direction * 160f; // Set the velocity, adjust the speed multiplier as necessary

            //destroy the projectile and the target after 5 seconds
            Destroy(projectile, 5f);
            Destroy(target.GetGameObject()); // destroy the target object
            characterMovement.score += 1;
            characterMovement.energy += 10;
            if(characterMovement.energy > 100)
            {
                characterMovement.energy = 100;
            }
        }
        Debug.Log("I threw my letter");
    }
}
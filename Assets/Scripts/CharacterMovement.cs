using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    public float speed;
    public float movementCap;

    private Rigidbody rb;
    private PlayerInput playercontrols;
    private Vector2 movement;

    private void Awake()
    {
        playercontrols = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 actualMovement = new Vector3(movement.x, 0, movement.y);

        if (rb.velocity.magnitude < movementCap)
            rb.velocity += actualMovement * Time.deltaTime * speed;
    }

    public void onMove(InputAction.CallbackContext ctx)
    {
        movement = ctx.ReadValue<Vector2>();
    }
}
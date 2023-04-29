using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    public float speed;
    public float movementSpeedCap;
    public float rotationPower;

    public GameObject followTransform;
    public Vector3 followTransformOffset;

    private Rigidbody rb;
    private PlayerInput playercontrols;

    private Vector2 _move;
    private Vector2 _look;

    public void onMove(InputAction.CallbackContext ctx)
    {
        _move = ctx.ReadValue<Vector2>();
    }

    public void onLook(InputAction.CallbackContext ctx)
    {
        _look = ctx.ReadValue<Vector2>();
    }

    private void Awake()
    {
        playercontrols = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        followTransform.transform.position = transform.position + followTransformOffset;
        #region Camera
        followTransform.transform.rotation *= Quaternion.AngleAxis(_look.x * rotationPower, Vector3.up);

        followTransform.transform.rotation *= Quaternion.AngleAxis(-_look.y * rotationPower, Vector3.right);

        var angles = followTransform.transform.localEulerAngles;
        angles.z = 0;

        var angle = followTransform.transform.localEulerAngles.x;

        //Clamp the Up/Down rotation
        if (angle > 180 && angle < 340)
        {
            angles.x = 340;
        }
        else if (angle < 180 && angle > 40)
        {
            angles.x = 40;
        }

        followTransform.transform.localEulerAngles = angles;
        #endregion

        #region Movement

        if (rb.velocity.magnitude < movementSpeedCap)
            rb.velocity += (transform.forward * _move.y * speed * Time.deltaTime) + (transform.right * _move.x * speed * Time.deltaTime);
        #endregion

        transform.rotation = Quaternion.Euler(0, followTransform.transform.rotation.eulerAngles.y, 0);
    }
}
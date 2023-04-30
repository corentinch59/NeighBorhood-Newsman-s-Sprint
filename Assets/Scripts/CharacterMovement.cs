using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    public float speed;
    public float movementSpeedCap;
    public float rotationPower;
    public float deceleration = 5.0f;
    public float acceleration = 5.0f;
    private float rotationSpeed = 5.0f;
    private float maxGroundAngle = 85.0f;

    public GameObject followTransform;
    public Vector3 followTransformOffset;

    private PlayerInput playercontrols;
    private Rigidbody rb;
    private CharacterController characterController;

    public Vector3 velocity;
    private Vector2 _move;
    private Vector2 _look;
    private Vector3 groundNormal;

    private float fixedDeltaTime;
    private float deltaTime;

    public void onMove(InputAction.CallbackContext ctx)
    {
        _move = ctx.ReadValue<Vector2>();
    }

    public void onLook(InputAction.CallbackContext ctx)
    {
        _look = ctx.ReadValue<Vector2>()*rotationPower;
        transform.Rotate(0, _look.x, 0);
        if(Mathf.Abs(followTransform.transform.localRotation.x+_look.y) < 30) followTransform.transform.Rotate(-_look.y, 0, 0);
    }

    private void Awake()
    {
        playercontrols = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {   
        deltaTime = Time.deltaTime;
        bool isGrounded = characterController.isGrounded;
        if(_move.x != 0 && isGrounded) transform.Rotate(0, _move.x*rotationPower*4*Mathf.Sqrt(velocity.magnitude), 0);
        if(_move.y  != 0)
            velocity += new Vector3(0,0,_move.y) * acceleration * deltaTime;
        else {
            Vector3 deltaVelocity = new Vector3(velocity.x,0,velocity.z).normalized * deceleration * deltaTime;
            velocity -= deltaVelocity;
            if(velocity.magnitude < 0.1f || Vector3.Dot(-deltaVelocity,velocity)>0) velocity = Vector3.zero;
        }
        if(!characterController.isGrounded) velocity -= Vector3.up * deceleration * 1f * deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, movementSpeedCap);
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded){
            velocity.y = 10;
        }
        characterController.Move((velocity.z*transform.forward+velocity.x*transform.right+velocity.y*Vector3.up) * deltaTime);

      
        // if (rb.velocity.magnitude < movementSpeedCap)
        // {
        //     Vector3 force = (transform.forward * _move.y * speed) + (transform.right * _move.x * speed);
        //     rb.AddForce(force, ForceMode.VelocityChange);
        // }
    }  

    private void FixedUpdate(){
        RotateToSurfaceNormal();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.point.y < transform.position.y-0.5) return;
        Vector3 normal = -hit.normal;
        Vector3 globalVelocity = velocity.x * transform.right + velocity.z * transform.forward;
        float dotProduct = Vector3.Dot(globalVelocity.normalized, normal);
        if(dotProduct > 0)
            velocity -= dotProduct * 1.5f * velocity;
    }

    private void RotateToSurfaceNormal()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 5f))
        {
            groundNormal = hit.normal;
            float angle = Vector3.Angle(Vector3.up, groundNormal);
            if (angle < maxGroundAngle)
            {
                Debug.Log(angle);
                Quaternion targetRotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * deltaTime);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.UI;

public class CharacterMovement : MonoBehaviour
{
    public float speed;
    public float movementSpeedCap;
    public float rotationPower;
    public float cameraLookPower;
    public float deceleration = 5.0f;
    public float acceleration = 5.0f;
    private float rotationSpeed = 5.0f;
    private float maxGroundAngle = 85.0f;

    public GameObject followTransform;
    public Vector3 followTransformOffset;
    public GameObject menu;

    private PlayerInput playercontrols;
    private Rigidbody rb;
    private CharacterController characterController;
    public Animator animator;
    public Animator skateAnimator;
    [SerializeField] private CinemachineVirtualCamera vcam;
    private CinemachineComposer composer;

    public Vector3 velocity;
    private Vector2 _move;
    private Vector2 _look;
    private Vector3 groundNormal;
    
    private float fixedDeltaTime;
    private float deltaTime;
    private float lastGroundTime = 0.0f;
    public float energy = 100.0f;
    public GameObject energyBar;
    public bool isPlaying = false;
    public Vector3 startPosition;
    public int score = 0;
    //legacy text
    public UnityEngine.UI.Text scoreUI;
    public float startTime;
    public UnityEngine.UI.Text timerUI;
    public Slider mySlider;
    private bool alreadyPlayed = false;
    public void onMove(InputAction.CallbackContext ctx)
    {
        if(!isPlaying) return;
        _move = ctx.ReadValue<Vector2>();
        animator.SetFloat("Trust", _move.y);
    }

    public void onLook(InputAction.CallbackContext ctx)
    {
        if(!isPlaying) return;
        _look = ctx.ReadValue<Vector2>();
        float y = _look.y*deltaTime*cameraLookPower*1;//(Time.timeScale == 1 ? 1 : 0.7f)
        transform.Rotate(0, _look.x*cameraLookPower*1*deltaTime, 0);
        if(Mathf.Abs(followTransform.transform.localRotation.x+y) < 30) followTransform.transform.Rotate(-y, 0, 0);
    }

    public void onJump(InputAction.CallbackContext ctx)
    {
        if(Time.time - lastGroundTime < 0.2f) velocity.y = 20;
    }

    public void onBoost(InputAction.CallbackContext ctx)
    {
        if (energy >= 10)
        {
            velocity.z += 15;
            energy -= 10;
        }
    }

    private void Awake()
    {
        playercontrols = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
        composer = vcam.GetComponentInChildren<CinemachineComposer>();
        skateAnimator.SetBool("Grounded",true);
        startPosition = transform.position;
        mySlider.onValueChanged.AddListener(OnSliderChanged);
    }

    public void OnPlay(){
        isPlaying = true;
        Cursor.lockState = CursorLockMode.Locked;
        score = 0;
        transform.position = startPosition;
        menu.transform.Find("Win").gameObject.SetActive(false);
        menu.transform.Find("Lose").gameObject.SetActive(false);
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        startTime = Time.time;
        yield return new WaitForSeconds(120);
        isPlaying = false;
        if(score > 40){
            menu.transform.Find("Win").gameObject.SetActive(true);
        }else{
            menu.transform.Find("Lose").gameObject.SetActive(true);
        }
    }

    public void OnSliderChanged(float value)
    {
        cameraLookPower = value;
    }

    private void Update()
    {   
        scoreUI.text = "Score: " + score;
        if(isPlaying) timerUI.text = "Time Left: " + (120-(int)(Time.time - startTime));
        if(!isPlaying){menu.SetActive(true);Cursor.lockState = CursorLockMode.None;return;}else menu.SetActive(false);

        deltaTime = Time.deltaTime;
        energyBar.transform.localScale = new Vector3(1, energy/100, 1);
        bool isGrounded = characterController.isGrounded;
        skateAnimator.SetBool("Grounded", Time.time - lastGroundTime < 0.1f);
        if(isGrounded) lastGroundTime = Time.time;
        if(_move.x != 0 && isGrounded) transform.Rotate(0, _move.x*rotationPower*4*Mathf.Sqrt(velocity.magnitude)*Time.timeScale*Time.timeScale, 0);
        if(_move.y  != 0){
            if(velocity.magnitude < movementSpeedCap) velocity += new Vector3(0,0,_move.y) * acceleration * deltaTime;
        }
        if(_move.y <= 0){
            Vector3 deltaVelocity = new Vector3(velocity.x,0,velocity.z).normalized * deceleration * deltaTime;
            velocity -= deltaVelocity;
            if(velocity.magnitude < 0.1f || Vector3.Dot(-deltaVelocity,velocity)>0) velocity = Vector3.zero;
        }
        if(!characterController.isGrounded) velocity -= Vector3.up * deceleration * 1f * deltaTime;
        // velocity = Vector3.ClampMagnitude(velocity, movementSpeedCap);

        animator.SetFloat("Speed", velocity.magnitude);
        //if animator is playing running and throwing is not getting played
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Running") && animator.GetCurrentAnimatorStateInfo(1).IsName("Throwing"))
            animator.speed = velocity.magnitude/60;
        if(_move.y != 0 && velocity.magnitude < 40){
            animator.speed = 2;
        }
        float currentSpeed = Mathf.Clamp(velocity.magnitude, 0, movementSpeedCap);
        float speedRatio = currentSpeed / movementSpeedCap;
        vcam.m_Lens.FieldOfView = Mathf.Lerp(60, 100, speedRatio);
        composer.m_SoftZoneWidth = Mathf.Lerp(0.353f, 0.2f, speedRatio);
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
        // if(hit.point.y < transform.position.y-0.5) return;
        if( Vector3.Dot(transform.up, hit.normal)> 0.4) return;
        Vector3 normal = -hit.normal;
        Vector3 globalVelocity = velocity.x * transform.right + velocity.z * transform.forward;
        float dotProduct = Vector3.Dot(globalVelocity.normalized, normal);
        //reflect the velocity on the wall
        if(dotProduct > 0)
            velocity -=  1.3f * velocity;
    }

    private void RotateToSurfaceNormal()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 1.5f))
        {
            groundNormal = hit.normal;
        }else{
            groundNormal = Vector3.up;

        }
        float angle = Vector3.Angle(Vector3.up, groundNormal);
        if (angle < maxGroundAngle)
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * deltaTime);
        }
    }
}
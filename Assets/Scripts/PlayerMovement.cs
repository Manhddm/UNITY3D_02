using System.Globalization;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController _controls;
    
    public CharacterController characterController;
    private Animator _animator;
    private Vector3 moveDirection;
    [Header("Movement info")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 4.5f;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private bool isRunning = true;
    [SerializeField] private bool isMoving = false;
    [Header("Aim data")] 
    public LayerMask aimLayerMask;
    [SerializeField] private Transform aimTarget;
    private Vector3 lookDirection;
    
    private float verticalVelocity;
    
    private Vector2 moveInput ;
    private Vector2 aimInput;
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Awake()
    {
        _controls = new PlayerController();
        characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        Register();
    }

    private void Update()
    {
        
        Movement();
        AimTowardsMouse();
        AnimatorController();
        
    }

    private void AnimatorController()
    {
        Vector3 input3 =  new Vector3(moveInput.x, 0, moveInput.y);
        float xVelocity = Vector3.Dot(input3, transform.right);
        float zVelocity = Vector3.Dot(input3, transform.forward);
        _animator.SetFloat("xVelocity", xVelocity, .1f, Time.deltaTime);
        _animator.SetFloat("zVelocity", zVelocity, .1f, Time.deltaTime);
        _animator.SetBool("isRunning", isRunning);
        isMoving = input3 != Vector3.zero;
        _animator.SetBool("isMoving", isMoving);
    }


    #region Movement

    private void Movement()
    {
        moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        ApplyGravity();
        float speed = isRunning ? runSpeed : walkSpeed;
        characterController.Move(moveDirection * (Time.deltaTime * speed));
        
    }

    private void Jump()
    {
        Debug.Log("Jump");

    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded == false)
        {
            verticalVelocity += (verticalVelocity - gravity)*Time.deltaTime;
            moveDirection.y = verticalVelocity;
        }
        else
        {
            verticalVelocity = -.5f;
        }
    }

    #endregion

    private void AimTowardsMouse()
    {
        if (_camera != null)
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, aimLayerMask))
            {
                lookDirection = hit.point - transform.position;//giai thich: 
                lookDirection.y = 0f;
                lookDirection.Normalize();
                transform.forward = lookDirection;
                aimTarget.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            }
        }
    }

    private void Shoot()
    {
        _animator.SetTrigger("Fire");
    }

    #region Input System
    private void Register()
    {
        _controls.Character.Fire.performed += context => Shoot();
        _controls.Character.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        _controls.Character.Movement.canceled += context => moveInput = Vector2.zero;

        _controls.Character.Jump.started += context => Jump();
        
        _controls.Character.Aim.performed += context => aimInput = context.ReadValue<Vector2>();
        _controls.Character.Aim.canceled += context => aimInput = Vector2.zero;
        
        _controls.Character.Run.started += context => isRunning = !isRunning;
        //_controls.Character.Run.canceled += context => isRunning = false;
    }
    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    #endregion


}

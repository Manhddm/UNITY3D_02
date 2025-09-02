using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController _controls;
    
    public CharacterController characterController;
    private Vector3 moveDirection;
    [Header("Movement info")]
    [SerializeField]private float walkSpeed = 3f;
    [SerializeField]private float runSpeed = 6f;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float gravity = 9.8f;
    private float verticalVelocity;
    
    private Vector2 moveInput ;
    private Vector2 aimInput;
    private void Awake()
    {
        _controls = new PlayerController();
        characterController = GetComponent<CharacterController>();
        Register();
    }

    private void Update()
    {
        
        Movement();
        
    }


    private void Register()
    {
        //_controls.Character.Fire.performed += context => Shoot();
        _controls.Character.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        _controls.Character.Movement.canceled += context => moveInput = Vector2.zero;

        _controls.Character.Jump.started += context => Jump();
        
        _controls.Character.Aim.performed += context => aimInput = context.ReadValue<Vector2>();
        _controls.Character.Aim.canceled += context => aimInput = Vector2.zero;
    }
    #region Movement

    private void Movement()
    {
        moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        ApplyGravity();
        characterController.Move(moveDirection * (Time.deltaTime * walkSpeed));
        
    }

    private void Jump()
    {
        Debug.Log("Jump");
        moveDirection.y = jumpHeight;
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

    #region MyRegion

    private void Shoot()
    {
        Debug.Log("Bang bang");
    }

    #endregion
    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }
}

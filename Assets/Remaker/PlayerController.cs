using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    public PlayerInputControl inputControl;
    private PhysicCheck physicCheck;
    public Vector2 inputDirection;
    private Rigidbody2D Rb;

    [Header("Basic i")]
    public float runSpeed;
    public float jumpforce;
    public bool isKnocked;
    private void Awake()
    {

        inputControl = new PlayerInputControl();
        physicCheck = GetComponent<PhysicCheck>();
        Rb = GetComponent<Rigidbody2D>();
        inputControl.Gameplay.Jump.started += Jump;//使JUMP輸入註冊(+=)jump
    }
    private void Update()
    {
        inputDirection = inputControl.Gameplay.Movement.ReadValue<Vector2>();
    }
     private void FixedUpdate()
    {
        if (!isKnocked)
            Move();
    }
    void Start()
    {

    }
    private void OnEnable()
    {
        inputControl.Enable();
    }
    private void OnDisable()
    {
        inputControl.Disable();
    }

    public void Move()
    {
        Rb.velocity = new Vector2(inputDirection.x * runSpeed * Time.deltaTime, Rb.velocity.y);
        int faceDir = (int)transform.localScale.x;
        if (inputDirection.x < 0)
        {
            faceDir = -1;
        }
        if (inputDirection.x > 0)
        {
            faceDir = 1;
        }
        transform.localScale = new Vector3(faceDir, 1, 1);
    }

    private void Jump(InputAction.CallbackContext obj)
    {
        if (physicCheck.canJump)
        {
            physicCheck.groundHourglass = 0;
            Debug.Log(physicCheck.groundHourglass);
            if (Rb.velocity.y < 0)
                jumpforce -= Rb.velocity.y;
            Rb.AddForce(transform.up * jumpforce, ForceMode2D.Impulse);

        }
    }
}

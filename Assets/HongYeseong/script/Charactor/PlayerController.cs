using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Animator animator;

    public float walkSpeed = 3;
    public float runSpeed = 6;

    private Vector3 moveInput;
    private float currentSpeed;

    private bool isAttacking;
    
    Vector2 inputVector;

    CharaStat characterStats;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        characterStats = GetComponent<CharaStat>();
    }

    void Start()
    {
        walkSpeed = characterStats.walkSpeed;
        runSpeed = characterStats.walkSpeed * 2;
    }
    void Update()
    {
        if (isAttacking) return;
        
        bool isRun = Input.GetKey(KeyCode.LeftShift);
        currentSpeed = moveInput.magnitude > 0 ? (isRun ? runSpeed : walkSpeed) : 0;

        animator.SetFloat("MoveX", moveInput.x);
        animator.SetFloat("MoveY", moveInput.z);
        animator.SetFloat("Speed", currentSpeed);
    }

    void FixedUpdate()
    {
        if (isAttacking) return;

        Vector3 velocity = moveInput * currentSpeed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;
    }

    void OnFireAttack()
    {
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(1f);

        isAttacking = false;
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
        moveInput = new Vector3(inputVector.x, 0f, inputVector.y).normalized;
    }
}
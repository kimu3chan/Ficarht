using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.InputSystem;

public class CharaStat : MonoBehaviour
{
    public CharacterStats characterStats; //캐릭터 스탯
    Vector2 inputVector;
    public Vector3 moveVector {get; private set;}
    private Rigidbody CharctorRb;
    float moveSpeed;
    AnimManager animManager;
    public enum status // 캐릭터 현재 상태
    {
        Default,
        burn,
        fainting,
        slowdown,
        freezing
    }
    
    void Awake()
    { 
        CharctorRb = GetComponent<Rigidbody>();
        moveSpeed = characterStats.speed;
        animManager = GetComponent<AnimManager>();
        Debug.Log(moveSpeed);
        Debug.Log(CharctorRb);
        Debug.Log(animManager);
    }
    void FixedUpdate() 
    {
        CharctorRb.linearVelocity = moveVector.normalized * moveSpeed;    
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
        moveVector = new Vector3(inputVector.x, 0f, inputVector.y);
        animManager.MoveAnim(inputVector);
        Debug.Log(moveVector);
    }
}

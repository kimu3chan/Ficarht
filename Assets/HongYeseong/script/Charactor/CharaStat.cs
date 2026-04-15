using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.InputSystem;

public class CharaStat : MonoBehaviour
{
    public CharacterStats characterStats; //캐릭터 스탯
    Vector2 inputVector;
    Vector3 moveVector;
    Rigidbody CharctorRb;
    float moveSpeed;
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
        Debug.Log(moveSpeed);
        Debug.Log(CharctorRb);
    }
    void Update() 
    {
        CharctorRb.linearVelocity = moveVector.normalized * moveSpeed;    
    }

    public void OnMove(InputValue value)
    { 
        inputVector = value.Get<Vector2>();
        moveVector = new Vector3(inputVector.x, 0f, inputVector.y);
    }
}

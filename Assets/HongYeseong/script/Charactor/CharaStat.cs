using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.InputSystem;

public class CharaStat : MonoBehaviour
{
    public CharacterStats characterStats; //캐릭터 스탯
    private Rigidbody CharctorRb;
    public float walkSpeed;
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
        walkSpeed = characterStats.speed;
        animManager = GetComponent<AnimManager>();
    }
}


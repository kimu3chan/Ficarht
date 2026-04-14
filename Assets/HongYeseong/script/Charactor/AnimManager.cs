using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.Animations;
using UnityEngine.InputSystem;

public class AnimManager : MonoBehaviour
{
    [Header("Anim List")]
    public List<string> AnimatorState = new List<string>();
    
    [Header("MoveAnim List")]
    public List<string> MoveAnimatorState = new List<string>();
    
    [Header("Key List")]
    public List<string> KeyList = new List<string>();
    
    [Header("Skill List")]
    public List<GameObject> SkillList = new List<GameObject>();
    
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    public void OnKey(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;
        Debug.Log(context.control.name);
        for (int i = 0; i < KeyList.Count; i++)
        {
            if (context.control.name == KeyList[i] && animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= 1.0f)        
            {
                
                Anim(i);
                Skill(i);
                break;
            }
        }
    }

    public void Anim(int i)
    {
            animator.Play(AnimatorState[i], 1, 0f);
    }
    public void Skill(int key)
    {
        GameObject skill = Instantiate(SkillList[key], transform.position, Quaternion.identity);    
        skill.SetActive(true);
    }

    public void MoveAnim(Vector2 inputVector)
    {
        if (inputVector.x > 0 && inputVector.y >= 0)
        {
            animator.Play(MoveAnimatorState[0], 0, 0f); // 오른쪽 이동
        }
        else if (inputVector.x < 0 && inputVector.y >= 0)
        {
            animator.Play(MoveAnimatorState[1], 0, 0f);// 왼쪽 이동
        }
        else if (inputVector.x == 0 && inputVector.y > 0)
        {
            animator.Play(MoveAnimatorState[2], 0, 0f);// 앞 이동
        }
        else if (inputVector.x == 0 && inputVector.y < 0)
        {
            animator.Play(MoveAnimatorState[3], 0, 0f);// 뒤 이동
        }
        else
        {
            animator.Play(MoveAnimatorState[4], 0, 0f);// idle
        }
    }
}

using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.Animations;
using UnityEngine.InputSystem;

public class AnimManager : MonoBehaviour
{
    [Header("Anim List")]
    public List<string> AnimatorState = new List<string>();
    
    [Header("Key List")]
    public List<string> KeyList = new List<string>();
    
    [Header("Skill List")]
    public List<GameObject> SkillList = new List<GameObject>();
    
    Animator animator;

    private bool isRightAttack;
    
    [SerializeField]
    CharactorType charactorType;
    public enum CharactorType
    {
        Mage,
        Paladin,
        Berserker,
        Bard
    }

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
        if (KeyList[i] == "leftButton")
        {
            animator.Play(AnimatorState[i], 1, 0f);
            isRightAttack = true;
            return;
        }
            animator.Play(AnimatorState[i], 1, 0f);
    }
    public void Skill(int key)
    {
        if (SkillList[key] == null)
            return;
        GameObject skill = Instantiate(SkillList[key], transform.position + SkillList[key].transform.localPosition, Quaternion.identity);    
        skill.SetActive(true);
    }
}

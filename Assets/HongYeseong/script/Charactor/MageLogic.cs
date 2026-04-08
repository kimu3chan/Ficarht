using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MageLogic : MonoBehaviour
{
    Animator animator;
    public List<RuntimeAnimatorController> animationList = new List<RuntimeAnimatorController>();
    public Dictionary<string, RuntimeAnimatorController> animationDic = new Dictionary<string, RuntimeAnimatorController>();
    AnimatorStateInfo stateInfo;
    void Awake()
    {
        animator = GetComponent<Animator>();
        for (int i = 0; i < animationList.Count; i++)
        {
            animationDic.Add(animationList[i].name, animationList[i]);
        }
        if(animator == null)
            Debug.LogError("animator is null");
        if(animationList == null)
            Debug.LogError("animationList is null");
    }
    void Update()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (Input.GetMouseButtonDown(0))
        {
            animator.runtimeAnimatorController = animationDic["MageBasicAttack"];
            animator.SetBool("right", !animator.GetBool("right"));
        }
        else if (!Input.anyKey && stateInfo.normalizedTime >= 0.95f)
        {
            animator.runtimeAnimatorController = animationDic["MageIdle"];
        }
    }
}

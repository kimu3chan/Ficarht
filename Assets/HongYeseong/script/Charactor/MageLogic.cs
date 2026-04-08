using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MageLogic : MonoBehaviour
{
    Animator animator;
    public List<RuntimeAnimatorController> animationList = new List<RuntimeAnimatorController>();
    public Dictionary<string, RuntimeAnimatorController> animationDic = new Dictionary<string, RuntimeAnimatorController>();
    AnimatorStateInfo stateInfo;
    CharaStat charaStat;
    void Awake()
    {
        animator = GetComponent<Animator>();
        charaStat = GetComponent<CharaStat>();
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
        if (charaStat.moveVector != Vector3.zero && Input.GetMouseButtonDown(0))
        {
            animator.runtimeAnimatorController = animationDic["MageRunBasicAttack"];
            animator.SetBool("right", !animator.GetBool("right"));
        }
        else if (Input.GetMouseButtonDown(0))
        {
            animator.runtimeAnimatorController = animationDic["MageBasicAttack"];
            animator.SetBool("right", !animator.GetBool("right"));
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            animator.runtimeAnimatorController = animationDic["BigAttak"];
        }
        else if (Input.GetKeyDown(KeyCode.E) && charaStat.moveVector != Vector3.zero)
        {
            animator.runtimeAnimatorController = animationDic["RunBigAttak1"];
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            animator.runtimeAnimatorController = animationDic["MageStrongAttack"];
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            animator.runtimeAnimatorController = animationDic["A"];
        }
        else if (!Input.anyKey && stateInfo.normalizedTime >= 0.99f)
        {
            animator.runtimeAnimatorController = animationDic["MageIdle"];
        }
    }
}

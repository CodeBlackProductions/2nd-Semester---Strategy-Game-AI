using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimState : MonoBehaviour
{
    private Animator animator;
    private UnitController parentUnit;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        parentUnit = transform.parent.GetComponent<UnitController>();
    }

    public void OnDamagePoint()
    {
        parentUnit.OnDealDamage();
    }

    public void OnDeath()
    {
        parentUnit.OnDeath();
    }

    public void OnStep()
    {
        parentUnit.OnStep();
    }

    public void OnAnimStart(AnimationEvent animationEvent)
    {
        animator.SetBool("IsInAnim", true);
        animator.SetInteger("CurrentAnim", animationEvent.intParameter);
    }

    public void OnAnimEnd(AnimationEvent animationEvent)
    {
        animator.SetBool("IsInAnim", false);
        animator.SetInteger("CurrentAnim",0);
    }

}

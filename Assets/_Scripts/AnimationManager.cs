using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationManager : MonoBehaviour
{
    public Animator Animator => animator;

    [SerializeField] string currentAnim;
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAnim(string animName, bool allowInterupt = false)
    {
        if(!allowInterupt && currentAnim == animName)return;

        animator.Play(animName);
        currentAnim = animName;
    }


}

﻿using UnityEngine;
using System;

public class CharacterAnimator : MonoBehaviour
{
    public Animator[] animators;

    public Action onLandAnim;
    public Action onJumpAnim;
    public Action onAttackAnim;
    public Action onDeathAnim;
    
    public void Run(bool value, Vector3 planarVelocity)
    {
        Bool("isRunning", value);
        for (int i = 0; i < animators.Length; i++)
        {
            if (animators[i] == null) continue;
            animators[i].SetFloat("x", planarVelocity.x);
            animators[i].SetFloat("z", planarVelocity.z);
        }
    }

    public void JumpLeft(float value)
    {
        Float("JumpLeft", value);
    }

    public void Jumping(bool value)
    {
        Bool("isJumping", value);
    }

    public void IsFalling(bool value)
    {
        Bool("isFalling", value);
    }

    public void Grounded(bool value)
    {
        Bool("isGrounded", value);
    }

    public void Attack()
    {
        Trigger("Attack");
        onAttackAnim?.Invoke();
    }

    public void Jump()
    {
        Trigger("Jump");
        onJumpAnim?.Invoke();
    }

    public void Land()
    {
        Trigger("Land");
        onLandAnim?.Invoke();
    }

    public void Death()
    {
        Trigger("Death");
        onDeathAnim?.Invoke();
    }

    public void Dash()
    {
        Trigger("Dash");
    }

    public void WallClimb(bool climb)
    {
        Bool("isWallclimbing", climb);
    }

    private void Trigger(string name)
    {
        for (int i = 0; i < animators.Length; i++)
        {
            if (animators[i] == null) continue;
            animators[i].SetTrigger(name);
        }
    }

    private void Bool(string name, bool value)
    {
        for (int i = 0; i < animators.Length; i++)
        {
            if (animators[i] == null) continue;
            animators[i].SetBool(name, value);
        }
    }
    private void Float(string name, float value)
    {
        for (int i = 0; i < animators.Length; i++)
        {
            if (animators[i] == null) continue;
            animators[i].SetFloat(name, value);
        }
    }
}

using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    public Animator[] animators;
    
    public void Run(bool value, Vector3 planarVelocity)
    {
        Bool("isRunning", value);
        for (int i = 0; i < animators.Length; i++)
        {
            animators[i].SetFloat("x", planarVelocity.x);
            animators[i].SetFloat("z", planarVelocity.z);
        }
    }

    public void Grounded(bool value)
    {
        Bool("isGrounded", value);
    }

    public void Attack()
    {
        Trigger("Attack");
    }

    public void Jump()
    {
        Trigger("Jump");
    }

    public void Land()
    {
        Trigger("Land");
    }

    public void Death()
    {
        Trigger("Death");
    }

    public void WallClimb(bool climb)
    {
        Bool("isWallclimbing", climb);
    }

    private void Trigger(string name)
    {
        for (int i = 0; i < animators.Length; i++)
        {
            animators[i].SetTrigger(name);
        }
    }

    private void Bool(string name, bool value)
    {
        for (int i = 0; i < animators.Length; i++)
        {
            animators[i].SetBool(name, value);
        }
    }
}

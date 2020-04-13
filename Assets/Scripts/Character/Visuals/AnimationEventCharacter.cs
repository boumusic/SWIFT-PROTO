using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventCharacter : MonoBehaviour
{
    public Character chara;

    public void DisableWeapon()
    {
        chara?.DisableWeapon();
    }
}

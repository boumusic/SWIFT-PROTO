using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (!instance) instance = FindObjectOfType<UIManager>();
            return instance;
        }
    }

    [Header("Components")]
    public Animator hitMarker;

    public void KillPerformed()
    {
        hitMarker.SetTrigger("In");
    }
}

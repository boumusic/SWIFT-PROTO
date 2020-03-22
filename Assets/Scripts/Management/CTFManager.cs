using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTFManager : MonoBehaviour
{
    private static CTFManager instance;
    public static CTFManager Instance
    {
        get
        {
            if (!instance) instance = FindObjectOfType<CTFManager>();
            return instance;
        }
    }

    [Header("Settings")]
    public int flagGoal = 3;
    public int minutes = 10;

}

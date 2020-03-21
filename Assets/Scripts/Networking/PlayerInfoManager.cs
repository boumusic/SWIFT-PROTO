using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoManager : MonoBehaviour
{
    public static PlayerInfoManager Instance;

    public string playerName;

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(this.gameObject);
    }

    public void ChangeName(string name)
    {
        playerName = name;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public bool debugLogMessages = false;
    

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
    public UIGeneralMessage generalMessage;

    public void KillPerformed()
    {
        hitMarker.SetTrigger("In");
    }

    public void LogMessage(string message)
    {
        message = message.ToUpper();
        if (debugLogMessages) Debug.Log(message);
        generalMessage.Message(message);
    }
}

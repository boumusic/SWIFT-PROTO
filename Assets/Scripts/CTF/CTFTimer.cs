using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CTFTimer : MonoBehaviour
{
    public TextMeshProUGUI text;
    private void Update()
    {
        text.text = CTFManager.Instance.timer.GetTimeLeftString();
    }
}

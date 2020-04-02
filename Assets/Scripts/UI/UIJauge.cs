using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIJauge : MonoBehaviour
{
    public Animator animator;
    public Image fill;

    [Header("Settings")]
    public float delayEndConsume;
    
    public void Init(ref System.Action start, ref System.Action end)
    {
        start += StartConsuming;
        end += EndConsuming;
    }

    public void UpdateJauge(float charge)
    {
        fill.fillAmount = charge;
    }

    public void StartConsuming()
    {
        animator.SetBool("started", true);
        if (waitEndCoroutine != null) StopCoroutine(waitEndCoroutine);
    }

    private Coroutine waitEndCoroutine;
    private IEnumerator WaitEndConsuming()
    {
        yield return new WaitForSeconds(delayEndConsume);
        animator.SetBool("started", false);
    }

    public void EndConsuming()
    {
        waitEndCoroutine = StartCoroutine(WaitEndConsuming());
    }
}

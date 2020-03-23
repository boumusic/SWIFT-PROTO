using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIKillFeed : MonoBehaviour
{
    public float lifeTime = 2f;
    public float smoothnessPos = 0.5f;
    public Animator animator;
    public TextMeshProUGUI textKiller;
    public TextMeshProUGUI textKilled;

    private Vector3 targetPos;
    private Vector3 currentVelPos;

    private void OnEnable()
    {
        StartCoroutine(LifeTime());
    }

    private void Update()
    {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPos, ref currentVelPos, smoothnessPos);
    }

    public void Init(Character killer, Character killed)
    {
        textKiller.text = killer.PlayerName;
        textKiller.color = killer.TeamColor;

        textKilled.text = killed.PlayerName;
        textKilled.color = killed.TeamColor;        
    }

    public void UpdatePosition(Vector3 pos)
    {
        targetPos = new Vector3(transform.localPosition.x, pos.y, 0);
    }

    private IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(lifeTime);
        animator.SetTrigger("Out");
    }

    public void Die()
    {
        UIManager.Instance.UnregisterKillFeed(this);
    }
}

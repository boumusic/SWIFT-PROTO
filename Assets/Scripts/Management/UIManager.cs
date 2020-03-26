using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public Player player;

    [Header("Components")]
    public Animator hitMarker;
    public UIGeneralMessage generalMessage;

    [Header("Kill Feed")]
    public GameObject prefabKillFeed;
    public Transform killFeedParent;
    public float killFeedSpacing = 100f;
    public float killFeedOffset = 50f;

    [Header("Dash")]
    public Image dashCd;
    public Image dashReset;
    private List<UIKillFeed> killFeeds = new List<UIKillFeed>();

    [Header("Pause")]
    public GameObject pause;
    private bool isPaused = false;
    public bool IsPaused => isPaused;

    [Header("Flag Status")]
    public GameObject flagStatus;

    public void FeedPlayer(Player p)
    {
        player = p;
    }

    private void Update()
    {
        PositionKillFeeds();
        UpdateFlagStatus();
        UpdateDashCooldown();
    }

    public void HitMarker()
    {
        hitMarker.SetTrigger("In");
    }

    public void LogMessage(string message)
    {
        message = message.ToUpper();
        if (debugLogMessages) Debug.Log(message);
        generalMessage.Message(message);
    }

    public void UpdateDashCooldown()
    {
        if (player)
        {
            dashCd.fillAmount = player.Character.DashCooldownProgress;
            dashReset.gameObject.SetActive(player.Character.ResetDash);

            float a = player.Character.CanDash ? 0.5f : 0.05f;
            Color col = new Color(1, 1, 1, a);
            dashReset.color = col;
            dashCd.color = col;
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pause.SetActive(isPaused);
        Cursor.visible = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void UpdateFlagStatus()
    {
        if(player)
        {
            flagStatus.SetActive(player.Character.HasFlag);
        }
    }

    #region KillFeed

    public void DisplayKillFeed(Character killer, Character killed)
    {
        GameObject newKillFeed = Instantiate(prefabKillFeed, killFeedParent);
        newKillFeed.transform.parent = killFeedParent;
        newKillFeed.transform.localPosition = new Vector3(newKillFeed.transform.localPosition.x, -800f, 0f);
        newKillFeed.gameObject.SetActive(true);
        UIKillFeed kf = newKillFeed.GetComponent<UIKillFeed>();
        kf.Init(killer, killed);
        killFeeds.Add(kf);
    }

    private void PositionKillFeeds()
    {
        for (int i = 0; i < killFeeds.Count; i++)
        {
            killFeeds[i].UpdatePosition(new Vector3(0f, -killFeedSpacing * i - killFeedOffset));
        }
    }

    public void UnregisterKillFeed(UIKillFeed kf)
    {
        killFeeds.Remove(kf);
        Destroy(kf.gameObject);
    }

    #endregion
}

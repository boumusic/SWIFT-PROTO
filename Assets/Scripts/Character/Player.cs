using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Generated;

public class Player : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Character character;
    [SerializeField] private TextMeshPro nameText;
    public PlayerScore score;

    [Header("Mouse")]
    public float sensitivity = 1f;
    public float scrollSpeed = 1f;
    public float attackSensitivity = 0.2f;

    public float CurrentSensitivity => character.IsAttacking ? attackSensitivity : sensitivity;
    public string PlayerName { get => playerName; }

    public bool cursor = true;
    private Vector2 mouse;

    [Header("Inputs")]
    public KeyCode jump = KeyCode.Space;
    public bool Tab => Input.GetKey(KeyCode.Tab);
    public KeyCode attack = KeyCode.Mouse0;
    public KeyCode dash = KeyCode.LeftShift;
    public KeyCode toggleTps = KeyCode.H;
    public KeyCode pause = KeyCode.Escape;
    
    [Header("Team")]
    public int debugTeamIndex = -1;
    public int TeamIndex => TeamManager.Instance.GetIndex(this);
    public Color TeamColor => TeamManager.Instance.GetTeamColor(TeamIndex);

    public Character Character { get => character; }

    private string playerName = "KRUSHER99";

    private void Awake()
    {
        Cursor.visible = cursor;
        Cursor.lockState = cursor ? CursorLockMode.None : CursorLockMode.Locked;

        InitializeCharacter();

        if (NetworkManager.Instance != null) return;

        SetPlayerName(playerName);

        if (debugTeamIndex >= 0)
        {
            TeamManager.Instance.JoinTeam(debugTeamIndex, this);
        }

        else
        {
            TeamManager.Instance.JoinSmallestTeam(this);
        }
    }

    private void InitializeCharacter()
    {
        character.Initialize(this);
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
        nameText.text = playerName;
    }

    private void Update()
    {
        if(!UIManager.Instance.IsPaused)
        {
            UpdateAxis();
            UpdateMouse();

            character.InputSpacebar(Input.GetKey(jump));
            if (Input.GetKeyDown(jump))
            {
                character.Jump();
            }

            if (Input.GetKeyDown(attack))
            {
                character.TryAttack();
            }

            if (Input.GetKeyDown(dash))
            {
                character.StartDash();
            }

            if (Input.GetKeyDown(toggleTps))
            {
                character.ToggleTPS();
            }
            
        }

        if (Input.GetKeyDown(pause) || Input.GetKeyDown(KeyCode.P))
        {
            character.InputAxis(Vector2.zero);
            UIManager.Instance.TogglePause();
        }

        sensitivity += Time.deltaTime * Input.GetAxis("Mouse Scroll") * scrollSpeed;
        sensitivity = Mathf.Clamp(sensitivity, 0, 50);
    }

    private void UpdateAxis()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 velocity = new Vector2(x, y);
        character.InputAxis(velocity);
    }

    private void UpdateMouse()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");
        mouse += new Vector2(x, y) * Time.deltaTime * 1000 * sensitivity;
        mouse.y = Mathf.Clamp(mouse.y, -90, 90);
        character.playerCamera.InputMouse(mouse);
    }
}

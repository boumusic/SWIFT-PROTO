﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Character character;
    [SerializeField] private TextMeshPro nameText;

    [Header("Mouse")]
    public float sensitivity = 1f;
    public float attackSensitivity = 0.2f;

    public float CurrentSensitivity => character.IsAttacking ? attackSensitivity : sensitivity;
    public bool cursor = true;
    private Vector2 mouse;

    [Header("Inputs")]
    public KeyCode jump = KeyCode.Space;
    public KeyCode attack = KeyCode.Mouse0;
    public KeyCode toggleTps = KeyCode.H;

    private string playerName = "XxkillerxX";

    private void Start()
    {
        SetPlayerName(playerName);
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
        nameText.text = playerName;
    }
    
    private void Update()
    {
        Cursor.visible = cursor;
        Cursor.lockState =  cursor ? CursorLockMode.None : CursorLockMode.Locked;
        UpdateAxis();
        UpdateMouse();

        character.InputSpacebar(Input.GetKey(jump));

        if(Input.GetKeyDown(jump))
        {
            character.Jump();
        }

        if (Input.GetKeyDown(attack))
        {
            character.TryAttack();
        }

        if (Input.GetKeyDown(toggleTps))
        {
            character.ToggleTPS();
        }
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
        mouse += new Vector2(x, y) * Time.deltaTime * 1000 * CurrentSensitivity;
        mouse.y = Mathf.Clamp(mouse.y, -90, 90);
        character.playerCamera.InputMouse(mouse);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Character character;

    [Header("Mouse")]
    public float sensitivity = 1f;
    public bool cursor = true;
    private Vector2 mouse;
    
    private void Update()
    {
        Cursor.visible = cursor;
        UpdateAxis();
        UpdateMouse();

        character.InputSpacebar(Input.GetKey(KeyCode.Space));

        if(Input.GetKeyDown(KeyCode.Space))
        {
            character.Jump();
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
        mouse += new Vector2(x, y) * Time.deltaTime * 1000 * sensitivity;
        mouse.y = Mathf.Clamp(mouse.y, -90, 90);
        character.playerCamera.InputMouse(mouse);
    }
}

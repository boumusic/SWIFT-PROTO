using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine.SceneManagement;

public class BasicCube : BasicCubeBehavior
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && networkObject.IsServer)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}

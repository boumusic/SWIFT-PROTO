using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Generated;

public class NetworkedGameManager : MonoBehaviour
{
    private void Start()
    {
        NetworkManager.Instance.InstantiateNetworkedPlayer();
    }
}

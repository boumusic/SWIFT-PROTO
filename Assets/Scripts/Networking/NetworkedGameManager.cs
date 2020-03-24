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
        ReplaceFlags();

        NetworkManager.Instance.InstantiateNetworkedPlayer();
    }

    void ReplaceFlags()
    {
        Zone[] allFlagZones = FindObjectsOfType<Zone>();

        for (int i = 0; i < allFlagZones.Length; i++)
        {
            if (NetworkManager.Instance.IsServer)
            {
                NetworkedFlagBehavior flag = NetworkManager.Instance.InstantiateNetworkedFlag(position: allFlagZones[i].transform.position, rotation: allFlagZones[i].transform.rotation);
                flag.networkObject.teamIndex = allFlagZones[i].teamIndex;
            }
        }
    }
}

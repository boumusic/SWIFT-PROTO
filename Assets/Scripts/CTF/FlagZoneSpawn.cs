using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagZoneSpawn : MonoBehaviour
{
    public int teamIndex;
    public float radius = 10f;
    public FlagZoneType type;

    private void OnDrawGizmos()
    {
        
        if(teamIndex == 0)
        {
            Gizmos.color = new Color(1, 0.23f, 0.37f, 1f);
        }

        else
        {
            Gizmos.color = new Color(0.14f, 0.65f, 1f, 1f);
        }

        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

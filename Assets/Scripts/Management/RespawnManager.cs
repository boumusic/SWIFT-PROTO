using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn
{
    public Character character;
    private float time;
    private float progress;

    public Respawn(Character chara, float time)
    {
        this.time = time;
        this.character = chara;
    }

    public void Update()
    {
        progress += Time.deltaTime;
        if(progress >= time)
        {
            RespawnManager.Instance.Respawn(this);
        }
    }
}

public class RespawnManager : MonoBehaviour
{
    private static RespawnManager instance;
    public static RespawnManager Instance
    {
        get
        {
            if (!instance) instance = FindObjectOfType<RespawnManager>();
            return instance;
        }
    }

    [Header("Respawn")]
    public float respawnTime = 2f;
    private List<Respawn> ongoingRespawns = new List<Respawn>();

    private void Update()
    {
        for (int i = 0; i < ongoingRespawns.Count; i++)
        {
            ongoingRespawns[i].Update();
        }
    }

    public void Death(Character character)
    {
        Respawn respawn = new Respawn(character, respawnTime);
        ongoingRespawns.Add(respawn);
    }

    public void Respawn(Respawn respawn)
    {
        ongoingRespawns.Remove(respawn);
        respawn.character.Respawn(RespawnPosition(respawn.character));
    }

    private Vector3 RespawnPosition(Character character)
    {
        return Vector3.zero;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerEntity<T> : InstantiableEntity where T : PowerEntityStats
{
    public T stats;
    private float currentLife = 0;
    private bool hasLifetime => stats.lifetime != 0;
    public NetworkedPlayer owner;

    public void OnEnable()
    {
        Initialize();
    }

    public virtual void Update()
    {
        if (stats == null) return;
        if(hasLifetime)
        {
            currentLife += Time.deltaTime;
            if(currentLife >= stats.lifetime)
            {
                Die();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        NetworkedPlayer p = collision.gameObject.GetComponentInParent<NetworkedPlayer>();
        if (p != null)
        {
            if(p.teamIndex != owner.teamIndex)
            {
                CollidedWithEnemy(p);
            }

            else
            {
                CollidedWithAlly(p);
            }
        }

        else
        {
            CollidedWithGeometry(collision.collider);
        }
    }

    public virtual void Initialize()
    {
        currentLife = 0f;
    }

    public virtual void CollidedWithGeometry(Collider collider)
    {

    }

    public virtual void CollidedWithAlly(NetworkedPlayer ally)
    {

    }

    public virtual void CollidedWithEnemy(NetworkedPlayer enemy)
    {

    }

    public virtual void Die()
    {
        gameObject.SetActive(false);
    }
}

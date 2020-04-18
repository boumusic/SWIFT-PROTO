using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public ProjectileStats stats;
    private float currentFallSpeed = 0f;
    private float currentLife = 0f;

    private void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        currentLife = 0f;
        currentFallSpeed = 0f;
    }

    public void Update()
    {
        if (stats == null) return;
        transform.position += transform.forward * Time.deltaTime * stats.speed;
        currentFallSpeed += stats.gravity;
        currentFallSpeed = Mathf.Clamp(currentFallSpeed, - stats.maxFallSpeed, stats.maxFallSpeed);
        transform.position -= Vector3.up * Time.deltaTime * currentFallSpeed;

        currentLife += Time.deltaTime / stats.lifetime;

        if(currentLife > 1)
        {
            gameObject.SetActive(false);
        }
    }
}

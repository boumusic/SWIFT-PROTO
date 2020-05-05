using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : PowerEntity<ProjectileStats>
{
    private float currentFallSpeed = 0f;
    
    public override void Initialize()
    {
        base.Initialize();
        currentFallSpeed = 0f;
    }

    public override void Update()
    {
        base.Update();
        transform.position += transform.forward * Time.deltaTime * stats.speed;
        currentFallSpeed += stats.gravity;
        currentFallSpeed = Mathf.Clamp(currentFallSpeed, - stats.maxFallSpeed, stats.maxFallSpeed);
        transform.position -= Vector3.up * Time.deltaTime * currentFallSpeed;        
    }
}

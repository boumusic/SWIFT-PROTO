using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster : MonoBehaviour
{
    [Header("Boost Settings")]
    public SpeedBoostSettings boost;

    [Header("Components")]
    public GameObject archTop;
    public GameObject archBottom;
    public GameObject portal;
    public GameObject[] archSides;
    public CapsuleCollider coll;

    [Header("Settings")]
    public float radius = 1;
    public float height = 0f;
    public bool top = true;
    public bool bottom = true;

    private void OnDrawGizmos()
    {
        ApplyMesh();
    }

    private void Start()
    {
        ApplyMesh();
    }

    private void ApplyMesh()
    {
        archTop?.SetActive(top);
        archBottom?.SetActive(bottom);
        archTop.transform.localScale = new Vector3(radius, radius, radius);
        archBottom.transform.localScale = new Vector3(radius, radius, radius);
        archTop.transform.localPosition = new Vector3(0, height, 0);
        for (int i = 0; i < archSides.Length; i++)
        {
            float mul = i == 0 ? -1 : 1;
            archSides[i].transform.localScale = new Vector3(radius, height * 5, radius);
            archSides[i].transform.localPosition = new Vector3(mul * radius, 0, 0);
        }

        portal.transform.localPosition = new Vector3(0, height / 2, 0);
        portal.transform.localScale = new Vector3(radius * 2, height + 2 * radius, 1);

        coll.radius = radius;
        coll.height = height + radius * 2;
        coll.center = new Vector3(0, height / 2, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        Character chara;
        if(other.TryGetComponent(out chara))
        {
            chara.ApplyBoosterBoost(boost);
            Debug.Log("Chara Enter booster");
        }
    }
}

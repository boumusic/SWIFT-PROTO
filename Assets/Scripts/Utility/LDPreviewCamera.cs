using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LDPreviewCamera : MonoBehaviour
{
    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam.enabled = false;
    }
}

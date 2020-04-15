using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LDPreviewCamera : MonoBehaviour
{
    public Camera cam;
    public GameObject postProcess;
    /*public GameObject essentials;
    public GameObject player;
    
    private void Awake()
    {
        player.SetActive(true);
        essentials.SetActive(true);
    }
    */
    private void Start()
    {
        cam.enabled = false;
        postProcess.SetActive(false);
    }
    /*
    [ContextMenu("Initialize")]
    private void Initialize()
    {
        essentials = GameObject.Find("ESSENTIALS");
        essentials.SetActive(false);
        player = GameObject.Find("Player");
        player.SetActive(false);
    }
    */
}

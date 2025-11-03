using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerCam : MonoBehaviour
{
    public Transform player;
    public float distance = 5f;
    public float height = 2f;
    public float angle = 0f;

    [Space(10)]
    public SceneBuildingManager sbManager;

    void Start()
    {
        
    }
    
    void Update()
    {
        FollowPlayer();
        ChangeCamPos();
    }

    void FollowPlayer()
    {
        if (player == null) return;

        transform.position = player.position + Quaternion.Euler(0, angle, 0) * (player.forward * -distance + player.up * height);
        transform.LookAt(player);
    }

    void ChangeCamPos()
    {
        if (Input.GetKey(KeyCode.W))
        {
            distance -= 0.1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            distance += 0.1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            angle += 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            angle -= 1;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            height -= 0.1f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            height += 0.1f;
        }

        if (sbManager.isPreviewFollowedObj)
            sbManager.AdjustFollowObjDirection(distance, height, angle);
    }
}

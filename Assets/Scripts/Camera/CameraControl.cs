using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("Camera scripts")]
    public FlyCamControl flyCam;
    public FollowPlayerCam followCam;    

    [Space(5)]
    public CameraControlType cameraType;

    [Header("Fixed positions")]
    public Transform[] pos;
    
    void Start()
    {
        SetCameraType();
    }

    
    void Update()
    {
        //SwitchCamera();
    }

    public void FollowObject(Transform obj, float dis, float height,  float angle)
    {
        if (obj == null) return;

        followCam.player = obj;
        followCam.distance = dis;
        followCam.height = height;
        followCam.angle = angle;

        cameraType = CameraControlType.FollowPlayer;
        SetCameraType();
    }

    public void SetCameraType()
    {
        if (flyCam != null) flyCam.enabled = cameraType == CameraControlType.FlyCamera;
        if (followCam != null) followCam.enabled = cameraType == CameraControlType.FollowPlayer;
    }

    void SwitchCamera()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (int.TryParse(cameraType.GetDescription(), out int nextType))
                cameraType = (CameraControlType)nextType;

            SetCameraType();
        }
    }

    public void GoToFixedPos(int index)
    {
        if (pos.Length > 0 && index >=0 && index < pos.Length)
        {
            Camera.main.transform.position = pos[index].position;
            Camera.main.transform.localEulerAngles = pos[index].localEulerAngles;
        }
    }
}

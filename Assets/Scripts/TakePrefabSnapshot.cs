using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakePrefabSnapshot : MonoBehaviour
{
    [Header("Settings")]
    public Vector3 snOffset = new Vector3(0, -1, 1);
    public Vector3 snRotation = new Vector3(0, 180, 0);
    public Vector3 snScale = new Vector3(1, 1, 1);
    public Vector2 snSize = new Vector2(256, 256);
    public string saveFolder = "\\Resources\\Images\\PrefabImages";

    [Header("Data")]
    public GameObject[] prefabs;

    [Header("Debug")]
    public bool takeSnapShot;


    void Update()
    {
        TakeSnapshot();
    }

    void TakeSnapshot()
    {
        if (takeSnapShot)
        {
            takeSnapShot = false;

            if (prefabs.Length == 0) return;

            var s = SnapshotCamera.MakeSnapshotCamera();
            s.defaultPositionOffset = snOffset;
            s.defaultRotation = snRotation;
            s.defaultScale = snScale;

            int count = 0;
            for (int i = 0; i < prefabs.Length; i++)
            {
                var tex = s.TakePrefabSnapshot(prefabs[i], (int)snSize.x, (int)snSize.y);
                if (tex != null)
                {
                    SnapshotCamera.SavePNG(tex, prefabs[i].name, Application.dataPath + saveFolder);
                    count++;
                }
            }
            Debug.Log("Done! Create " + count + " files");
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;

[Serializable]
public class ListViewWalkingPeopleItemDescription
{
    [SerializeField]
    public int WalkingPeopleIndex;

    [SerializeField]
    public string Note;

    [SerializeField]
    public bool HavePath;

    [SerializeField]
    public string PathGUID;

    [SerializeField]
    public bool IsShowed;

    [SerializeField]
    public ObservableList<Vector3> Points;

    [SerializeField]
    public bool CreateAlongPath;

    [SerializeField]
    public bool IsLoop;

    [SerializeField]
    public bool IsReverse;

    [SerializeField]
    public float Density;

    [SerializeField]
    public float TimeInterval;

    [SerializeField]
    public bool IsPreviewed;

}

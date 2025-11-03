using System;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;

[Serializable]
public class ListViewSoundItemDescription
{
    [SerializeField]
    public int SoundIndex;

    [SerializeField]
    public string SoundNote;

    [SerializeField]
    public bool HaveSoundPos;

    [SerializeField]
    public Vector3 SoundPosition;

    [SerializeField]
    public float SoundDistance;

    [SerializeField]
    public SoundControlType SoundType;

    [SerializeField]
    public ObservableList<string> SoundFiles;
}

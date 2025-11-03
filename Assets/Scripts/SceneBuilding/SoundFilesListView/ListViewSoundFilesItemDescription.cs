using System;
using UnityEngine;

[Serializable]
public class ListViewSoundFilesItemDescription
{
    [SerializeField]
    public int SoundFileIndex;

    [SerializeField]
    public string SoundFilePath;

    [SerializeField]
    public bool IsPlaying;
}

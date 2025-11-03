using System;
using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;

public class ListViewSoundFiles : ListViewCustom<ListViewSoundFilesComponent, ListViewSoundFilesItemDescription>
{
    readonly Comparison<ListViewSoundFilesItemDescription> itemsComparison = (x, y) => UtilitiesCompare.Compare(x.SoundFileIndex, y.SoundFileIndex);

    bool isListViewSoundFilesInited;

    public ListViewSound soundListView;
    public int belongToSoundIndex;

    /// <summary>
    /// Set items comparison.
    /// </summary>
    public override void Init()
    {
        if (isListViewSoundFilesInited)
        {
            return;
        }

        isListViewSoundFilesInited = true;

        base.Init();
        DataSource.Comparison = itemsComparison;
    }

    public void DeleteSoundFileRecord(int soundFileIndex, string fileName)
    {
        soundListView.DeleteSoundFileRecord(belongToSoundIndex, soundFileIndex, fileName);
    }

    public void PlayOrPauseMusicFile(int soundFileIndex)
    {
        soundListView.PlayOrPauseMusicFile(belongToSoundIndex, soundFileIndex);
    }
}

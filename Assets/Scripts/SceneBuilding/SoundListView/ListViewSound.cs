using System;
using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;

public class ListViewSound : ListViewCustom<ListViewSoundComponent, ListViewSoundItemDescription>
{
    readonly Comparison<ListViewSoundItemDescription> itemsComparison = (x, y) => UtilitiesCompare.Compare(x.SoundIndex, y.SoundIndex);

    bool isListViewSoundInited;

    public ObservableList<ListViewSoundItemDescription> data = new ObservableList<ListViewSoundItemDescription>();

    public SceneBuildingManager sceneBuildManager;
    public ListViewSoundFiles listViewSoundFiles;
    public int currentPlay_soundIndex = -1;
    public int currentPlay_soundFileIndex = -1;

    /// <summary>
    /// Set items comparison.
    /// </summary>
    public override void Init()
    {
        if (isListViewSoundInited)
        {
            return;
        }

        isListViewSoundInited = true;

        base.Init();
        DataSource.Comparison = itemsComparison;
    }

    public void LoadDataFromList(ObservableList<ListViewSoundItemDescription> list)
    {
        data = list;

        this.DataSource = data;
    }

    public void AddNewSound()
    {
        for (int i = 0; i < data.Count; i++)
        {
            data[i].SoundIndex = i;
        }

        data.Add(new ListViewSoundItemDescription() { SoundIndex = data.Count });

        this.DataSource = data;
    }

    public void DeleteSoundRecord(int index)
    {   
        StartCoroutine(DeleteSoundRecordAsync(index));

        //var item = data.Find(i => i.SoundIndex == index);
        //if (item != null)
        //{
        //    data.Remove(item);

        //    for (int i = 0; i < data.Count; i++)
        //    {
        //        data[i].SoundIndex = i;
        //    }

        //    this.DataSource = data;
        //}
    }

    IEnumerator DeleteSoundRecordAsync(int index)
    {
        var item = data.Find(i => i.SoundIndex == index);
        if (item != null)
        {
            string msg = string.Format("Bạn có muốn xóa nguồn âm thanh \"{0}\" không?", item.SoundNote);
            sceneBuildManager.ShowConfirmDialog(msg);

            while (sceneBuildManager.confirmStatus == ConfirmDialogStatus.Waiting)
            {
                yield return null;
            }

            if (sceneBuildManager.confirmStatus == ConfirmDialogStatus.ConfirmYes)
            {
                data.Remove(item);

                for (int i = 0; i < data.Count; i++)
                {
                    data[i].SoundIndex = i;
                }

                this.DataSource = data;
            }
        }
    }

    public void GoToSoundPos(Vector3 pos)
    {
        Camera.main.transform.position = pos;
    }

    public void GetSoundPos(out Vector3 pos)
    {
        pos = Camera.main.transform.position;
    }

    public void ShowMusicSelectDialog(int index)
    {
        if (sceneBuildManager != null) sceneBuildManager.ShowMusicSelectFileDialog(index);
    }

    public void MusicFileSelectedCompleted(int index, string filePath)
    {
        var item = data.Find(i => i.SoundIndex == index);
        if (item != null)
        {
            item.SoundFiles.Add(filePath);

            this.DataSource = data;
        }
        
    }

    public void DeleteSoundFileRecord(int soundIndex, int soundFileIndex, string fileName)
    {
        StartCoroutine(DeleteSoundFileRecordAsync(soundIndex, soundFileIndex, fileName));

        //var soundItem = data.Find(i => i.SoundIndex == soundIndex);
        //if (soundItem != null)
        //{
        //    soundItem.SoundFiles.RemoveAt(soundFileIndex);

        //    this.DataSource = data;
        //}
    }

    IEnumerator DeleteSoundFileRecordAsync(int soundIndex, int soundFileIndex, string fileName)
    {
        var soundItem = data.Find(i => i.SoundIndex == soundIndex);
        if (soundItem != null)
        {
            string msg = string.Format("Bạn có muốn xóa file nhạc \"{0}\" khỏi danh sách không?", fileName);
            sceneBuildManager.ShowConfirmDialog(msg);

            while (sceneBuildManager.confirmStatus == ConfirmDialogStatus.Waiting)
            {
                yield return null;
            }

            if (sceneBuildManager.confirmStatus == ConfirmDialogStatus.ConfirmYes)
            {
                soundItem.SoundFiles.RemoveAt(soundFileIndex);

                this.DataSource = data;
            }
            
        }
    }

    public void PlayOrPauseMusicFile(int soundIndex, int soundFileIndex)
    {
        //Play new file
        if (currentPlay_soundIndex != soundIndex || currentPlay_soundFileIndex != soundFileIndex)
        {
            currentPlay_soundIndex = soundIndex;
            currentPlay_soundFileIndex = soundFileIndex;
            this.DataSource = data;//Reload

            var filePath = string.Format("{0}\\{1}\\{2}", Application.dataPath, Helper.Sounds_DataFolder, data[soundIndex].SoundFiles[soundFileIndex]);
            StartCoroutine(sceneBuildManager.PlayMusicFile(filePath));
        }
        else//Pause playing file
        {
            currentPlay_soundIndex = -1;
            currentPlay_soundFileIndex = -1;
            this.DataSource = data;//Reload

            sceneBuildManager.PausePlayingMusicFile();
        }
    }
}

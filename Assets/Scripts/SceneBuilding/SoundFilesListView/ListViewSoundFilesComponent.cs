using System.Collections;
using System.Collections.Generic;
using System.IO;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

public class ListViewSoundFilesComponent : ListViewItem, IViewData<ListViewSoundFilesItemDescription>
{
    [SerializeField]
    public Text SoundName;

    [SerializeField]
    public Button BtnPlay;
    [SerializeField]
    public Button BtnPause;

    [Space(10)]
    public ListViewSoundFiles mainListView;
    public ListViewSoundFilesItemDescription data;

    public void SetData(ListViewSoundFilesItemDescription item)
    {
        if (item == null)
        {
            
        }
        else
        {
            data = item;

            var path = string.Format("{0}\\{1}\\{2}", Application.dataPath, Helper.Sounds_DataFolder, data.SoundFilePath);
            SoundName.text = Path.GetFileName(path);
            if (!File.Exists(path))
                SoundName.text = "KHÔNG TÌM THẤY FILE NHẠC";

            BtnPlay.gameObject.SetActive(!data.IsPlaying);
            BtnPause.gameObject.SetActive(data.IsPlaying);
        }
    }

    public void DeleteRecord()
    {
        var path = string.Format("{0}\\{1}\\{2}", Application.dataPath, Helper.Sounds_DataFolder, data.SoundFilePath);
        var fileName = Path.GetFileName(path);
        mainListView.DeleteSoundFileRecord(data.SoundFileIndex, fileName);
    }

    public void PlayOrPauseMusicFile()
    {
        mainListView.PlayOrPauseMusicFile(data.SoundFileIndex);
    }
}

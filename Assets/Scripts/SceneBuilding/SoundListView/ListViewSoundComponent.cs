using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

public class ListViewSoundComponent : ListViewItem, IViewData<ListViewSoundItemDescription>
{
    [SerializeField]
    public InputField TxtNote;

    [SerializeField]
    public InputField TxtDistance;

    [SerializeField]
    public Image HaveSoundPosIcon;

    [SerializeField]
    public Image NotHaveSoundPosIcon;

    [SerializeField]
    public ComboboxString CboSoundType;


    [Space(10)]
    public ListViewSound mainListView;
    public ListViewSoundItemDescription data;
    public ListViewSoundFiles soundFilesListView;

    public void SetData(ListViewSoundItemDescription item)
    {
        if (item == null)
        {
            HaveSoundPosIcon.gameObject.SetActive(false);
            NotHaveSoundPosIcon.gameObject.SetActive(true);
        }
        else
        {
            data = item;

            TxtNote.text = data.SoundNote;
            TxtDistance.text = data.SoundDistance.ToString();
            HaveSoundPosIcon.gameObject.SetActive(data.HaveSoundPos);
            NotHaveSoundPosIcon.gameObject.SetActive(!data.HaveSoundPos);

            CboSoundType.ListView.DataSource = new ObservableList<string>() { "Nhạc", "Đường phố", "Nói chuyện", "Khác"};
            CboSoundType.ListView.SelectedIndex = (int)data.SoundType - 1;

            if (data.SoundFiles == null) data.SoundFiles = new ObservableList<string>();
            var lst = new ObservableList<ListViewSoundFilesItemDescription>();
            for (int i = 0; i < data.SoundFiles.Count; i++)
            {
                bool isPlaying = mainListView.currentPlay_soundIndex == data.SoundIndex && mainListView.currentPlay_soundFileIndex == i;
                lst.Add(new ListViewSoundFilesItemDescription
                            {
                                SoundFileIndex = i,
                                SoundFilePath = data.SoundFiles[i],
                                IsPlaying = isPlaying
                });
            }
            soundFilesListView.DataSource = lst;
            soundFilesListView.belongToSoundIndex = data.SoundIndex;
        }
    }

    public void TxtNoteChange(string value)
    {
        data.SoundNote = value;
    }

    public void TxtDistanceChange(string value)
    {
        //float.TryParse(value, out data.SoundDistance);

        if (!float.TryParse(value, out data.SoundDistance))
            TxtDistance.text = string.IsNullOrEmpty(value) ? "0" : value.Substring(0, value.Length - 1);
    }

    public void CboSoundTypeChange()
    {
        data.SoundType = (SoundControlType)(CboSoundType.ListView.SelectedIndex + 1);
    }

    public void GoToSoundPos()
    {
        if (data != null && data.HaveSoundPos)
            mainListView.GoToSoundPos(data.SoundPosition);
    }

    public void GetSoundPos()
    {
        if (data != null)
        {
            mainListView.GetSoundPos(out data.SoundPosition);
            data.HaveSoundPos = true;

            HaveSoundPosIcon.gameObject.SetActive(data.HaveSoundPos);
            NotHaveSoundPosIcon.gameObject.SetActive(!data.HaveSoundPos);
        }
    }

    public void DeleteRecord()
    {
        mainListView.DeleteSoundRecord(data.SoundIndex);
    }

    public void ShowMusicSelectDialog()
    {
        mainListView.ShowMusicSelectDialog(data.SoundIndex);
    }


}

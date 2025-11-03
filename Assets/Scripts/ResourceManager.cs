using NAudio.Wave;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    static GameObject m_instance;

    public GameObject startLoader;
    public GameObject startLoader_Root;
    public Slider startLoader_Slider;
    public Text startLoader_Text;

    public float time;
    public List<ResourceSound> sounds;
    public int total;

    Task mp3Task;
    public bool processUI;
    public bool startLoading;
    public bool taskCompleted;
    public bool converted;

    [Header("Scenario passer")]
    public string file;
    public BtnScenario.ScenarioLoadType mode;

    void Start()
    {
        if (m_instance == null)
        {
            m_instance = this.gameObject;
            DontDestroyOnLoad(this.gameObject);

            var path = Application.dataPath + Helper.Sounds_DataFolder;
            mp3Task = Task.Run(() => { LoadFilesToWavs(path); });
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        time += Time.deltaTime;
        taskCompleted = mp3Task.IsCompleted;

        if (taskCompleted && !converted)
        {
            converted = true;
            ConvertWavsToMp3();
        }


        LoadingUI();
    }

    void LoadFilesToWavs(string folderPath)
    {
        if (Directory.Exists(folderPath))
        {
            startLoading = true;
            sounds = new List<ResourceSound>();

            var files = Directory.GetFiles(folderPath, "*.mp3", SearchOption.AllDirectories);
            total = files.Length;

            foreach (var filePath in files)
            {
                sounds.Add(LoadFile(filePath));
            }
        }

    }

    void LoadingUI()
    {
        if (startLoading && startLoader_Root != null && !startLoader_Root.activeSelf)
        {
            processUI = true;
            startLoader.SetActive(true);
            startLoader_Root.SetActive(true);
            startLoader_Slider.value = 0;
            startLoader_Text.text = string.Format("LOADING {0}%", 0);
        }

        if (startLoading && processUI)
        {
            if (!taskCompleted)
            {
                float startP = (float)sounds.Count / total;
                float endP = Mathf.Clamp((float)(sounds.Count + 1) / total, 0f, 100f);

                startLoader_Slider.value = Mathf.Clamp(startLoader_Slider.value + 0.0005f, startP, endP);
                startLoader_Text.text = string.Format("LOADING {0}%", Mathf.RoundToInt(startLoader_Slider.value * 100f));
            }
            else
            {
                startLoader.SetActive(false);
                startLoader_Root.SetActive(false);

                processUI = false;
            }
        }
    }        

    ResourceSound LoadFile(string filePath)
    {
        var data = File.ReadAllBytes(filePath);
        var name = Path.GetFileName(filePath);

        // Load the data into a stream
        MemoryStream mp3stream = new MemoryStream(data);
        // Convert the data in the stream to WAV format
        Mp3FileReader mp3audio = new Mp3FileReader(mp3stream);
        WaveStream waveStream = WaveFormatConversionStream.CreatePcmStream(mp3audio);
        // Convert to WAV data
        WAV wav = new WAV(NAudioPlayer.AudioMemStream(waveStream).ToArray());

        return new ResourceSound
                {
                    path = filePath,
                    wav = wav
                };
    }

    void ConvertWavsToMp3()
    {
        for (int i = 0; i < sounds.Count; i++)
        {
            AudioClip audioClip = AudioClip.Create(Path.GetFileName(sounds[i].path), sounds[i].wav.SampleCount, 1, sounds[i].wav.Frequency, false);
            audioClip.SetData(sounds[i].wav.LeftChannel, 0);

            sounds[i].clip = audioClip;
        }
    }

    //PUBLIC METHODS
    public AudioClip GetClipByPath(string path)
    {
        if (sounds == null || sounds.Count == 0 || string.IsNullOrEmpty(path)) return null;

        var rs = sounds.FirstOrDefault(x => x.path == path);
        return rs != null ? rs.clip : null;
    }
}

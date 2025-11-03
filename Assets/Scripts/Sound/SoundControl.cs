using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SoundControl : MonoBehaviour
{
    AudioSource audioSource;

    [Header("Settings")]
    public bool playOnAwake;
    public SoundControlType soundType;
    public float currentVolume;
    public float soundMaxDistance = 25f;

    [Space(5)]
    public AudioClip[] clips;
    public List<int> unPlayIndex = new List<int>();//For shuffle playing type
    public AudioPlayType playType;
    public int timeBetweenClip = 5;    
    public bool canGetClipToPlay;

    [Header("Debug")]
    public bool isPlaying;
    public int currentPlayIndex;


    void Awake()
    {
        var sm = FindObjectOfType<SoundManager>();
        if (sm != null) sm.AddNewSoundControl(this);
    }

    void Start()
    {
        if (GetComponent<AudioSource>() == null)
            gameObject.AddComponent<AudioSource>();

        audioSource = GetComponent<AudioSource>(); 
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.maxDistance = soundMaxDistance;
        audioSource.spatialBlend = 1;//Make sound full 3D
        audioSource.volume = currentVolume;

        canGetClipToPlay = false;
        isPlaying = false;
        currentPlayIndex = -1;

        //init unplay list for shuffle
        if (clips != null)
            for (int i = 0; i < clips.Length; unPlayIndex.Add(i++))

        if (playOnAwake)
            StartCoroutine(PlayAudio(0));
    }

    void Update()
    {
        GetAudioToPlay();
        CheckIfPlayNextAudio();
        SoundTuning();
    }

    void GetAudioToPlay()
    {
        if (canGetClipToPlay && !isPlaying && clips.Length > 0)
        {
            canGetClipToPlay = false;

            switch (playType)
            {
                case AudioPlayType.OneShot_First:
                    if (currentPlayIndex == -1) //Not play before, play first clip
                        currentPlayIndex = 0;
                    else //Stop if play before
                        currentPlayIndex = clips.Length;
                    break;
                case AudioPlayType.OneShot_Random:
                    if (currentPlayIndex == -1) // Not play before, get random clip to play
                        currentPlayIndex = Random.Range(0, clips.Length);
                    else //Stop if play before
                        currentPlayIndex = clips.Length;
                    break;
                case AudioPlayType.All_Loop_OneTime:
                    currentPlayIndex = Mathf.Clamp(currentPlayIndex + 1, 0, clips.Length);                    
                    break;
                case AudioPlayType.All_Loop_Repeat:
                    currentPlayIndex = Mathf.Clamp(currentPlayIndex + 1, 0, clips.Length);
                    if (currentPlayIndex == clips.Length) //Did play all clips one time, repeat from first clip
                        currentPlayIndex = 0;
                    break;
                case AudioPlayType.All_Shuffle_Forever:
                    if (unPlayIndex.Count == 0) for (int i = 0; i < clips.Length; unPlayIndex.Add(i++)) ;
                    int rnd = Random.Range(0, unPlayIndex.Count);

                    currentPlayIndex = unPlayIndex[rnd];
                    unPlayIndex.Remove(currentPlayIndex);
                    break;
                default:
                    currentPlayIndex = -1;
                    break;
            }

            //Play clip
            if (currentPlayIndex == Mathf.Clamp(currentPlayIndex, 0, clips.Length - 1))
            {
                audioSource.PlayOneShot(clips[currentPlayIndex]);
                isPlaying = true;
            }
        }
    }

    void CheckIfPlayNextAudio()
    {
        //Check if done current clip
        if (isPlaying && !audioSource.isPlaying)
        {
            isPlaying = false;
            StartCoroutine(PlayAudio(timeBetweenClip));
        }
    }


    public IEnumerator PlayAudio(float timeWait)
    {
        if (audioSource.isPlaying) audioSource.Stop();

        yield return new WaitForSeconds(timeWait);
        
        canGetClipToPlay = true;
    }


    void SoundTuning()
    {
        audioSource.volume = currentVolume;
        audioSource.maxDistance = soundMaxDistance;
    }
}

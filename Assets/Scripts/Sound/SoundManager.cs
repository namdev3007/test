using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Networking;

public class SoundManager : MonoBehaviour
{
    public List<SoundControl> allSoundControls;
    public List<TalkingClip> allTalkingClips;

    void RemoveDestroyedSound()
    {
        allSoundControls.RemoveAll(x => x == null);
    }

    public void AddNewSoundControl(SoundControl sc)
    {
        RemoveDestroyedSound();

        if (allSoundControls.Any(x => x == sc)) return;

        allSoundControls.Add(sc);
    }

    public void ApplySoundVolume(SoundControlType type, float volume)
    {
        RemoveDestroyedSound();

        foreach (var item in allSoundControls)
        {
            if (item != null && item.soundType == type)
                item.currentVolume = volume;
        }
    }
    
    public void AddTalkingClip(string soundPath)
    {
        var filePath = string.Format("{0}\\{1}\\{2}", Application.dataPath, Helper.Sounds_DataFolder, soundPath);        

        var tc = allTalkingClips.FirstOrDefault(c => c != null && c.soundPath == soundPath);
        if (tc != null)
        {
            if (tc.clip == null)
                tc.clip = Helper.LoadClipFromFile(filePath);
        }
        else
        {
            var clip = Helper.LoadClipFromFile(filePath);
            var item = new TalkingClip
            {
                soundPath = soundPath,
                clip = clip
            };
            allTalkingClips.Add(item);
        }
    }

    public AudioClip GetTalkingClipBySoundPath(string soundPath)
    {
        var tc = allTalkingClips.FirstOrDefault(c => c != null && c.soundPath == soundPath);
        if (tc != null)
            return tc.clip;

        return null;
    }
}

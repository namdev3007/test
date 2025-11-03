using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

public static class Helper
{
    //Moving speed values
    public const float WalkingSpeed = 1f;
    public const float RunningSpeed = 3.5f;
    public const float FastRunningSpeed = 5f;


    //Scenario settings file
    public const string Sc_Section_General = "COMMON";
    public const string Sc_Section_Camera = "CAMERA";
    public const string Sc_Section_Sound = "SOUND";
    public const string Sc_Section_SoundDetailPrefix = "SOUND_";
    public const string Sc_Section_WalkingPeople = "WALKING_PEOPLE";
    public const string Sc_Section_WalkingPeopleDetailPrefix = "WP_";
    public const string Sc_Section_PeopleWithAction = "ACTING_PEOPLE";
    public const string Sc_Section_PeopleWithActionDetailPrefix = "AP_";

    //common section
    public const string Sc_Key_ScenarioName = "name";
    public const string Sc_Key_MusicSound = "music";
    public const string Sc_Key_TalkingSound = "talking";
    public const string Sc_Key_StreetSound = "street";
    public const string Sc_Key_OtherSound = "other";

    //camera section
    public const string Sc_Key_CamDistance = "distance";
    public const string Sc_Key_CAmHeight = "height";
    public const string Sc_Key_CamAngle = "angle";

    //sound sections
    public const string Sc_Key_SoundCount = "soundCount";
    public const string Sc_Key_SoundInfo = "soundInfo";
    public const string Sc_Key_SoundFileCount = "fileCount";
    public const string Sc_Key_SoundFileInfoPrefix = "file_";

    //wp section
    public const string Sc_Key_WPCount = "wpCount";
    public const string Sc_Key_WPInfo = "wpInfo";
    public const string Sc_Key_WPPointCount = "wpPointCount";
    public const string Sc_Key_WPPointInfoPrefix = "wpPoint_";

    //ap section
    public const string Sc_Key_APCount = "apCount";
    public const string Sc_Key_APInfo = "apInfo";    
    public const string Sc_Key_APPointCount = "apPointCount";
    public const string Sc_Key_APPointInfoPrefix = "apPoint_";


    //Path
    public const string Scenarios_DataFolder = "\\Data\\scenarios";
    public const string Sounds_DataFolder = "\\Data\\sounds";
    public const string Help_DataFolder = "\\Data\\help";
    public const string PrefabImages_ResourcesFolder = "Images/PrefabImages/";

    //Select music file
    public const string MusicFileSelectPatterns = "*.mp3";//; - separator
    //Select scenario file
    public const string ScenarioFileSelectPatterns = "*.ini";//; - separator

    //METHODS

    public static string GetDescription(this Enum GenericEnum) //Hint: Change the method signature and input paramter to use the type parameter T
    {
        Type genericEnumType = GenericEnum.GetType();
        MemberInfo[] memberInfo = genericEnumType.GetMember(GenericEnum.ToString());
        if ((memberInfo != null && memberInfo.Length > 0))
        {
            var _Attribs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            if ((_Attribs != null && _Attribs.Count() > 0))
            {
                return ((DescriptionAttribute)_Attribs.ElementAt(0)).Description;

            }
        }
        return GenericEnum.ToString();
    }

    public static float DistanceXZ(Vector3 from, Vector3 to)
    {
        from.y = 0;
        to.y = 0;
        return Vector3.Distance(from, to);
    }

    public static string GetAnimParamByAnim(HumanAnim anim)
    {
        var arr = anim.GetDescription().Split('|');

        if (arr.Length > 0)        
            return arr[0];             
        else
            return anim.GetDescription();
    }

    public static float GetMovingSpeedByAnim(HumanAnim anim)
    {
        var arr = anim.GetDescription().Split('|');
        if (arr.Length > 1 && float.TryParse(arr[1], out float speedType))
        {
            switch (speedType)
            {
                case 0:
                    return 0;
                case 1:
                    return WalkingSpeed;
                case 2:
                    return RunningSpeed;
                case 3:
                    return FastRunningSpeed;
                default:
                    return 0;
            }
        }
        else
            return 0;
    }

    public static AudioClip LoadClipFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            var clip = NAudioPlayer.FromMp3Data(File.ReadAllBytes(filePath), Path.GetFileName(filePath));
            return clip;
        }

        return null;
    }

    public static List<AudioClip> LoadClipsFromFolder(string folderPath, SearchOption option = SearchOption.TopDirectoryOnly)
    {
        if (Directory.Exists(folderPath))
        {
            var files = Directory.GetFiles(folderPath, "*.mp3", option);
            var clips = new List<AudioClip>();
            foreach (var item in files)
            {
                var c = LoadClipFromFile(item);
                if (c != null) clips.Add(c);
            }

            return clips;
        }

        return new List<AudioClip>();
    }
}


public enum HumanAnim
{
    //Description format: {Animation param}|{Moving speed type}|{Index range: optional}
    //Moving speed type: 0:stand | 1:walk | 2:run | 3:run fast
    //Index range(for random trigger): min-max. Anim param = {Animation param}_{Random Index range}

    [Description("|0")]
    Null = 0,

    //stand state anims: 1->99
    [Description("Stand|0")]
    Stand = 1,

    [Description("LookAround1|0")]
    Look_Around_1 = 2,
    [Description("LookAround2|0")]
    Look_Around_2 = 3,
    [Description("LookBack|0")]
    Look_Back = 4,
    [Description("ShakeHandRight|0")]
    Shake_Hand_Right = 5,
    [Description("ShakeHandLeft|0")]
    Shake_Hand_Left = 6,
    [Description("PointingRightHand|0")]
    Pointing_Right_Hand = 7,
    [Description("PointingLeftHand|0")]
    Pointing_Left_Hand = 8,

    [Description("Talk1|0")]
    Stand_Talk_1 = 10,
    [Description("Talk2|0")]
    Stand_Talk_2 = 11,
    [Description("TalkCellphone|0")]
    Stand_Talk_Phone_1 = 12,
    [Description("TalkCellphone2|0")]
    Stand_Talk_Phone_2 = 13,
    [Description("TalkCellphone3|0")]
    Stand_Talk_Phone_3 = 14,

    //sit state anims: 100->199
    [Description("Sit|0")]
    Sit = 100,
    [Description("SitTalking1|0")]
    Sit_Talking_1 = 101,
    [Description("SitTalking2|0")]
    Sit_Talking_2 = 102,
    [Description("SitTalking3|0")]
    Sit_Talking_3 = 103,
    [Description("SitTalking4|0")]
    Sit_Talking_4 = 104,
    [Description("SitTalking5|0")]
    Sit_Talking_5 = 105,

    //walk state anims: 200->299
    [Description("Walk|1")]
    Walk = 200,
    [Description("WalkBriefcase|1")]
    WalkWithBriefcase = 201,
    [Description("WalkLookAround|1")]
    Walk_Look_Around = 202,
    [Description("WalkLookBack|1")]
    Walk_Look_Back = 203,

    [Description("Run|2")]
    Run = 210,

    //Others: 10000 -> Infinity,
    [Description("Bartending1|0")]//bartender
    Bartending_1 = 1000,
    [Description("DatKhay|0")]//tiep vien
    Dat_Khay = 1001,
}

public enum LoopMoving
{
    //Move straight line 1 time
    None = 0,
    //Move reverse line 1 time
    Reverse = 1,
    //Move straight line forever
    Loop = 2,
    //Move reverse line forever
    ReverseLoop = 3
}

public enum CameraControlType
{
    //Description format: {next camera type while switching}

    [Description("2")]
    FlyCamera = 1,
    [Description("1")]
    FollowPlayer = 2
}

public enum AudioPlayType
{
    OneShot_First = 1,
    OneShot_Random = 2,
    All_Loop_OneTime = 3,
    All_Loop_Repeat = 4,
    All_Shuffle_Forever = 5
}

public enum SoundControlType
{
    Music = 1,
    Street = 2,
    Talking = 3,
    Other = 4
}

public enum CreatePathType
{
    None = 0,
    WalkingPeople = 1,
    ObjectWithAction = 2
}

public enum ConfirmDialogStatus
{
    Waiting = 0,
    ConfirmYes = 1,
    ConfirmNo = 2
}

public enum ShowMusicSelectDialogType
{
    Common = 0,
    ActionAtPos = 1
}

[Serializable]
public class ResourceSound
{
    public string path;
    public WAV wav;
    public AudioClip clip;
}

[Serializable]
public class TalkingClip
{
    public string soundPath;
    public AudioClip clip;
}
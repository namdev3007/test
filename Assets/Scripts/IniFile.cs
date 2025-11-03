using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

public class IniFile
{
    // Token: 0x0600003B RID: 59
    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    private static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

    // Token: 0x0600003C RID: 60
    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    private static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

    // Token: 0x0600003D RID: 61 RVA: 0x000065BC File Offset: 0x000047BC
    public IniFile(string IniPath = null)
    {
        this.Path = new FileInfo(IniPath ?? (this.EXE + ".ini")).FullName.ToString();
    }

    // Token: 0x0600003E RID: 62 RVA: 0x00006610 File Offset: 0x00004810
    public string Read(string Key, string Section = null)
    {
        StringBuilder stringBuilder = new StringBuilder(255);
        IniFile.GetPrivateProfileString(Section ?? this.EXE, Key, "", stringBuilder, 255, this.Path);
        return stringBuilder.ToString();
    }

    public bool ReadBoolean(string Key, string Section, bool defaultValue)
    {
        StringBuilder stringBuilder = new StringBuilder(255);
        IniFile.GetPrivateProfileString(Section ?? this.EXE, Key, "", stringBuilder, 255, this.Path);

        bool result;
        if (bool.TryParse(stringBuilder.ToString(), out result))
            return result;
        else
            return defaultValue;
    }

    public int ReadInt(string Key, string Section, int defaultValue)
    {
        StringBuilder stringBuilder = new StringBuilder(255);
        IniFile.GetPrivateProfileString(Section ?? this.EXE, Key, "", stringBuilder, 255, this.Path);

        int result;
        if (int.TryParse(stringBuilder.ToString(), out result))
            return result;
        else
            return defaultValue;
    }

    public float ReadFloat(string Key, string Section, float defaultValue)
    {
        StringBuilder stringBuilder = new StringBuilder(255);
        IniFile.GetPrivateProfileString(Section ?? this.EXE, Key, "", stringBuilder, 255, this.Path);

        float result;
        if (float.TryParse(stringBuilder.ToString(), out result))
            return result;
        else
            return defaultValue;
    }

    // Token: 0x0600003F RID: 63 RVA: 0x00006656 File Offset: 0x00004856
    public void Write(string Key, string Value, string Section = null)
    {
        IniFile.WritePrivateProfileString(Section ?? this.EXE, Key, Value, this.Path);
    }

    // Token: 0x06000040 RID: 64 RVA: 0x00006672 File Offset: 0x00004872
    public void DeleteKey(string Key, string Section = null)
    {
        this.Write(Key, null, Section ?? this.EXE);
    }

    // Token: 0x06000041 RID: 65 RVA: 0x00006689 File Offset: 0x00004889
    public void DeleteSection(string Section = null)
    {
        this.Write(null, null, Section ?? this.EXE);
    }

    // Token: 0x06000042 RID: 66 RVA: 0x000066A0 File Offset: 0x000048A0
    public bool KeyExists(string Key, string Section = null)
    {
        return this.Read(Key, Section).Length > 0;
    }

    // Token: 0x04000065 RID: 101
    public string Path;

    // Token: 0x04000066 RID: 102
    private string EXE = Assembly.GetExecutingAssembly().GetName().Name;
}

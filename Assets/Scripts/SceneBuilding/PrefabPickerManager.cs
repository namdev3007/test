using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;

public class PrefabPickerManager : MonoBehaviour
{
    public ListViewIcons listViewPrefabIcons;
    public SceneBuildingManager sbManager;
    
    public void ButtonOkClicked()
    {
        sbManager.FinishPickPrefab();
    }
}

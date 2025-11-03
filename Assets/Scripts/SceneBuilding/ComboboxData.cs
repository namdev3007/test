using System;
using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;

public class ComboboxData : MonoBehaviour
{
    public ListViewString listView;
    [SerializeField]
    public List<ComboboxDataItem> data;

    void Start()
    {
        //PopulateData();
    }

    public void PopulateData()
    {
        var lst = new ObservableList<string>();

        foreach (var item in data)
        {
            lst.Add(item.Name);
        }

        listView.DataSource = lst;
    }

    public void SelectItem(HumanAnim anim)
    {
        if (listView.DataSource.Count == 0) PopulateData();

        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].Value == anim)
            {
                listView.Select(i);
                break;
            }
        }

        listView.Select(-1);
    }

    public HumanAnim GetSelectedValue()
    {
        if (listView.SelectedIndex != -1)
            return data[listView.SelectedIndex].Value;
        else
            return HumanAnim.Null;
    }
}

[Serializable]
public class ComboboxDataItem
{
    public string Name;
    public HumanAnim Value;
}

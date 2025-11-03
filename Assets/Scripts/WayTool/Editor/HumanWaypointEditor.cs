using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(HumanWaypoint))]
public class HumanWaypointEditor : Editor
{
    HumanWaypoint wpScript;

    void OnSceneGUI()
    {
        Event e = Event.current;
        wpScript = (HumanWaypoint)target;

        if (e != null)
        {

            if (e.isMouse && e.control && e.type == EventType.MouseDown)
            {

                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(ray, out hit, 5000.0f))
                {

                    Vector3 newTilePosition = hit.point;

                    GameObject wp = new GameObject("HP-0" + (wpScript.waypoints.Count + 1).ToString());

                    wp.transform.position = newTilePosition;
                    wp.transform.SetParent(wpScript.transform);

                    GetWaypoints();

                }

            }

            if (wpScript)
                Selection.activeGameObject = wpScript.gameObject;

        }

        GetWaypoints();

    }

    public void GetWaypoints()
    {
        List<Transform> allTransforms = new List<Transform>(wpScript.transform.GetComponentsInChildren<Transform>());

        //remove parent node
        allTransforms.Remove(wpScript.transform);

        //remove order nodes
        allTransforms = allTransforms.Where(t => t.GetComponent<TextOrder>() == null).ToList();

        //process
        for (int i = 0; i < Mathf.Max(wpScript.waypoints.Count, allTransforms.Count); i++)
        {
            if (i < wpScript.waypoints.Count && i < allTransforms.Count)
            {
                wpScript.waypoints[i].name = allTransforms[i].name;
                wpScript.waypoints[i].pos = allTransforms[i];
            }
            else if (i >= wpScript.waypoints.Count && i < allTransforms.Count)
            {
                wpScript.waypoints.Add(
                    new HWaypoint
                    {
                        name = allTransforms[i].name,
                        pos = allTransforms[i]
                    });
            }
            else if (i < wpScript.waypoints.Count && i >= allTransforms.Count)
            {
                wpScript.waypoints.RemoveAt(i);
            }
        }

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HumanWaypoint : MonoBehaviour
{
    public List<HWaypoint> waypoints = new List<HWaypoint>();

    void OnDrawGizmos()
    {        
#if UNITY_EDITOR
        if (!Selection.Contains(gameObject))
        {
            bool selected = false;
            foreach (var item in waypoints)
            {
                if (Selection.Contains(item.pos.gameObject))
                {
                    selected = true;
                    break;
                }                
            }

            if (!selected) return;
        }
#endif

        if (waypoints.Count < 1) return;

        for (int i = 0; i < waypoints.Count; i++)
        {

            Gizmos.color = Color.blue;

            if (waypoints.Count < 1) return;

            Gizmos.DrawSphere(waypoints[i].pos.position, 0.05f);
            Gizmos.DrawWireSphere(waypoints[i].pos.position, 0.1f);

#if UNITY_EDITOR
            UnityEditor.Handles.Label(waypoints[i].pos.position + Vector3.up * 0.1f, transform.name + " - " + waypoints[i].name);
#endif

            if (waypoints.Count < 2) return;

            if (i < waypoints.Count - 1)
            {
                if (waypoints[i].pos && waypoints[i + 1].pos)
                {

                    if (waypoints.Count > 0)
                    {

                        if (i < waypoints.Count - 1)
                        {
                            Gizmos.DrawLine(waypoints[i].pos.position, waypoints[i + 1].pos.position);
                            waypoints[i].pos.LookAt(waypoints[i + 1].pos);

                        }

                    }
                }
            }
            else if (i == waypoints.Count - 1)
            {
                waypoints[i].pos.rotation = waypoints[i - 1].pos.rotation;
            }

        }

    }

}

[Serializable]
public class HWaypoint
{
    public string name;
    public Transform pos;

    [Space(5)]
    public Vector3 posV3;    
    public HumanAnim animAtPos;
    public float timeWaitAtPos;    
    public bool haveLookDir;
    public float lookAngle;
    public bool haveSoundAtPos;
    public string soundAtPosPath;

    [Space(5)]
    public HumanAnim animWhileGoingToNextPos;
    public float speedToNextPos;    

    [Space(5)]
    public bool destroyAIAtPos;

    [Space(5)]
    public float approxTimeReachPoint;

    public HWaypoint()
    {
        name = string.Empty;
        soundAtPosPath = string.Empty;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        var pObj = (HWaypoint)obj;
        if (pObj == null) return false;

        return posV3 == pObj.posV3
            && animAtPos == pObj.animAtPos
            && timeWaitAtPos == pObj.timeWaitAtPos
            && haveLookDir == pObj.haveLookDir
            && lookAngle == pObj.lookAngle
            && haveSoundAtPos == pObj.haveSoundAtPos
            && string.Compare(soundAtPosPath, pObj.soundAtPosPath) == 0
            && animWhileGoingToNextPos == pObj.animWhileGoingToNextPos
            && speedToNextPos == pObj.speedToNextPos;
    }

    public string GetSavedDataString()
    {
        var str = string.Format("({0},{1},{2})|{3}|{4}|{5}|{6}|{7}|{8}"
                    , posV3.x, posV3.y, posV3.z
                    , (int)animAtPos
                    , timeWaitAtPos
                    , (int)animWhileGoingToNextPos
                    , speedToNextPos
                    , haveLookDir ? lookAngle.ToString() : ""
                    , haveSoundAtPos ? soundAtPosPath : "");

        return str;
    }

    public HWaypoint(string savedDataString)
    {
        var pInfoArr = savedDataString.Split('|');

        var posArr = pInfoArr[0].Replace("(", "").Replace(")", "").Split(',');
        posV3 = new Vector3(float.Parse(posArr[0]), float.Parse(posArr[1]), float.Parse(posArr[2]));

        animAtPos = (HumanAnim)int.Parse(pInfoArr[1]);
        timeWaitAtPos = float.Parse(pInfoArr[2]);
        animWhileGoingToNextPos = (HumanAnim)int.Parse(pInfoArr[3]);
        speedToNextPos = float.Parse(pInfoArr[4]);
        haveLookDir = float.TryParse(pInfoArr[5], out float x);
        lookAngle = x;
        soundAtPosPath = pInfoArr[6].Trim();
        haveSoundAtPos = !string.IsNullOrWhiteSpace(soundAtPosPath);
    }
}

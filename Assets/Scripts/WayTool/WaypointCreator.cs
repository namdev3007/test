using SWS;
using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;

public class WaypointCreator : MonoBehaviour
{
    [Header("Setup")]
    public SceneBuildingManager sbManager;
    public GameObject pointPrefab;
    public GameObject pathPrefab;
    public KeyCode modeSwitchKey = KeyCode.F12;
    public KeyCode placePointKey = KeyCode.P;
    public KeyCode finishPathKey = KeyCode.Escape;

    [Header("Debug")]
    public bool inEditingMode;
    public GameObject pointPreview;
    public GameObject pathContainer;    
    public List<GameObject> paths;
    public int pathCount;
    public GameObject currentPath;
    public List<GameObject> points;
    
    void Start()
    {
        if (pathContainer == null)
        {
            pathContainer = new GameObject("Paths container");
            pathCount = 0;
        }
    }

    void Update()
    {
        if (inEditingMode)
        {
            DrawPoint();
            PlacePoint();
            FinishPath();
        }
    }

    void DrawPoint()
    {
        if (pointPreview == null) pointPreview = Instantiate(pointPrefab);

        Ray worldRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        //Ignore stop wall
        if (Physics.Raycast(worldRay, out hitInfo, Mathf.Infinity, ~LayerMask.NameToLayer(""), QueryTriggerInteraction.Ignore))
        {
            pointPreview.transform.position = hitInfo.point;
        }
    }

    void PlacePoint()
    {
        if (Input.GetKeyDown(placePointKey))
        {
            var p = Instantiate(pointPrefab);
            p.name = "Point " + (points.Count + 1).ToString();
            p.transform.position = pointPreview.transform.position;
            p.transform.parent = currentPath.transform;

            points.Add(p);

            //Enable line draw when have 2 points
            if (points.Count == 2)
            {
                currentPath.GetComponent<LineRenderer>().enabled = true;
                currentPath.GetComponent<PathRenderer>().enabled = true;
            }

            //Create path
            if (points.Count >= 1) currentPath.GetComponent<PathManager>().Create();            
        }
    }

    void FinishPath()
    {
        if (Input.GetKeyDown(finishPathKey))
        {
            inEditingMode = false;

            if (pointPreview != null) DestroyImmediate(pointPreview);

            //Delete path if dont have at least 1 point
            if (points.Count < 1)
            {
                pathCount--;
                DestroyImmediate(currentPath);
            }

            //Add created path to list
            if (currentPath != null) paths.Add(currentPath);

            //Notify to manager
            if (sbManager != null)
            {
                if (currentPath != null)
                {
                    var pathPoints = new ObservableList<Vector3>();
                    if (currentPath.GetComponent<PathManager>().waypoints.Length > 0)
                    {
                        foreach (var item in currentPath.GetComponent<PathManager>().waypoints)
                            pathPoints.Add(item.position);
                    }
                    else
                    {
                        foreach (var item in points)
                        {
                            pathPoints.Add(item.transform.position);
                        }
                    }
                    
                    sbManager.FinishCreateNewPath(pathPoints);
                }
                else sbManager.CancelCreateNewPath();
            }

            currentPath = null;
        }
    }

    public void CreatePathManually(string oldGuid, string newGuid)
    {
        //Remove old path
        if (!string.IsNullOrEmpty(oldGuid))
        {
            var oldPath = paths.Find(p => p.GetComponent<ObjectIdentifier>().Id == oldGuid);
            if (oldPath != null)
            {
                //remove from list
                paths.Remove(oldPath);

                //destroy people if needed
                var hs = oldPath.GetComponent<HumanAISpawner>();
                if (hs != null && hs.peopleContainer != null) DestroyImmediate(hs.peopleContainer);

                //destroy old path
                DestroyImmediate(oldPath);
                pathCount--;
            }
        }


        if (!inEditingMode)
        {
            inEditingMode = true;

            pathCount++;

            //Not set instantiated object name but
            //change prefab name to fix warning about duplicated name in WaypointManager
            //refer to: PathManager.Awake() && WaypointManager.AddPath()
            pathPrefab.name = "Path_" + newGuid;
            currentPath = Instantiate(pathPrefab, pathContainer.transform);
            currentPath.GetComponent<ObjectIdentifier>().Id = newGuid;
            currentPath.GetComponent<PathManager>().drawCurved = false;

            points = new List<GameObject>();
        }
        
    }

    public void CreatePathFromPoints(ObservableList<Vector3> pList, string guid, bool showPath)
    {
        if (pathContainer == null)
        {
            pathContainer = new GameObject("Paths container");
            pathCount = 0;
        }

        if (pList.Count == 0) return;

        pathCount++;

        //Not set instantiated object name but
        //change prefab name to fix warning about duplicated name in WaypointManager
        //refer to: PathManager.Awake() && WaypointManager.AddPath()
        pathPrefab.name = "Path_" + guid;
        currentPath = Instantiate(pathPrefab, pathContainer.transform);
        currentPath.GetComponent<ObjectIdentifier>().Id = guid;

        for (int i = 0; i < pList.Count; i++)
        {
            var p = Instantiate(pointPrefab);
            p.name = "Point " + (i + 1).ToString();
            p.transform.position = pList[i];
            p.transform.parent = currentPath.transform;
        }

        paths.Add(currentPath);

        currentPath.GetComponent<LineRenderer>().enabled = true;
        currentPath.GetComponent<PathRenderer>().enabled = true;
        currentPath.GetComponent<PathManager>().drawCurved = false;
        currentPath.GetComponent<PathManager>().Create();

        currentPath = null;

        ShowHidePath(guid, showPath);
    }

    public GameObject GetPathObj(string guid)
    {
        var path = paths.Find(p => p.GetComponent<ObjectIdentifier>().Id == guid);
        return path;
    }

    public void DeletePath(string guid)
    {
        var path = paths.Find(p => p.GetComponent<ObjectIdentifier>().Id == guid);
        if (path != null)
        {
            paths.Remove(path);
            DestroyImmediate(path);
            pathCount--;
        }
    }

    public Transform GetFirstPointOnPath(string guid)
    {
        foreach (var p in paths)
        {
            if (p != null && p.GetComponent<ObjectIdentifier>().Id == guid)
                return p.GetComponent<PathManager>().waypoints[0];
        }

        return null;
    }

    public void ShowHidePath(string guid, bool isShow)
    {
        foreach (var p in paths)
        {
            if (p != null && p.GetComponent<ObjectIdentifier>().Id == guid)
            {
                p.GetComponent<LineRenderer>().enabled = isShow;
                int order = 1;
                foreach (var item in p.GetComponent<PathManager>().waypoints)
                {   
                    item.GetComponent<MeshRenderer>().enabled = isShow;
                    item.GetComponentInChildren<TextOrder>().SetText((order++).ToString(), isShow);
                }
                break;
            }
        }
    }

    public void ChangePointPosition(string guid, int pointIndex, Vector3 toValue)
    {
        foreach (var p in paths)
        {
            if (p != null && p.GetComponent<ObjectIdentifier>().Id == guid)
            {
                p.GetComponent<PathManager>().waypoints[pointIndex].localPosition = toValue;
                break;
            }
        }
    }

}

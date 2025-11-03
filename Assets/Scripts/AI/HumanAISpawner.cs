using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;
using System.Linq;

public class HumanAISpawner : MonoBehaviour
{
    public GameObject[] aiPrefabs;
    public HumanWaypoint way;
    public GameObject peopleContainer;
    [HideInInspector]
    public bool autoSpawnAI;

    public GameObject spawnPlane;

    [Space(5)]
    public bool createAlongPath;
    public float spawnInterval;
    public LoopMoving loopMoving;

    [Header("Debug")]
    public HumanAIController[] allPeople;
    public float currentTime;
    public float lastSpawnTime;
    public float nextSpawnTime;

    void Start()
    {
    }

    void Update()
    {   
        if (autoSpawnAI) SpawnAI();
    }

    void GetNextSpawnTime()
    {
        nextSpawnTime = Time.time + Mathf.Clamp(spawnInterval + Random.Range(-5, 5), 5, Mathf.Infinity);
    }

    void SpawnAI()
    {
        currentTime = Time.time;

        if (createAlongPath && (loopMoving == LoopMoving.Loop || loopMoving == LoopMoving.ReverseLoop)) return;

        if (nextSpawnTime < Time.time && peopleContainer != null && way.waypoints.Count >= 2)
        {
            //DO LATER: check people around spawn pos
            int spawnIndex = Random.Range(0, aiPrefabs.Length);
            int firstWayIndex = loopMoving == LoopMoving.Loop || loopMoving == LoopMoving.None ? 0 : way.waypoints.Count - 1;
            int nextWayIndex = loopMoving == LoopMoving.Loop || loopMoving == LoopMoving.None ? 1 : way.waypoints.Count - 2;
            var spawnPos = way.waypoints[firstWayIndex].pos.position;
            var spawnRot = Quaternion.LookRotation(way.waypoints[nextWayIndex].pos.position - way.waypoints[firstWayIndex].pos.position);
            
            SpawnOne(spawnIndex, spawnPos, Vector3.zero, spawnRot, firstWayIndex);
            GetNextSpawnTime();
        }
    }

    public void SpawnWalkingPeopleOnPath(Transform[] points, bool alongPath, bool loop, bool reverse, float density, float interval)
    {   
        if (aiPrefabs.Length == 0 || way == null) return;

        createAlongPath = alongPath;
        spawnInterval = interval;
        density = Mathf.Clamp(density / 10f, 0, 4);

        if (loop)
            loopMoving = reverse ? LoopMoving.ReverseLoop : LoopMoving.Loop;
        else
            loopMoving = reverse == true ? LoopMoving.Reverse : LoopMoving.None;        

        //Create people container
        if (peopleContainer == null)
        {
            peopleContainer = new GameObject();
            peopleContainer.transform.parent = this.transform.parent;
            peopleContainer.name = "people_" + this.name;
        }

        //Create waypoints if needed
        if (way.waypoints.Count == 0 && points.Length > 0)
        {
            for (int i = 0; i < points.Length; i++)
            {
                way.waypoints.Add(
                    new HWaypoint
                    {
                        name = points[i].name,
                        pos = points[i]
                    });
            }
        }
        way.waypoints[0].destroyAIAtPos = loopMoving == LoopMoving.Reverse;
        way.waypoints[way.waypoints.Count - 1].destroyAIAtPos = loopMoving == LoopMoving.None;

        //Spawn along path
        if (createAlongPath && way.waypoints.Count >= 2)
        {
            for (int i = 0; i < way.waypoints.Count - 1; i++)
            {
                int index = !reverse ? i : (way.waypoints.Count - 1) - i;
                int nextIndex = !reverse ? i + 1 : (way.waypoints.Count - 1) - i - 1;

                Vector3 start = way.waypoints[index].pos.position;
                Vector3 end = way.waypoints[nextIndex].pos.position;
                Vector3 dir = end - start;
                Vector3 nor = dir.normalized;

                float magnitude = dir.magnitude;
                int num3 = (int)(density / 5f * magnitude);
                if (num3 < 1)
                {
                    num3 = 1;
                }
                float num4 = magnitude / (float)num3;
                float num5 = 0f;
                for (int k = 0; k < num3; k++)
                {
                    float num6;
                    if (k == 0)
                    {
                        num6 = Random.Range(num5, num4 - 0.5f);
                    }
                    else
                    {
                        num6 = Random.Range(num5, (float)(k + 1) * num4 - 0.5f);
                    }
                    num5 = num6 + 0.5f;

                    int spawnIndex = UnityEngine.Random.Range(0, aiPrefabs.Length);
                    var spawnPos = start + dir * (num6 / magnitude);
                    var spawnRot = Quaternion.LookRotation(dir);

                    SpawnOne(spawnIndex, spawnPos, Vector3.zero, spawnRot, nextIndex);
                }
            }
        }

        //Start spawn ai automatically
        autoSpawnAI = true;

        //Get next spawn time;
        GetNextSpawnTime();
    }

    public HumanAIController SpawnPeopleWithActionOnPath(Transform[] points, ListViewObjectWithActionItemDescription data)
    {
        if (aiPrefabs.Length == 0 || way == null) return null;

        loopMoving = LoopMoving.None;

        //Create people container
        if (peopleContainer == null)
        {
            peopleContainer = new GameObject();
            peopleContainer.transform.parent = this.transform.parent;
            peopleContainer.name = "people_" + this.name;
        }

        //Creat waypoints if needed
        if (way.waypoints.Count == 0 && points.Length > 0)
        {
            for (int i = 0; i < points.Length; i++)
            {
                way.waypoints.Add(
                    new HWaypoint
                    {
                        name = points[i].name,
                        pos = points[i]
                    });
            }
        }
        //Update points data
        for (int i = 0; i < data.PointsData.Count; i++)
        {
            way.waypoints[i].posV3 = data.PointsData[i].posV3;
            way.waypoints[i].animAtPos = data.PointsData[i].animAtPos;
            way.waypoints[i].timeWaitAtPos = data.PointsData[i].timeWaitAtPos;
            way.waypoints[i].animWhileGoingToNextPos = data.PointsData[i].animWhileGoingToNextPos;
            way.waypoints[i].speedToNextPos = data.PointsData[i].speedToNextPos;
            way.waypoints[i].haveLookDir = data.PointsData[i].haveLookDir;
            way.waypoints[i].lookAngle = data.PointsData[i].lookAngle;
            way.waypoints[i].haveSoundAtPos = data.PointsData[i].haveSoundAtPos;
            way.waypoints[i].soundAtPosPath = data.PointsData[i].soundAtPosPath;
        }


        //Spawn ai
        HumanAIController aiScript = null;
        var spawnIndex = -1;
        for (int i = 0; i < aiPrefabs.Length; i++)
        {
            if (aiPrefabs[i].GetComponent<ObjectIdentifier>().Id == data.ObjectPrefabId)
            {
                spawnIndex = i;
                break;
            }
        }
        if (spawnIndex != -1)
        {
            var spawnRot = Quaternion.identity;
            if (way.waypoints.Count >= 2)
                spawnRot = Quaternion.LookRotation(way.waypoints[1].pos.position - way.waypoints[0].pos.position);

            aiScript = SpawnOne(spawnIndex, data.PointsData[0].posV3, Vector3.zero, spawnRot, 0, 0.1f, true);
            aiScript.ObjGuid = data.ObjectGUID;
        }

        //Just spawn 1 ai
        autoSpawnAI = false;
        nextSpawnTime = Mathf.Infinity;

        return aiScript;
    }

    HumanAIController SpawnOne(int spawnIndex, Vector3 startPos, Vector3 offset, Quaternion rot, int waypointIndex, float closeTargetCheck = 0.3f, bool sameSpeed = false)
    {
        var go = Instantiate(aiPrefabs[spawnIndex], startPos + offset, rot, peopleContainer.transform);

        go.GetComponent<HumanAIController>().spawner = this;
        go.GetComponent<HumanAIController>().wScript = way;
        go.GetComponent<HumanAIController>().offset = offset;
        go.GetComponent<HumanAIController>().currentIndex = waypointIndex;
        go.GetComponent<HumanAIController>().loopType = loopMoving;
        go.GetComponent<HumanAIController>().closeToTargetCheck = closeTargetCheck;
        go.GetComponent<HumanAIController>().sameSpeed = sameSpeed;

        lastSpawnTime = Time.time;

        return go.GetComponent<HumanAIController>();
    }

    public void DestroyAI(GameObject ai)
    {
        Destroy(ai);

        //Set time to spawn next ai right after destroy old one
        if (lastSpawnTime + 2 > Time.time)
            nextSpawnTime = lastSpawnTime + 2;
        else
            nextSpawnTime = Time.time;
    }

    //FOR GROUP OBJECTS WITH FORMATION -- NOT USING ANY MORE
    public void SpawnGroupPeopleWithActionOnPath(Transform[] points, Vector3 startPos, ObservableList<HWaypoint> pointsData, 
                                                int formationType, int formationWidth, int formationLength, int amount, float minDis)
    {
        if (aiPrefabs.Length == 0 || way == null) return;

        loopMoving = LoopMoving.None;

        //Create people container
        if (peopleContainer == null)
        {
            peopleContainer = new GameObject();
            peopleContainer.transform.parent = this.transform.parent;
            peopleContainer.name = "people_" + this.name;
        }

        //Creat waypoints if needed
        if (way.waypoints.Count == 0 && points.Length > 0)
        {
            for (int i = 0; i < points.Length; i++)
            {
                way.waypoints.Add(
                    new HWaypoint
                    {
                        name = points[i].name,
                        pos = points[i]
                    });
            }
        }
        //Update points data
        for (int i = 0; i < pointsData.Count; i++)
        {
            way.waypoints[i].posV3 = pointsData[i].posV3;
            way.waypoints[i].animAtPos = pointsData[i].animAtPos;
            way.waypoints[i].timeWaitAtPos = pointsData[i].timeWaitAtPos;
            way.waypoints[i].animWhileGoingToNextPos = pointsData[i].animWhileGoingToNextPos;
            way.waypoints[i].speedToNextPos = pointsData[i].speedToNextPos;
            way.waypoints[i].haveLookDir = pointsData[i].haveLookDir;
            way.waypoints[i].lookAngle = pointsData[i].lookAngle;
        }


        var spawnRot = Quaternion.identity;
        if (way.waypoints.Count >= 2)
            spawnRot = Quaternion.LookRotation(way.waypoints[1].pos.position - way.waypoints[0].pos.position);

        //Spawn Group
        List<Vector3> groupPos = new List<Vector3>();
        Vector3 groupDir = Vector3.zero;
        if (way.waypoints.Count >= 2)
            groupDir = way.waypoints[1].pos.position - way.waypoints[0].pos.position;

        //spawn plane
        var pl = GameObject.Instantiate(spawnPlane, startPos, spawnRot);
        pl.transform.localScale = new Vector3(formationWidth, 1, formationLength);
        var plane = pl.GetComponentInChildren<MeshFilter>().gameObject;
        Destroy(pl, 5);//destroy after 5s

        List<Vector3> VerticeList = new List<Vector3>(plane.GetComponent<MeshFilter>().sharedMesh.vertices);
        Vector3 leftTop = plane.transform.TransformPoint(VerticeList[0]);
        Vector3 rightTop = plane.transform.TransformPoint(VerticeList[10]);
        Vector3 leftBottom = plane.transform.TransformPoint(VerticeList[110]);
        Vector3 rightBottom = plane.transform.TransformPoint(VerticeList[120]);
        Vector3 XAxis = rightTop - leftTop;
        Vector3 ZAxis = leftBottom - leftTop;

        if (formationType == 1)//Chaos
        {
            for (int i = 0; i < amount; i++)
            {
                Vector3 posI = startPos;
                bool valid = false;
                int invalidCount = 0;
                while (!valid)
                {
                    posI = leftTop + XAxis * Random.value + ZAxis * Random.value;
                    posI.y = startPos.y;

                    valid = CheckGroupPeoplePos(groupPos, posI, minDis);
                    if (valid) groupPos.Add(posI);
                    else invalidCount++;

                    if (invalidCount > 10)
                    {
                        Debug.Log("ERROR");
                        break;
                    }
                }

                //Spawn ai
                var spawnIndex = Random.Range(0, aiPrefabs.Length);
                SpawnOne(spawnIndex, startPos, posI - startPos, spawnRot, 0, 0.1f);
            }
        }
        else if (formationType == 2)//Row
        {
            for (int i = 0; i < amount; i++)
            {
                Vector3 posI = leftTop + ZAxis * i / ZAxis.magnitude;
                for (int j = 0; j < formationWidth / minDis; j++)
                {
                    posI = posI + XAxis * minDis / XAxis.magnitude;
                    posI.y = startPos.y;

                    //Spawn ai
                    var spawnIndex = Random.Range(0, aiPrefabs.Length);
                    SpawnOne(spawnIndex, startPos, posI - startPos, spawnRot, 0, 0.1f, true);
                }
                

            }
        }

        

        //Just spawn 1 ai
        autoSpawnAI = false;
        nextSpawnTime = Mathf.Infinity;
    }

    bool CheckGroupPeoplePos(List<Vector3> groupPos, Vector3 newPos, float checkDisMin)
    {
        for (int i = 0; i < groupPos.Count; i++)
        {
            var d = Vector3.Distance(newPos, groupPos[i]);
            if (d < checkDisMin)
                return false;
        }

        return true;
    }


    Vector3 RandomPointOnPlane(GameObject plane)
    {
        List<Vector3> VerticeList = new List<Vector3>(plane.GetComponent<MeshFilter>().sharedMesh.vertices);
        Vector3 leftTop = plane.transform.TransformPoint(VerticeList[0]);
        Vector3 rightTop = plane.transform.TransformPoint(VerticeList[10]);
        Vector3 leftBottom = plane.transform.TransformPoint(VerticeList[110]);
        Vector3 rightBottom = plane.transform.TransformPoint(VerticeList[120]);
        Vector3 XAxis = rightTop - leftTop;
        Vector3 ZAxis = leftBottom - leftTop;
        Vector3 RndPointonPlane = leftTop + XAxis * Random.value + ZAxis * Random.value;

        return RndPointonPlane + plane.transform.up * 0.5f;
    }


}

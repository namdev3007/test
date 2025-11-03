using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class HumanAIController : MonoBehaviour
{
    public string ObjGuid;

    [Header("Settings")]
    public HumanAnimsController animController;    
    public NavMeshAgent navAgent;
    public HumanAnim defaultAnim;
    public AudioSource audioSource;
    [Space(3)]
    public HumanAISpawner spawner;

    [Header("Waypoints")]
    public HumanWaypoint wScript;
    public Vector3 offset;
    public float closeToTargetCheck = 0.1f;
    public LoopMoving loopType;

    [Header("Sounds")]
    public SoundManager soundManager;

    [Header("Debug")]
    public Vector3 target;
    public bool reachedTarget = false;
    public float remainDisToTarget;
    public float movingSpeed;
    public bool sameSpeed;

    [Space(5)]
    public int currentIndex = 0;
    public int nextIndex = -1;

    private void Awake()
    {
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.maxDistance = 100;
        audioSource.spatialBlend = 1;//Make sound full 3D
        audioSource.volume = 1;
    }

    void Start()
    {
        if (animController == null) animController = GetComponent<HumanAnimsController>();
        if (navAgent == null) navAgent = GetComponent<NavMeshAgent>();

        soundManager = FindObjectOfType<SoundManager>();

        //Warp starting position
        navAgent.Warp(transform.position);

        //Move to first point or target point
        nextIndex = currentIndex;
        if (wScript != null && wScript.waypoints.Count > 0)
            StartCoroutine(MoveToWaypoint(nextIndex, HumanAnim.Walk, Helper.WalkingSpeed));
        else
            animController.commandAnim = defaultAnim;
    }

    void Update()
    {   
        remainDisToTarget = Helper.DistanceXZ(transform.position, target);
        CheckReachDestination();
    }


    void GetNextMovingIndex()
    {
        if (wScript.waypoints.Count == 1)
        {
            nextIndex = -1;
            return;
        }       

        if (loopType == LoopMoving.None)//Move 1 time
        {
            if (currentIndex + 1 < wScript.waypoints.Count)
                nextIndex = currentIndex + 1;
            else
                nextIndex = -1;
        }
        else if (loopType == LoopMoving.Reverse)//Move reverse 1 time
        {
            if (currentIndex - 1 >= 0)
                nextIndex = currentIndex - 1;
            else
                nextIndex = -1;

        }
        else if (loopType == LoopMoving.Loop)//Forever loop
        {
            if (currentIndex + 1 < wScript.waypoints.Count)
                nextIndex = currentIndex + 1;
            else
                nextIndex = 0;
        }
        else if (loopType == LoopMoving.ReverseLoop)//Forever loop and reverse
        {
            if (currentIndex - 1 >= 0)
                nextIndex = currentIndex - 1;
            else
                nextIndex = wScript.waypoints.Count - 1;
        }
    }


    IEnumerator MoveToWaypoint(int targetIndex, HumanAnim toTargetAnim, float toTargetSpeed, float timeWait = 0)
    {
        yield return new WaitForSeconds(timeWait);

        if (wScript != null && wScript.waypoints != null && targetIndex >= 0 && targetIndex < wScript.waypoints.Count)
        {
            reachedTarget = false;

            target = wScript.waypoints[targetIndex].pos.position + offset;
            navAgent.SetDestination(target);      
            
            movingSpeed = toTargetSpeed == 0 ? Helper.GetMovingSpeedByAnim(toTargetAnim) : toTargetSpeed;
            if (!sameSpeed) movingSpeed = movingSpeed + Random.Range(movingSpeed * -0.15f, movingSpeed * 0.15f);
            navAgent.speed = movingSpeed;

            animController.commandAnim = toTargetAnim;
        }
    }

    void CheckReachDestination()
    {
        if (//navAgent.remainingDistance != Mathf.Infinity
            navAgent.pathStatus == NavMeshPathStatus.PathComplete
            && remainDisToTarget <= closeToTargetCheck
            && !reachedTarget)
        {
            //Reach target
            reachedTarget = true;
            currentIndex = nextIndex;

            //Get info at current position
            var animAtPos = wScript.waypoints[currentIndex].animAtPos == HumanAnim.Null ? HumanAnim.Stand : wScript.waypoints[currentIndex].animAtPos;
            //var rotAtPos = wScript.waypoints[currentIndex].pos.localEulerAngles;
            var timeWaitAtpos = wScript.waypoints[currentIndex].timeWaitAtPos;
            var toAnim = wScript.waypoints[currentIndex].animWhileGoingToNextPos == HumanAnim.Null ? HumanAnim.Walk : wScript.waypoints[currentIndex].animWhileGoingToNextPos;
            var toSpeed = wScript.waypoints[currentIndex].speedToNextPos;
            var talkAtPos = wScript.waypoints[currentIndex].haveSoundAtPos;
            var soundAtPosPath = wScript.waypoints[currentIndex].soundAtPosPath;
            var destroyAI = wScript.waypoints[currentIndex].destroyAIAtPos;
            var haveLookDir = wScript.waypoints[currentIndex].haveLookDir;
            var lookAngle = wScript.waypoints[currentIndex].lookAngle;

            //DestoyAI if needed
            if (destroyAI)
            {
                //Notify to spawner to destroy ai
                if (spawner != null)
                    spawner.DestroyAI(this.gameObject);
                else
                    Destroy(gameObject);
            }

            //Perform action at pos
            if (timeWaitAtpos > 0)
                animController.commandAnim = animAtPos;

            //Look at direction
            if (haveLookDir)
            {
                Vector3 start = wScript.waypoints[currentIndex].posV3;
                Vector3 nor = start.normalized;
                Vector3 end = start + Quaternion.Euler(0, lookAngle, 0) * nor;

                transform.LookAt(end);
            }


            if (talkAtPos)
            {
                //var filePath = string.Format("{0}\\{1}\\{2}", Application.dataPath, Helper.Sounds_DataFolder, soundAtPosPath);
                //var clip = Helper.LoadClipFromFile(filePath);
                //if (clip != null) audioSource.PlayOneShot(clip);

                var clip = soundManager.GetTalkingClipBySoundPath(soundAtPosPath);
                if (clip != null) audioSource.PlayOneShot(clip);
            }

            //Move next
            GetNextMovingIndex();
            if (nextIndex != -1)
                StartCoroutine(MoveToWaypoint(nextIndex, toAnim, toSpeed, timeWaitAtpos));
        }
    }

}

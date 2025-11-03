using UnityEngine;


public class HumanAnimsController : MonoBehaviour
{
    public Animator animator;
    public HumanAnim currentAnim;


    [Header("Commands")]
    public HumanAnim commandAnim;
    

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (commandAnim != currentAnim) ChangeAnim(commandAnim);
    }

    void ChangeAnim(HumanAnim toAnim)
    {
        doTurnBone = false;

        string toParam = Helper.GetAnimParamByAnim(toAnim);
        
        if (!string.IsNullOrEmpty(toParam))
        {
            animator.SetTrigger(toParam);
            currentAnim = toAnim;
        }

        //Look around
        if (currentAnim == HumanAnim.Walk_Look_Around || currentAnim == HumanAnim.Look_Around_2)
        {
            doLook = true;

            //get random look around angle
            lookAngle = Random.Range(40, 90) * (Random.Range(0, 2) * 2 - 1);//random angle: 40 -> 90 or -40 -> -90
        }

        //Look back
        if (currentAnim == HumanAnim.Walk_Look_Back || currentAnim == HumanAnim.Look_Back)
        {
            doLook = true;
            doTurnBone = true;

            //get random look back angle
            lookAngle = 160 * (Random.Range(0, 2) * 2 - 1);
            turnBoneRot = -60 * Mathf.Sign(lookAngle) * turnBoneRot;
        }
    }


    [Header("Look around")]
    public Transform head;
    public Vector3 turnBoneRot;
    public float weightSpeed = 1f;       // tốc độ tăng giảm weight

    float lookAngle = 0;
    Vector3 lookTarget;
    bool haveTarget;
    float currentWeight = 0f;

    [Header("Debug")]
    public bool doLook;
    public bool doTurnBone;

    void OnAnimatorIK(int layerIndex)
    {
        if (head == null) return;

        if (doLook && !haveTarget)
        {
            doLook = false;
            haveTarget = true;

            lookTarget = head.position + Quaternion.Euler(0, lookAngle, 0) * transform.forward;
        }

        if (haveTarget)
        {
            // Mượt tăng weight lên 1
            currentWeight = Mathf.MoveTowards(currentWeight, 1f, weightSpeed * Time.deltaTime);

            if (doTurnBone)//for look back
            {
                var cpr = new Vector3(turnBoneRot.x * currentWeight, turnBoneRot.y * currentWeight, turnBoneRot.z * currentWeight);
                animator.SetBoneLocalRotation(HumanBodyBones.UpperChest, Quaternion.Euler(cpr));
            }

            animator.SetLookAtWeight(currentWeight);
            animator.SetLookAtPosition(lookTarget);

            if (currentWeight == 1) haveTarget = false;
        }
        else
        {
            // Mượt giảm weight về 0 khi không có target
            currentWeight = Mathf.MoveTowards(currentWeight, 0f, weightSpeed * Time.deltaTime);

            if (doTurnBone)//for look back
            {
                var cpr = new Vector3(turnBoneRot.x * currentWeight, turnBoneRot.y * currentWeight, turnBoneRot.z * currentWeight);
                animator.SetBoneLocalRotation(HumanBodyBones.UpperChest, Quaternion.Euler(cpr));
            }

            animator.SetLookAtWeight(currentWeight);
            animator.SetLookAtPosition(lookTarget);
        }

    }
}

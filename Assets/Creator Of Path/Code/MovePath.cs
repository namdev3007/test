using System;
using UnityEngine;

// Token: 0x02000003 RID: 3
[Serializable]
public class MovePath : MonoBehaviour
{
    // Token: 0x06000006 RID: 6 RVA: 0x0000234C File Offset: 0x0000054C
    public void MyStart(int _w, int _i, string anim, bool _loop, bool _forward, float _moveSpeed)
    {
        this.forward = _forward;
        this.moveSpeed = _moveSpeed;
        WalkPath component = this.walkPath.GetComponent<WalkPath>();
        this.w = _w;
        this.targetPointsTotal = component.getPointsTotal(0) - 2;
        this.loop = _loop;
        this.animName = anim;
        if (this.loop)
        {
            if (_i < this.targetPointsTotal && _i > 0)
            {
                if (this.forward)
                {
                    this.targetPoint = _i + 1;
                    this.finishPos = component.getNextPoint(this.w, _i + 1);
                    return;
                }
                this.targetPoint = _i;
                this.finishPos = component.getNextPoint(this.w, _i);
                return;
            }
            else
            {
                if (this.forward)
                {
                    this.targetPoint = 1;
                    this.finishPos = component.getNextPoint(this.w, 1);
                    return;
                }
                this.targetPoint = this.targetPointsTotal;
                this.finishPos = component.getNextPoint(this.w, this.targetPointsTotal);
                return;
            }
        }
        else
        {
            if (this.forward)
            {
                this.targetPoint = _i + 1;
                this.finishPos = component.getNextPoint(this.w, _i + 1);
                return;
            }
            this.targetPoint = _i;
            this.finishPos = component.getNextPoint(this.w, _i);
            return;
        }
    }

    // Token: 0x06000007 RID: 7 RVA: 0x00002480 File Offset: 0x00000680
    private void Start()
    {
        Vector3 vector;
        vector = new Vector3(this.finishPos.x, base.transform.position.y, this.finishPos.z);
        base.transform.LookAt(vector);
        base.GetComponent<Animator>().CrossFade(this.animName, 0.1f, 0, UnityEngine.Random.Range(0f, 1f));
        if (this.animName == "walk")
        {
            base.GetComponent<Animator>().speed = this.moveSpeed * 1.2f;
            return;
        }
        if (this.animName == "run")
        {
            base.GetComponent<Animator>().speed = this.moveSpeed / 3f;
        }
    }

    // Token: 0x06000008 RID: 8 RVA: 0x00002540 File Offset: 0x00000740
    private void Update()
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(base.transform.position + new Vector3(0f, 2f, 0f), -base.transform.up, out raycastHit))
        {
            this.finishPos.y = raycastHit.point.y;
            base.transform.position = new Vector3(base.transform.position.x, raycastHit.point.y, base.transform.position.z);
        }
        Vector3 nextPoint;
        nextPoint = new Vector3(this.finishPos.x, base.transform.position.y, this.finishPos.z);
        WalkPath component = this.walkPath.GetComponent<WalkPath>();
        if (Vector3.Distance(base.transform.position, this.finishPos) < 0.2f && this.animName == "walk" && (this.loop || (!this.loop && this.targetPoint > 0 && this.targetPoint < this.targetPointsTotal)))
        {
            if (this.forward)
            {
                if (this.targetPoint < this.targetPointsTotal)
                {
                    nextPoint = component.getNextPoint(this.w, this.targetPoint + 1);
                }
                else
                {
                    nextPoint = component.getNextPoint(this.w, 0);
                }
                nextPoint.y = base.transform.position.y;
            }
            else
            {
                if (this.targetPoint > 0)
                {
                    nextPoint = component.getNextPoint(this.w, this.targetPoint - 1);
                }
                else
                {
                    nextPoint = component.getNextPoint(this.w, this.targetPointsTotal);
                }
                nextPoint.y = base.transform.position.y;
            }
        }
        if (Vector3.Distance(base.transform.position, this.finishPos) < 0.5f && this.animName == "run" && (this.loop || (!this.loop && this.targetPoint > 0 && this.targetPoint < this.targetPointsTotal)))
        {
            if (this.forward)
            {
                if (this.targetPoint < this.targetPointsTotal)
                {
                    nextPoint = component.getNextPoint(this.w, this.targetPoint + 1);
                }
                else
                {
                    nextPoint = component.getNextPoint(this.w, 0);
                }
                nextPoint.y = base.transform.position.y;
            }
            else
            {
                if (this.targetPoint > 0)
                {
                    nextPoint = component.getNextPoint(this.w, this.targetPoint - 1);
                }
                else
                {
                    nextPoint = component.getNextPoint(this.w, this.targetPointsTotal);
                }
                nextPoint.y = base.transform.position.y;
            }
        }
        Vector3 vector = nextPoint - base.transform.position;
        if (vector != Vector3.zero)
        {
            Quaternion rotation = Quaternion.identity;
            if (this.animName == "walk")
            {
                rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.LookRotation(vector), Time.deltaTime * 4f * this.moveSpeed);
            }
            else if (this.animName == "run")
            {
                rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.LookRotation(vector), Time.deltaTime * 1.3f * this.moveSpeed);
            }
            base.transform.rotation = rotation;
        }
        if (base.transform.position != this.finishPos)
        {
            base.transform.position = Vector3.MoveTowards(base.transform.position, this.finishPos, Time.deltaTime * 1f * this.moveSpeed);
            return;
        }
        if (base.transform.position == this.finishPos && this.forward)
        {
            if (this.targetPoint != this.targetPointsTotal)
            {
                this.targetPoint++;
                this.finishPos = component.getNextPoint(this.w, this.targetPoint);
                return;
            }
            if (this.targetPoint == this.targetPointsTotal)
            {
                if (this.loop)
                {
                    this.finishPos = component.getStartPoint(this.w);
                    this.targetPoint = 0;
                    return;
                }
                component.SpawnOnePeople(this.w, this.forward, this.moveSpeed);
                Destroy(base.gameObject);
                return;
            }
        }
        else if (base.transform.position == this.finishPos && !this.forward)
        {
            if (this.targetPoint > 0)
            {
                this.targetPoint--;
                this.finishPos = component.getNextPoint(this.w, this.targetPoint);
                return;
            }
            if (this.targetPoint == 0)
            {
                if (this.loop)
                {
                    this.finishPos = component.getNextPoint(this.w, this.targetPointsTotal);
                    this.targetPoint = this.targetPointsTotal;
                    return;
                }
                component.SpawnOnePeople(this.w, this.forward, this.moveSpeed);
                Destroy(base.gameObject);
            }
        }
    }

    // Token: 0x04000010 RID: 16
    [SerializeField]
    public Vector3 startPos;

    // Token: 0x04000011 RID: 17
    [SerializeField]
    public Vector3 finishPos;

    // Token: 0x04000012 RID: 18
    [SerializeField]
    public int w;

    // Token: 0x04000013 RID: 19
    [SerializeField]
    public int targetPoint;

    // Token: 0x04000014 RID: 20
    [SerializeField]
    public int targetPointsTotal;

    // Token: 0x04000015 RID: 21
    [SerializeField]
    public string animName;

    // Token: 0x04000016 RID: 22
    [SerializeField]
    public float moveSpeed;

    // Token: 0x04000017 RID: 23
    [SerializeField]
    public bool loop;

    // Token: 0x04000018 RID: 24
    [SerializeField]
    public bool forward;

    // Token: 0x04000019 RID: 25
    [SerializeField]
    public GameObject walkPath;
}

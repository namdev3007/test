using System;
using UnityEngine;
using Random = UnityEngine.Random;

// Token: 0x02000005 RID: 5
public class PeopleController : MonoBehaviour
{
	// Token: 0x0600000E RID: 14 RVA: 0x00002BE2 File Offset: 0x00000DE2
	private void Start()
	{
		this.Tick();
	}

	// Token: 0x0600000F RID: 15 RVA: 0x00002BEC File Offset: 0x00000DEC
	private void Tick()
	{
		this.timer = 0f;
		int num = Random.Range(0, this.animNames.Length);
		this.SetAnimClip(this.animNames[num]);
		this.timer = Random.Range(3f, 5f);
	}

	// Token: 0x06000010 RID: 16 RVA: 0x00002C38 File Offset: 0x00000E38
	public void SetTarget(Vector3 _target)
	{
		Vector3 vector;
		vector = new Vector3(_target.x, base.transform.position.y, _target.z);
		base.transform.LookAt(vector);
	}

	// Token: 0x06000011 RID: 17 RVA: 0x00002C76 File Offset: 0x00000E76
	private void Update()
	{
		if (this.timer >= 0f)
		{
			this.timer -= Time.deltaTime;
			return;
		}
		this.Tick();
	}

	// Token: 0x06000012 RID: 18 RVA: 0x00002C9E File Offset: 0x00000E9E
	public void SetAnimClip(string animName)
	{
		base.GetComponent<Animator>().CrossFade(animName, 0.1f, 0, Random.Range(0f, 1f));
	}

	// Token: 0x04000021 RID: 33
	[HideInInspector]
	public float timer;

	// Token: 0x04000022 RID: 34
	[HideInInspector]
	public string[] animNames;
}

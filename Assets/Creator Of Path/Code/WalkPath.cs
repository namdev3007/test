using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// Token: 0x0200000B RID: 11
[Serializable]
public class WalkPath : MonoBehaviour
{
	// Token: 0x0600002F RID: 47 RVA: 0x00004951 File Offset: 0x00002B51
	public Vector3 getNextPoint(int w, int index)
	{
		return this.points[w, index];
	}

	// Token: 0x06000030 RID: 48 RVA: 0x00004965 File Offset: 0x00002B65
	public Vector3 getStartPoint(int w)
	{
		return this.points[w, 1];
	}

	// Token: 0x06000031 RID: 49 RVA: 0x00004979 File Offset: 0x00002B79
	public int getPointsTotal(int w)
	{
		return this.pointLength[w];
	}

	// Token: 0x06000032 RID: 50 RVA: 0x00004983 File Offset: 0x00002B83
	private void Awake()
	{
		this.DrawCurved(false);
	}

	// Token: 0x06000033 RID: 51 RVA: 0x0000498C File Offset: 0x00002B8C
	public void OnDrawGizmos()
	{
		this.DrawCurved(true);
	}

	// Token: 0x06000034 RID: 52 RVA: 0x00004998 File Offset: 0x00002B98
	public void DrawCurved(bool withDraw)
	{
		if (this.numberOfWays < 1)
		{
			this.numberOfWays = 1;
		}
		if (this.lineSpacing < 0.6f)
		{
			this.lineSpacing = 0.6f;
		}
		if (this.density < 0.1f)
		{
			this.density = 0.1f;
		}
		if (this.moveSpeed < 0.1f)
		{
			this.moveSpeed = 0.1f;
		}
		this._forward = new bool[this.numberOfWays];
		this.isWalk = (this._moveType.ToString() == "Walk");
		for (int i = 0; i < this.numberOfWays; i++)
		{
			if (this.direction.ToString() == "Forward")
			{
				this._forward[i] = true;
			}
			else if (this.direction.ToString() == "Backward")
			{
				this._forward[i] = false;
			}
			else if (this.direction.ToString() == "HugLeft")
			{
				if ((i + 2) % 2 == 0)
				{
					this._forward[i] = true;
				}
				else
				{
					this._forward[i] = false;
				}
			}
			else if (this.direction.ToString() == "HugRight")
			{
				if ((i + 2) % 2 == 0)
				{
					this._forward[i] = false;
				}
				else
				{
					this._forward[i] = true;
				}
			}
			else if (this.direction.ToString() == "WeaveLeft")
			{
				if (i == 1 || i == 2 || (i - 1) % 4 == 0 || (i - 2) % 4 == 0)
				{
					this._forward[i] = false;
				}
				else
				{
					this._forward[i] = true;
				}
			}
			else if (this.direction.ToString() == "WeaveRight")
			{
				if (i == 1 || i == 2 || (i - 1) % 4 == 0 || (i - 2) % 4 == 0)
				{
					this._forward[i] = true;
				}
				else
				{
					this._forward[i] = false;
				}
			}
		}
		if (this.pathPoint.Count < 2)
		{
			return;
		}
		this.points = new Vector3[this.numberOfWays + 2, this.pathPoint.Count + 2];
		this.pointLength[0] = this.pathPoint.Count + 2;
		for (int j = 0; j < this.pathPoint.Count; j++)
		{
			this.points[0, j + 1] = this.pathPointTransform[j].transform.position;
		}
		this.points[0, 0] = this.points[0, 1];
		this.points[0, this.pointLength[0] - 1] = this.points[0, this.pointLength[0] - 2];
		for (int k = 0; k < this.pointLength[0]; k++)
		{
			if (k != 0 && withDraw)
			{
				Gizmos.color = (this._forward[0] ? Color.green : Color.red);
				Gizmos.DrawLine(this.points[0, k], this.points[0, k - 1]);
			}
		}
		if (this.loopPath && withDraw)
		{
			Gizmos.color = (this._forward[0] ? Color.green : Color.red);
			Gizmos.DrawLine(this.points[0, 1], this.points[0, this.pointLength[0] - 2]);
		}
		for (int l = 1; l < this.numberOfWays; l++)
		{
			if (this.numberOfWays > 1)
			{
				if (!this.loopPath)
				{
					Vector3 vector = this.points[0, 2] - this.points[0, 1];
					Vector3 vector2 = vector;
					vector2 = Quaternion.Euler(0f, -90f, 0f) * vector2;
					if (l % 2 == 0)
					{
						vector2 = vector2.normalized * ((float)l * 0.5f * this.lineSpacing);
					}
					else if (l % 2 == 1)
					{
						vector2 = vector2.normalized * ((float)(l + 1) * 0.5f * this.lineSpacing);
					}
					Vector3 vector3 = Vector3.zero;
					if (l % 2 == 1)
					{
						vector3 = this.points[0, 1] - vector2;
					}
					else if (l % 2 == 0)
					{
						vector3 = this.points[0, 1] + vector2;
					}
					vector3.y = this.points[0, 1].y;
					this.points[l, 0] = vector3;
					this.points[l, 1] = vector3;
					Vector3 vector4 = this.points[0, this.pointLength[0] - 3] - this.points[0, this.pointLength[0] - 2];
					Vector3 vector5 = vector4;
					vector5 = Quaternion.Euler(0f, 90f, 0f) * vector5;
					if (l % 2 == 0)
					{
						vector5 = vector5.normalized * ((float)l * 0.5f * this.lineSpacing);
					}
					else if (l % 2 == 1)
					{
						vector5 = vector5.normalized * ((float)(l + 1) * 0.5f * this.lineSpacing);
					}
					Vector3 vector6 = Vector3.zero;
					if (l % 2 == 1)
					{
						vector6 = this.points[0, this.pointLength[0] - 2] - vector5;
					}
					else if (l % 2 == 0)
					{
						vector6 = this.points[0, this.pointLength[0] - 2] + vector5;
					}
					vector6.y = this.points[0, this.pointLength[0] - 2].y;
					this.points[l, this.pointLength[0] - 2] = vector6;
					this.points[l, this.pointLength[0] - 1] = vector6;
				}
				else
				{
					Vector3 vector7 = this.points[0, this.pointLength[0] - 2] - this.points[0, 1];
					Vector3 vector8 = this.points[0, 1] - this.points[0, 2];
					Vector3 vector9 = vector8;
					Vector3 vector10 = vector7;
					float num = Mathf.DeltaAngle(Mathf.Atan2(vector9.x, vector9.z) * 57.29578f, Mathf.Atan2(vector10.x, vector10.z) * 57.29578f);
					if (l % 2 == 0)
					{
						vector9 = vector9.normalized * ((float)l * 0.5f * this.lineSpacing);
					}
					else if (l % 2 == 1)
					{
						vector9 = vector9.normalized * ((float)(l + 1) * 0.5f * this.lineSpacing);
					}
					vector9 = Quaternion.Euler(0f, 90f + num / 2f, 0f) * vector9;
					Vector3 vector11 = Vector3.zero;
					if (l % 2 == 1)
					{
						vector11 = this.points[0, 1] - vector9;
					}
					else if (l % 2 == 0)
					{
						vector11 = this.points[0, 1] + vector9;
					}
					vector11.y = this.points[0, 1].y;
					this.points[l, 1] = vector11;
					this.points[l, 0] = vector11;
					Vector3 vector12 = this.points[0, this.pointLength[0] - 2] - this.points[0, 1];
					Vector3 vector13 = this.points[0, this.pointLength[0] - 3] - this.points[0, this.pointLength[0] - 2];
					Vector3 vector14 = vector13;
					Vector3 vector15 = vector12;
					float num2 = Mathf.DeltaAngle(Mathf.Atan2(vector14.x, vector14.z) * 57.29578f, Mathf.Atan2(vector15.x, vector15.z) * 57.29578f);
					if (l % 2 == 0)
					{
						vector14 = vector14.normalized * ((float)l * 0.5f * this.lineSpacing);
					}
					else if (l % 2 == 1)
					{
						vector14 = vector14.normalized * ((float)(l + 1) * 0.5f * this.lineSpacing);
					}
					vector14 = Quaternion.Euler(0f, 90f + num2 / 2f, 0f) * vector14;
					Vector3 vector16 = Vector3.zero;
					if (l % 2 == 1)
					{
						vector16 = this.points[0, this.pointLength[0] - 2] - vector14;
					}
					else if (l % 2 == 0)
					{
						vector16 = this.points[0, this.pointLength[0] - 2] + vector14;
					}
					vector16.y = this.points[0, this.pointLength[0] - 2].y;
					this.points[l, this.pointLength[0] - 2] = vector16;
					this.points[l, this.pointLength[0] - 1] = vector16;
				}
				for (int m = 2; m < this.pointLength[0] - 2; m++)
				{
					Vector3 vector17 = this.points[0, m] - this.points[0, m + 1];
					Vector3 vector18 = this.points[0, m - 1] - this.points[0, m];
					Vector3 vector19 = vector18;
					Vector3 vector20 = vector17;
					float num3 = Mathf.DeltaAngle(Mathf.Atan2(vector19.x, vector19.z) * 57.29578f, Mathf.Atan2(vector20.x, vector20.z) * 57.29578f);
					if (l % 2 == 0)
					{
						vector19 = vector19.normalized * ((float)l * 0.5f * this.lineSpacing);
					}
					else if (l % 2 == 1)
					{
						vector19 = vector19.normalized * ((float)(l + 1) * 0.5f * this.lineSpacing);
					}
					vector19 = Quaternion.Euler(0f, 90f + num3 / 2f, 0f) * vector19;
					Vector3 vector21 = Vector3.zero;
					if (l % 2 == 1)
					{
						vector21 = this.points[0, m] - vector19;
					}
					else if (l % 2 == 0)
					{
						vector21 = this.points[0, m] + vector19;
					}
					vector21.y = this.points[0, m].y;
					this.points[l, m] = vector21;
					if (withDraw)
					{
						Gizmos.color = (this._forward[l] ? Color.green : Color.red);
						Gizmos.DrawLine(this.points[l, m - 1], this.points[l, m]);
					}
				}
				if (withDraw)
				{
					Gizmos.color = (this._forward[l] ? Color.green : Color.red);
					Gizmos.DrawLine(this.points[l, this.pointLength[0] - 2], this.points[l, this.pointLength[0] - 3]);
				}
				if (withDraw && this.loopPath)
				{
					Gizmos.color = (this._forward[l] ? Color.green : Color.red);
					Gizmos.DrawLine(this.points[l, 1], this.points[l, this.pointLength[0] - 2]);
				}
			}
		}
	}

	// Token: 0x06000035 RID: 53 RVA: 0x00005614 File Offset: 0x00003814
	public void SpawnOnePeople(int w, bool forward, float mSpeed)
	{
		int num = Random.Range(0, this.peoplePrefabs.Length);
		GameObject gameObject = base.gameObject;
		if (!forward)
		{
			gameObject = (GameObject.Instantiate(this.peoplePrefabs[num], this.points[w, this.pointLength[0] - 2], Quaternion.identity) as GameObject);
		}
		else
		{
			gameObject = (GameObject.Instantiate(this.peoplePrefabs[num], this.points[w, 1], Quaternion.identity) as GameObject);
		}
		MovePath movePath = gameObject.AddComponent<MovePath>();
		gameObject.transform.parent = this.par.transform;
		movePath.walkPath = base.gameObject;
		string anim;
		if (this.isWalk)
		{
			anim = "walk";
		}
		else
		{
			anim = "run";
		}
		if (!forward)
		{
			movePath.MyStart(w, this.pointLength[0] - 2, anim, this.loopPath, forward, mSpeed);
			gameObject.transform.LookAt(this.points[w, this.pointLength[0] - 3]);
			return;
		}
		movePath.MyStart(w, 1, anim, this.loopPath, forward, mSpeed);
		gameObject.transform.LookAt(this.points[w, 2]);
	}

	// Token: 0x06000036 RID: 54 RVA: 0x0000574C File Offset: 0x0000394C
	public void SpawnPeople()
	{
		if (this.par == null)
		{
			this.par = new GameObject();
			this.par.transform.parent = base.gameObject.transform;
			this.par.name = "people";
		}
		int num;
		if (!this.loopPath)
		{
			num = this.pointLength[0] - 2;
		}
		else
		{
			num = this.pointLength[0] - 1;
		}
		for (int i = 0; i < this.numberOfWays; i++)
		{
			float num2 = this.moveSpeed + Random.Range(this.moveSpeed * -0.15f, this.moveSpeed * 0.15f);
			for (int j = 1; j < num; j++)
			{
				bool flag = false;
				if (this.direction.ToString() == "Forward")
				{
					flag = true;
				}
				else if (this.direction.ToString() == "Backward")
				{
					flag = false;
				}
				else if (this.direction.ToString() == "HugLeft")
				{
					flag = ((i + 2) % 2 == 0);
				}
				else if (this.direction.ToString() == "HugRight")
				{
					flag = ((i + 2) % 2 != 0);
				}
				else if (this.direction.ToString() == "WeaveLeft")
				{
					flag = (i != 1 && i != 2 && (i - 1) % 4 != 0 && (i - 2) % 4 != 0);
				}
				else if (this.direction.ToString() == "WeaveRight")
				{
					flag = (i == 1 || i == 2 || (i - 1) % 4 == 0 || (i - 2) % 4 == 0);
				}
				Vector3 vector = Vector3.zero;
				if (this.loopPath && j == num - 1)
				{
					vector = this.points[i, 1] - this.points[i, num];
				}
				else
				{
					vector = this.points[i, j + 1] - this.points[i, j];
				}
				float magnitude = vector.magnitude;
				int num3 = (int)(this.density / 5f * magnitude);
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
					int num7 = Random.Range(0, this.peoplePrefabs.Length);
					GameObject gameObject = base.gameObject;
					if (flag)
					{
						gameObject = (GameObject.Instantiate(this.peoplePrefabs[num7], this.points[i, j] + vector * (num6 / magnitude), Quaternion.identity) as GameObject);
					}
					else
					{
						gameObject = (GameObject.Instantiate(this.peoplePrefabs[num7], this.points[i, j] + vector * (num6 / magnitude), Quaternion.identity) as GameObject);
					}
					MovePath movePath = gameObject.AddComponent<MovePath>();
					gameObject.transform.parent = this.par.transform;
					movePath.walkPath = base.gameObject;
					string anim;
					if (this.isWalk)
					{
						anim = "walk";
					}
					else
					{
						anim = "run";
					}
					movePath.MyStart(i, j, anim, this.loopPath, flag, num2);
				}
			}
		}
	}

	// Token: 0x04000048 RID: 72
	public GameObject[] peoplePrefabs;

	// Token: 0x04000049 RID: 73
	[HideInInspector]
	public List<Vector3> pathPoint = new List<Vector3>();

	// Token: 0x0400004A RID: 74
	[HideInInspector]
	public List<GameObject> pathPointTransform = new List<GameObject>();

	// Token: 0x0400004B RID: 75
	[HideInInspector]
	public Vector3[,] points;

	// Token: 0x0400004C RID: 76
	[HideInInspector]
	public List<Vector3> CalcPoint = new List<Vector3>();

	// Token: 0x0400004D RID: 77
	public int numberOfWays;

	// Token: 0x0400004E RID: 78
	[HideInInspector]
	public int[] pointLength = new int[10];

	// Token: 0x0400004F RID: 79
	public float lineSpacing;

	// Token: 0x04000050 RID: 80
	public float density;

	// Token: 0x04000051 RID: 81
	public bool loopPath;

	// Token: 0x04000052 RID: 82
	public WalkPath.EnumDir direction;

	// Token: 0x04000053 RID: 83
	public WalkPath.EnumMove _moveType;

	// Token: 0x04000054 RID: 84
	public float moveSpeed = 1f;

	// Token: 0x04000055 RID: 85
	[HideInInspector]
	public bool[] _forward;

	// Token: 0x04000056 RID: 86
	[HideInInspector]
	public bool isWalk;

	// Token: 0x04000057 RID: 87
	[HideInInspector]
	public GameObject par;

	// Token: 0x0200000C RID: 12
	public enum EnumDir
	{
		// Token: 0x04000059 RID: 89
		Forward,
		// Token: 0x0400005A RID: 90
		Backward,
		// Token: 0x0400005B RID: 91
		HugLeft,
		// Token: 0x0400005C RID: 92
		HugRight,
		// Token: 0x0400005D RID: 93
		WeaveLeft,
		// Token: 0x0400005E RID: 94
		WeaveRight
	}

	// Token: 0x0200000D RID: 13
	public enum EnumMove
	{
		// Token: 0x04000060 RID: 96
		Walk,
		// Token: 0x04000061 RID: 97
		Run
	}
}

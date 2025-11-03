using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// Token: 0x02000007 RID: 7
public class StandingPeopleConcert : MonoBehaviour
{
	// Token: 0x06000017 RID: 23 RVA: 0x00002DBC File Offset: 0x00000FBC
	public void OnDrawGizmos()
	{
		if (!this.isCircle)
		{
			this.surface.transform.localScale = new Vector3(this.planeSize.x, this.planeSize.y, 1f);
			return;
		}
		this.surface.transform.localScale = new Vector3(this.circleDiametr, this.circleDiametr, 1f);
	}

	// Token: 0x06000018 RID: 24 RVA: 0x00002E28 File Offset: 0x00001028
	public void SpawnRectangleSurface()
	{
		if (this.surface != null)
		{
			DestroyImmediate(this.surface);
		}
		GameObject gameObject = GameObject.Instantiate(this.planePrefab, base.transform.position, Quaternion.identity) as GameObject;
		this.surface = gameObject;
		this.isCircle = false;
		gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x - 90f, gameObject.transform.eulerAngles.y, gameObject.transform.eulerAngles.z);
		gameObject.transform.position += new Vector3(0f, 0.01f, 0f);
		gameObject.transform.parent = base.transform;
		gameObject.name = "surface";
	}

	// Token: 0x06000019 RID: 25 RVA: 0x00002F0C File Offset: 0x0000110C
	public void SpawnCircleSurface()
	{
		if (this.surface != null)
		{
			DestroyImmediate(this.surface);
		}
		GameObject gameObject = GameObject.Instantiate(this.circlePrefab, base.transform.position, Quaternion.identity) as GameObject;
		this.isCircle = true;
		gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x - 90f, gameObject.transform.eulerAngles.y, gameObject.transform.eulerAngles.z);
		gameObject.transform.position += new Vector3(0f, 0.01f, 0f);
		gameObject.transform.parent = base.transform;
		gameObject.name = "surface";
		this.surface = gameObject;
	}

	// Token: 0x0600001A RID: 26 RVA: 0x00002FED File Offset: 0x000011ED
	public void RemoveButton()
	{
		if (this.par != null)
		{
			DestroyImmediate(this.par);
		}
	}

	// Token: 0x0600001B RID: 27 RVA: 0x00003008 File Offset: 0x00001208
	public void PopulateButton()
	{
		this.RemoveButton();
		GameObject gameObject = new GameObject();
		this.par = gameObject;
		gameObject.transform.parent = base.gameObject.transform;
		gameObject.name = "people";
		this.spawnPoints.Clear();
		this.SpawnPeople(this.peopleCount);
	}

	// Token: 0x0600001C RID: 28 RVA: 0x00003060 File Offset: 0x00001260
	private void SpawnPeople(int _peopleCount)
	{
		for (int i = 0; i < _peopleCount; i++)
		{
			int num = Random.Range(0, this.peoplePrefabs.Length);
			Vector3 vector;
			if (!this.isCircle)
			{
				vector = this.RandomRectanglePosition();
			}
			else
			{
				vector = this.RandomCirclePosition();
			}
			if (vector != Vector3.zero)
			{
				GameObject gameObject = GameObject.Instantiate(this.peoplePrefabs[num], vector, Quaternion.identity) as GameObject;
				RaycastHit raycastHit;
				if (Physics.Raycast(gameObject.transform.position + new Vector3(0f, 2f, 0f), -gameObject.transform.up, out raycastHit))
				{
					gameObject.transform.position = new Vector3(gameObject.transform.position.x, raycastHit.point.y, gameObject.transform.position.z);
				}
				gameObject.AddComponent<PeopleController>();
				this.spawnPoints.Add(gameObject.transform.position);
				if (this.target != null)
				{
					gameObject.GetComponent<PeopleController>().SetTarget(this.target.transform.position);
				}
				else
				{
					gameObject.transform.localEulerAngles = new Vector3(gameObject.transform.rotation.x, base.transform.rotation.y, gameObject.transform.rotation.z);
				}
				gameObject.GetComponent<PeopleController>().animNames = new string[]
				{
					"idle1",
					"idle2",
					"cheer",
					"claphands"
				};
				gameObject.transform.parent = this.par.transform;
			}
		}
	}

	// Token: 0x0600001D RID: 29 RVA: 0x00003220 File Offset: 0x00001420
	private Vector3 RandomRectanglePosition()
	{
		Vector3 vector;
		vector = new Vector3(0f, 0f, 0f);
		for (int i = 0; i < 10; i++)
		{
			vector.x = this.surface.transform.position.x - this.GetRealPlaneSize().x / 2f + 0.3f + Random.Range(0f, this.GetRealPlaneSize().x - 0.6f);
			vector.z = this.surface.transform.position.z - this.GetRealPlaneSize().y / 2f + 0.3f + Random.Range(0f, this.GetRealPlaneSize().y - 0.6f);
			vector.y = this.surface.transform.position.y;
			if (this.IsRandomPositionFree(vector))
			{
				return vector;
			}
		}
		return Vector3.zero;
	}

	// Token: 0x0600001E RID: 30 RVA: 0x00003324 File Offset: 0x00001524
	private Vector3 RandomCirclePosition()
	{
		Vector3 position = this.surface.transform.position;
		float num = this.GetRealPlaneSize().x / 2f;
		for (int i = 0; i < 10; i++)
		{
			float num2 = Random.value * num;
			float num3 = Random.value * 360f;
			Vector3 vector;
			vector.x = position.x + num2 * Mathf.Sin(num3 * 0.0174532924f);
			vector.y = position.y;
			vector.z = position.z + num2 * Mathf.Cos(num3 * 0.0174532924f);
			if (this.IsRandomPositionFree(vector))
			{
				return vector;
			}
		}
		return Vector3.zero;
	}

	// Token: 0x0600001F RID: 31 RVA: 0x000033D4 File Offset: 0x000015D4
	private bool IsRandomPositionFree(Vector3 pos)
	{
		for (int i = 0; i < this.spawnPoints.Count; i++)
		{
			if (this.spawnPoints[i].x - 0.6f < pos.x && this.spawnPoints[i].x + 1f > pos.x && this.spawnPoints[i].z - 0.5f < pos.z && this.spawnPoints[i].z + 0.6f > pos.z)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000020 RID: 32 RVA: 0x00003480 File Offset: 0x00001680
	private Vector2 GetRealPlaneSize()
	{
		Vector3 size = this.surface.GetComponent<MeshRenderer>().bounds.size;
		return new Vector2(size.x, size.z);
	}

	// Token: 0x06000021 RID: 33 RVA: 0x000034BC File Offset: 0x000016BC
	private Vector2 GetRealPeopleModelSize()
	{
		Vector3 size = this.peoplePrefabs[1].GetComponent<MeshRenderer>().bounds.size;
		return new Vector2(size.x, size.z);
	}

	// Token: 0x04000029 RID: 41
	[HideInInspector]
	public GameObject planePrefab;

	// Token: 0x0400002A RID: 42
	[HideInInspector]
	public GameObject circlePrefab;

	// Token: 0x0400002B RID: 43
	[HideInInspector]
	public GameObject surface;

	// Token: 0x0400002C RID: 44
	[HideInInspector]
	public Vector2 planeSize = new Vector2(1f, 1f);

	// Token: 0x0400002D RID: 45
	public GameObject[] peoplePrefabs = new GameObject[0];

	// Token: 0x0400002E RID: 46
	[HideInInspector]
	private List<Vector3> spawnPoints = new List<Vector3>();

	// Token: 0x0400002F RID: 47
	[HideInInspector]
	public GameObject target;

	// Token: 0x04000030 RID: 48
	[HideInInspector]
	public int peopleCount;

	// Token: 0x04000031 RID: 49
	[HideInInspector]
	public bool isCircle;

	// Token: 0x04000032 RID: 50
	[HideInInspector]
	public float circleDiametr = 1f;

	// Token: 0x04000033 RID: 51
	[HideInInspector]
	public bool showSurface = true;

	// Token: 0x04000034 RID: 52
	public StandingPeopleConcert.TestEnum SurfaceType;

	// Token: 0x04000035 RID: 53
	[HideInInspector]
	public GameObject par;

	// Token: 0x02000008 RID: 8
	public enum TestEnum
	{
		// Token: 0x04000037 RID: 55
		Rectangle,
		// Token: 0x04000038 RID: 56
		Circle
	}
}

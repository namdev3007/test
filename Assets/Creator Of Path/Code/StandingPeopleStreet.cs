using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// Token: 0x02000009 RID: 9
public class StandingPeopleStreet : MonoBehaviour
{
	// Token: 0x06000023 RID: 35 RVA: 0x0000354C File Offset: 0x0000174C
	public void OnDrawGizmos()
	{
		if (!this.isCircle)
		{
			this.surface.transform.localScale = new Vector3(this.planeSize.x, this.planeSize.y, 1f);
			return;
		}
		this.surface.transform.localScale = new Vector3(this.circleDiametr, this.circleDiametr, 1f);
	}

	// Token: 0x06000024 RID: 36 RVA: 0x000035B8 File Offset: 0x000017B8
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

	// Token: 0x06000025 RID: 37 RVA: 0x0000369C File Offset: 0x0000189C
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

	// Token: 0x06000026 RID: 38 RVA: 0x0000377D File Offset: 0x0000197D
	public void RemoveButton()
	{
		if (this.par != null)
		{
			DestroyImmediate(this.par);
		}
		this.par = null;
	}

	// Token: 0x06000027 RID: 39 RVA: 0x000037A0 File Offset: 0x000019A0
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

	// Token: 0x06000028 RID: 40 RVA: 0x000037F8 File Offset: 0x000019F8
	private void SpawnPeople(int _peopleCount)
	{
		int num = Random.Range(0, _peopleCount / 3) * 3;
		int num2 = Random.Range(0, (_peopleCount - num) / 2) * 2;
		int num3 = _peopleCount - num - num2;
		for (int i = 0; i < num3; i++)
		{
			int num4 = Random.Range(0, this.peoplePrefabs.Length);
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
				GameObject gameObject = GameObject.Instantiate(this.peoplePrefabs[num4], vector, Quaternion.identity) as GameObject;
				RaycastHit raycastHit;
				if (Physics.Raycast(gameObject.transform.position + new Vector3(0f, 2f, 0f), -gameObject.transform.up, out raycastHit))
				{
					gameObject.transform.position = new Vector3(gameObject.transform.position.x, raycastHit.point.y, gameObject.transform.position.z);
				}
				gameObject.AddComponent<PeopleController>();
				this.spawnPoints.Add(gameObject.transform.position);
				gameObject.transform.localEulerAngles = new Vector3(gameObject.transform.rotation.x, (float)Random.Range(1, 359), gameObject.transform.rotation.z);
				gameObject.GetComponent<PeopleController>().animNames = new string[]
				{
					"idle1",
					"idle2"
				};
				gameObject.transform.parent = this.par.transform;
			}
		}
		for (int j = 0; j < num2 / 2; j++)
		{
			Vector3 vector2;
			if (!this.isCircle)
			{
				vector2 = this.RandomRectanglePosition();
			}
			else
			{
				vector2 = this.RandomCirclePosition();
			}
			if (vector2 != Vector3.zero)
			{
				Vector3 vector3 = Vector3.zero;
				Vector3 vector4 = Vector3.zero;
				for (int k = 0; k < 100; k++)
				{
					for (int l = 0; l < 10; l++)
					{
						vector3 = vector2 + new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f));
						if (this.IsRandomPositionFree(vector3, Vector3.zero, Vector3.zero))
						{
							break;
						}
						vector3 = Vector3.zero;
					}
					for (int m = 0; m < 10; m++)
					{
						vector4 = vector2 + new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f));
						if (this.IsRandomPositionFree(vector4, vector3, Vector3.zero))
						{
							break;
						}
						vector4 = Vector3.zero;
					}
					if (vector3 != Vector3.zero && vector4 != Vector3.zero)
					{
						this.spawnPoints.Add(vector3);
						this.spawnPoints.Add(vector4);
						break;
					}
					vector3 = Vector3.zero;
					vector4 = Vector3.zero;
				}
				if (vector3 != Vector3.zero && vector4 != Vector3.zero)
				{
					int num5 = Random.Range(0, this.peoplePrefabs.Length);
					GameObject gameObject2 = base.gameObject;
					gameObject2 = (GameObject.Instantiate(this.peoplePrefabs[num5], vector3, Quaternion.identity) as GameObject);
					RaycastHit raycastHit2;
					if (Physics.Raycast(gameObject2.transform.position + new Vector3(0f, 2f, 0f), -gameObject2.transform.up, out raycastHit2))
					{
						gameObject2.transform.position = new Vector3(gameObject2.transform.position.x, raycastHit2.point.y, gameObject2.transform.position.z);
					}
					gameObject2.AddComponent<PeopleController>();
					gameObject2.GetComponent<PeopleController>().animNames = new string[]
					{
						"talk1",
						"talk2",
						"listen"
					};
					gameObject2.transform.parent = this.par.transform;
					num5 = Random.Range(0, this.peoplePrefabs.Length);
					GameObject gameObject3 = base.gameObject;
					gameObject3 = (GameObject.Instantiate(this.peoplePrefabs[num5], vector4, Quaternion.identity) as GameObject);
					RaycastHit raycastHit3;
					if (Physics.Raycast(gameObject3.transform.position + new Vector3(0f, 2f, 0f), -gameObject3.transform.up, out raycastHit3))
					{
						gameObject3.transform.position = new Vector3(gameObject3.transform.position.x, raycastHit3.point.y, gameObject3.transform.position.z);
					}
					gameObject3.AddComponent<PeopleController>();
					gameObject3.GetComponent<PeopleController>().animNames = new string[]
					{
						"talk1",
						"talk2",
						"listen"
					};
					gameObject3.transform.parent = this.par.transform;
					gameObject3.GetComponent<PeopleController>().SetTarget(gameObject2.transform.position);
					gameObject2.GetComponent<PeopleController>().SetTarget(gameObject3.transform.position);
				}
			}
		}
		for (int n = 0; n < num / 3; n++)
		{
			Vector3 vector5;
			if (!this.isCircle)
			{
				vector5 = this.RandomRectanglePosition();
			}
			else
			{
				vector5 = this.RandomCirclePosition();
			}
			if (vector5 != Vector3.zero)
			{
				int num6 = Random.Range(0, this.peoplePrefabs.Length);
				Vector3 vector6 = Vector3.zero;
				Vector3 vector7 = Vector3.zero;
				Vector3 vector8 = Vector3.zero;
				for (int num7 = 0; num7 < 100; num7++)
				{
					for (int num8 = 0; num8 < 10; num8++)
					{
						vector6 = vector5 + new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f));
						if (this.IsRandomPositionFree(vector6, Vector3.zero, Vector3.zero))
						{
							break;
						}
						vector6 = Vector3.zero;
					}
					for (int num9 = 0; num9 < 10; num9++)
					{
						if (vector6 != Vector3.zero)
						{
							vector7 = vector5 + new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f));
							if (this.IsRandomPositionFree(vector7, vector6, Vector3.zero))
							{
								break;
							}
							vector7 = Vector3.zero;
						}
						else
						{
							vector7 = Vector3.zero;
						}
					}
					for (int num10 = 0; num10 < 10; num10++)
					{
						if (vector7 != Vector3.zero && vector6 != Vector3.zero)
						{
							vector8 = vector5 + new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f));
							if (this.IsRandomPositionFree(vector8, vector6, vector7))
							{
								break;
							}
							vector8 = Vector3.zero;
						}
						else
						{
							vector8 = Vector3.zero;
						}
					}
					if (vector6 != Vector3.zero && vector7 != Vector3.zero && vector8 != Vector3.zero)
					{
						this.spawnPoints.Add(vector6);
						this.spawnPoints.Add(vector7);
						this.spawnPoints.Add(vector8);
						break;
					}
					vector6 = Vector3.zero;
					vector7 = Vector3.zero;
					vector8 = Vector3.zero;
				}
				if (vector6 != Vector3.zero)
				{
					if (vector6 != Vector3.zero)
					{
						GameObject gameObject4 = GameObject.Instantiate(this.peoplePrefabs[num6], vector6, Quaternion.identity) as GameObject;
						RaycastHit raycastHit4;
						if (Physics.Raycast(gameObject4.transform.position + new Vector3(0f, 2f, 0f), -gameObject4.transform.up, out raycastHit4))
						{
							gameObject4.transform.position = new Vector3(gameObject4.transform.position.x, raycastHit4.point.y, gameObject4.transform.position.z);
						}
						gameObject4.AddComponent<PeopleController>();
						gameObject4.GetComponent<PeopleController>().SetTarget(vector5);
						gameObject4.GetComponent<PeopleController>().animNames = new string[]
						{
							"talk1",
							"talk2",
							"listen"
						};
						gameObject4.transform.parent = this.par.transform;
					}
					num6 = Random.Range(0, this.peoplePrefabs.Length);
					if (vector6 != Vector3.zero)
					{
						GameObject gameObject5 = GameObject.Instantiate(this.peoplePrefabs[num6], vector7, Quaternion.identity) as GameObject;
						RaycastHit raycastHit5;
						if (Physics.Raycast(gameObject5.transform.position + new Vector3(0f, 2f, 0f), -gameObject5.transform.up, out raycastHit5))
						{
							gameObject5.transform.position = new Vector3(gameObject5.transform.position.x, raycastHit5.point.y, gameObject5.transform.position.z);
						}
						gameObject5.AddComponent<PeopleController>();
						gameObject5.GetComponent<PeopleController>().SetTarget(vector5);
						gameObject5.GetComponent<PeopleController>().animNames = new string[]
						{
							"talk1",
							"talk2",
							"listen"
						};
						gameObject5.transform.parent = this.par.transform;
					}
					num6 = Random.Range(0, this.peoplePrefabs.Length);
					if (vector6 != Vector3.zero)
					{
						GameObject gameObject6 = GameObject.Instantiate(this.peoplePrefabs[num6], vector8, Quaternion.identity) as GameObject;
						RaycastHit raycastHit6;
						if (Physics.Raycast(gameObject6.transform.position + new Vector3(0f, 2f, 0f), -gameObject6.transform.up, out raycastHit6))
						{
							gameObject6.transform.position = new Vector3(gameObject6.transform.position.x, raycastHit6.point.y, gameObject6.transform.position.z);
						}
						gameObject6.AddComponent<PeopleController>();
						gameObject6.GetComponent<PeopleController>().SetTarget(vector5);
						gameObject6.GetComponent<PeopleController>().animNames = new string[]
						{
							"talk1",
							"talk2",
							"listen"
						};
						gameObject6.transform.parent = this.par.transform;
					}
				}
			}
		}
	}

	// Token: 0x06000029 RID: 41 RVA: 0x000042E4 File Offset: 0x000024E4
	private Vector3 RandomRectanglePosition()
	{
		Vector3 vector;
		vector = new Vector3(0f, 0f, 0f);
		for (int i = 0; i < 10; i++)
		{
			vector.x = this.surface.transform.position.x - this.GetRealPlaneSize().x / 2f + 0.3f + Random.Range(0f, this.GetRealPlaneSize().x - 0.6f);
			vector.z = this.surface.transform.position.z - this.GetRealPlaneSize().y / 2f + 0.3f + Random.Range(0f, this.GetRealPlaneSize().y - 0.6f);
			vector.y = this.surface.transform.position.y;
			if (this.IsRandomPositionFree(vector, Vector3.zero, Vector3.zero))
			{
				return vector;
			}
		}
		return Vector3.zero;
	}

	// Token: 0x0600002A RID: 42 RVA: 0x000043F0 File Offset: 0x000025F0
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
			if (Vector3.Distance(vector, position) < this.GetRealPlaneSize().x / 2f - 0.3f && this.IsRandomPositionFree(vector, Vector3.zero, Vector3.zero))
			{
				return vector;
			}
		}
		return Vector3.zero;
	}

	// Token: 0x0600002B RID: 43 RVA: 0x000044D0 File Offset: 0x000026D0
	private bool IsRandomPositionFree(Vector3 pos, Vector3 helpPoint1, Vector3 helpPoint2)
	{
		for (int i = 0; i < this.spawnPoints.Count; i++)
		{
			if (this.spawnPoints[i].x - 0.5f < pos.x && this.spawnPoints[i].x + 0.5f > pos.x && this.spawnPoints[i].z - 0.5f < pos.z && this.spawnPoints[i].z + 0.5f > pos.z)
			{
				return false;
			}
		}
		if (helpPoint1 != Vector3.zero)
		{
			if (helpPoint1.x - 0.6f < pos.x && helpPoint1.x + 0.6f > pos.x && helpPoint1.z - 0.6f < pos.z && helpPoint1.z + 0.6f > pos.z)
			{
				return false;
			}
			if (!this.isCircle)
			{
				if (helpPoint1.x + 0.3f <= this.surface.transform.position.x - this.GetRealPlaneSize().x / 2f && helpPoint1.x - 0.3f >= this.surface.transform.position.x + this.GetRealPlaneSize().x / 2f && helpPoint1.z + 0.3f <= this.surface.transform.position.z - this.GetRealPlaneSize().y / 2f && helpPoint1.z - 0.3f >= this.surface.transform.position.z + this.GetRealPlaneSize().y / 2f)
				{
					return false;
				}
			}
			else if (Vector3.Distance(helpPoint1, this.surface.transform.position) >= this.GetRealPlaneSize().x / 2f - 0.3f)
			{
				return false;
			}
		}
		if (helpPoint2 != Vector3.zero)
		{
			if (helpPoint2.x - 0.6f < pos.x && helpPoint2.x + 0.6f > pos.x && helpPoint2.z - 0.6f < pos.z && helpPoint2.z + 0.6f > pos.z)
			{
				return false;
			}
			if (!this.isCircle)
			{
				if (helpPoint2.x + 0.3f <= this.surface.transform.position.x - this.GetRealPlaneSize().x / 2f && helpPoint2.x - 0.3f >= this.surface.transform.position.x + this.GetRealPlaneSize().x / 2f && helpPoint2.z + 0.3f <= this.surface.transform.position.z - this.GetRealPlaneSize().y / 2f && helpPoint2.z - 0.3f >= this.surface.transform.position.z + this.GetRealPlaneSize().y / 2f)
				{
					return false;
				}
			}
			else if (Vector3.Distance(helpPoint2, this.surface.transform.position) >= this.GetRealPlaneSize().x / 2f - 0.3f)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600002C RID: 44 RVA: 0x00004888 File Offset: 0x00002A88
	private Vector2 GetRealPlaneSize()
	{
		Vector3 size = this.surface.GetComponent<MeshRenderer>().bounds.size;
		return new Vector2(size.x, size.z);
	}

	// Token: 0x0600002D RID: 45 RVA: 0x000048C4 File Offset: 0x00002AC4
	private Vector2 GetRealPeopleModelSize()
	{
		Vector3 size = this.peoplePrefabs[1].GetComponent<MeshRenderer>().bounds.size;
		return new Vector2(size.x, size.z);
	}

	// Token: 0x04000039 RID: 57
	[HideInInspector]
	public GameObject planePrefab;

	// Token: 0x0400003A RID: 58
	[HideInInspector]
	public GameObject circlePrefab;

	// Token: 0x0400003B RID: 59
	[HideInInspector]
	public GameObject surface;

	// Token: 0x0400003C RID: 60
	[HideInInspector]
	public Vector2 planeSize = new Vector2(1f, 1f);

	// Token: 0x0400003D RID: 61
	public GameObject[] peoplePrefabs = new GameObject[0];

	// Token: 0x0400003E RID: 62
	[HideInInspector]
	public List<Vector3> spawnPoints = new List<Vector3>();

	// Token: 0x0400003F RID: 63
	[HideInInspector]
	public int peopleCount;

	// Token: 0x04000040 RID: 64
	[HideInInspector]
	public bool isCircle;

	// Token: 0x04000041 RID: 65
	[HideInInspector]
	public float circleDiametr = 1f;

	// Token: 0x04000042 RID: 66
	[HideInInspector]
	public bool showSurface = true;

	// Token: 0x04000043 RID: 67
	public StandingPeopleStreet.TestEnum SurfaceType;

	// Token: 0x04000044 RID: 68
	[HideInInspector]
	public GameObject par;

	// Token: 0x0200000A RID: 10
	public enum TestEnum
	{
		// Token: 0x04000046 RID: 70
		Rectangle,
		// Token: 0x04000047 RID: 71
		Circle
	}
}

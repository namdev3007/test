using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Token: 0x02000004 RID: 4
public class NewPath : MonoBehaviour
{
	// Token: 0x0600000A RID: 10 RVA: 0x00002A80 File Offset: 0x00000C80
	public List<Vector3> PointsGet()
	{
		return this.points;
	}

	// Token: 0x0600000B RID: 11 RVA: 0x00002A88 File Offset: 0x00000C88
	public void PointSet(int index, Vector3 pos)
	{
		this.points.Add(pos);
		if (this.par == null)
		{
			this.par = new GameObject();
			this.par.name = "New path points";
			this.par.transform.parent = base.gameObject.transform;
		}
		GameObject pointPrefab = GameObject.Find("Population System").GetComponent<PopulationSystemManager>().pointPrefab;
		GameObject gameObject = GameObject.Instantiate(pointPrefab, pos, Quaternion.identity) as GameObject;
		gameObject.name = "p" + index;
		gameObject.transform.parent = this.par.transform;
	}

	// Token: 0x0600000C RID: 12 RVA: 0x00002B38 File Offset: 0x00000D38
	public void OnDrawGizmos()
	{
#if UNITY_EDITOR
        Selection.activeGameObject = gameObject;
        ActiveEditorTracker.sharedTracker.isLocked = true;
#endif
        Gizmos.color = Color.green;
		if (this.pointLenght > 0 && !this.exit)
		{
			Gizmos.DrawLine(this.points[this.pointLenght - 1], this.mousePos);
		}
		if (this.pointLenght > 1)
		{
			for (int i = 0; i < this.pointLenght - 1; i++)
			{
				Gizmos.DrawLine(this.points[i], this.points[i + 1]);
			}
		}
	}

	// Token: 0x0400001A RID: 26
	private List<Vector3> points = new List<Vector3>();

	// Token: 0x0400001B RID: 27
	public int pointLenght;

	// Token: 0x0400001C RID: 28
	public Vector3 mousePos;

	// Token: 0x0400001D RID: 29
	public string pathName;

	// Token: 0x0400001E RID: 30
	public bool errors;

	// Token: 0x0400001F RID: 31
	public bool exit;

	// Token: 0x04000020 RID: 32
	public GameObject par;
}

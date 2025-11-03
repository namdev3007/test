using System;
using UnityEditor;
using UnityEngine;

// Token: 0x02000006 RID: 6
public class PopulationSystemManager : MonoBehaviour
{
	// Token: 0x06000014 RID: 20 RVA: 0x00002CCC File Offset: 0x00000ECC
	public void Concert(Vector3 pos)
	{
		this.isConcert = false;
		GameObject gameObject = new GameObject();
		gameObject.transform.position = pos;
		gameObject.name = "Audience";
#if UNITY_EDITOR
        Selection.activeGameObject = gameObject;
#endif
        gameObject.AddComponent<StandingPeopleConcert>();
		StandingPeopleConcert component = gameObject.GetComponent<StandingPeopleConcert>();
		component.planePrefab = this.planePrefab;
		component.circlePrefab = this.circlePrefab;
		component.SpawnRectangleSurface();
#if UNITY_EDITOR
        Selection.activeGameObject = gameObject;
        ActiveEditorTracker.sharedTracker.isLocked = false;
#endif
    }

	// Token: 0x06000015 RID: 21 RVA: 0x00002D40 File Offset: 0x00000F40
	public void Street(Vector3 pos)
	{
		this.isStreet = false;
		GameObject gameObject = new GameObject();
		gameObject.transform.position = pos;
		gameObject.name = "Talking people";
#if UNITY_EDITOR
        Selection.activeGameObject = gameObject;
#endif
        gameObject.AddComponent<StandingPeopleStreet>();
		StandingPeopleStreet component = gameObject.GetComponent<StandingPeopleStreet>();
		component.planePrefab = this.planePrefab;
		component.circlePrefab = this.circlePrefab;
		component.SpawnRectangleSurface();
#if UNITY_EDITOR
        Selection.activeGameObject = gameObject;
        ActiveEditorTracker.sharedTracker.isLocked = false;
#endif
    }

	// Token: 0x04000023 RID: 35
	[SerializeField]
	private GameObject planePrefab;

	// Token: 0x04000024 RID: 36
	[SerializeField]
	private GameObject circlePrefab;

	// Token: 0x04000025 RID: 37
	public GameObject pointPrefab;

	// Token: 0x04000026 RID: 38
	[HideInInspector]
	public bool isConcert;

	// Token: 0x04000027 RID: 39
	[HideInInspector]
	public bool isStreet;

	// Token: 0x04000028 RID: 40
	[HideInInspector]
	public Vector3 mousePos;
}

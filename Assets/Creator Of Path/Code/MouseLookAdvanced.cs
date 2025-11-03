using System;
using UnityEngine;

// Token: 0x02000002 RID: 2
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLookAdvanced : MonoBehaviour
{
	// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
	private void Start()
	{
		this.rotationY = -base.transform.localEulerAngles.x;
		this.rotationX = base.transform.localEulerAngles.y;
		this.smoothRotationX = base.transform.localEulerAngles.y;
		this.smoothRotationY = -base.transform.localEulerAngles.x;
	}

	// Token: 0x17000001 RID: 1
	// (get) Token: 0x06000002 RID: 2 RVA: 0x000020B7 File Offset: 0x000002B7
	// (set) Token: 0x06000003 RID: 3 RVA: 0x000020BE File Offset: 0x000002BE
	private bool IsCursorLock
	{
		get
		{
			return Screen.lockCursor;
		}
		set
		{
			Screen.lockCursor = value;
		}
	}

	// Token: 0x06000004 RID: 4 RVA: 0x000020C8 File Offset: 0x000002C8
	private void Update()
	{
		this.verticalAcceleration = 0f;
		if (Input.GetMouseButtonDown(1))
		{
			Screen.lockCursor = !Screen.lockCursor;
			Cursor.visible = !Cursor.visible;
		}
		if (Input.GetKey(KeyCode.Space))
		{
			this.verticalAcceleration = 1f;
		}
		if (Input.GetKey(KeyCode.LeftShift))
		{
			this.verticalAcceleration = -1f;
		}
		if (!this.IsCursorLock)
		{
			return;
		}
		this.rotationX += Input.GetAxis("Mouse X") * this.sensitivityX;
		this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY;
		this.rotationY = Mathf.Clamp(this.rotationY, this.minimumY, this.maximumY);
		this.smoothRotationX += (this.rotationX - this.smoothRotationX) * this.smoothSpeed * Time.smoothDeltaTime;
		this.smoothRotationY += (this.rotationY - this.smoothRotationY) * this.smoothSpeed * Time.smoothDeltaTime;
		base.transform.localEulerAngles = new Vector3(-this.smoothRotationY, this.smoothRotationX, 0f);
		Vector3 vector;
		vector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
		Vector3 vector2 = base.transform.rotation * vector;
		base.transform.position += vector2 * this.Speed * Time.smoothDeltaTime;
		base.transform.position += new Vector3(0f, this.Speed / 2f * this.verticalAcceleration * Time.smoothDeltaTime, 0f);
		base.transform.position += base.transform.rotation * Vector3.forward * Input.GetAxis("Mouse ScrollWheel") * 200f;
	}

	// Token: 0x04000001 RID: 1
	public float sensitivityX = 5f;

	// Token: 0x04000002 RID: 2
	public float sensitivityY = 5f;

	// Token: 0x04000003 RID: 3
	public float minimumX = -360f;

	// Token: 0x04000004 RID: 4
	public float maximumX = 360f;

	// Token: 0x04000005 RID: 5
	public float minimumY = -90f;

	// Token: 0x04000006 RID: 6
	public float maximumY = 90f;

	// Token: 0x04000007 RID: 7
	public float smoothSpeed = 20f;

	// Token: 0x04000008 RID: 8
	private float verticalAcceleration;

	// Token: 0x04000009 RID: 9
	private float rotationX;

	// Token: 0x0400000A RID: 10
	private float smoothRotationX;

	// Token: 0x0400000B RID: 11
	private float rotationY;

	// Token: 0x0400000C RID: 12
	private float smoothRotationY;

	// Token: 0x0400000D RID: 13
	private Vector3 vMousePos;

	// Token: 0x0400000E RID: 14
	public float Speed = 100f;

	// Token: 0x0400000F RID: 15
	private bool bActive;
}

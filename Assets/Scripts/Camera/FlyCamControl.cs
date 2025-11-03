using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FlyCamControl : MonoBehaviour
{
    public float xRotateSpeed = 200.0f;
    public float yRotateSpeed = 200.0f;
    public int yMinLimit = -80;
    public int yMaxLimit = 80;
    public float xMinMoveSpeed = 50.0f;
    public float xMaxMoveSpeed = 200.0f;
    public float yMinMoveSpeed = 100.0f;
    public float yMaxMoveSpeed = 500.0f;
    public float zoomDampening = 5.0f;
    public float speedMovingObservation;
    public float heightMovingObservation;

    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float xMoveSpeed;
    private float yMoveSpeed;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;

    private float inputHorizontal = 0;
    private float inputVertical = 0;
    private bool isShiftKeyDown = false;


    void Start() { Init(); }
    void OnEnable() { Init(); }

    /// <summary>
    /// On Init
    /// </summary>
    public void Init()
    {
        speedMovingObservation = 1;
        heightMovingObservation = 2;

        //be sure to grab the current rotations as starting points.
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;

        xDeg = transform.eulerAngles.y;
        yDeg = transform.eulerAngles.x;
    }


    /// <summary>
    /// Change Position
    /// </summary>
    public void ChangeCameraPosition()
    {
        xDeg = transform.eulerAngles.y;
        yDeg = transform.eulerAngles.x;
    }


    /// <summary>
    /// When Hold Shift Key
    /// </summary>
    public void Update()
    {
        isShiftKeyDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }


    /*
     * Camera logic on LateUpdate to only update after all character movement logic has been handled. 
     */
    void LateUpdate()
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            return;
        }

        // If Up Arrow or Down Arrow and Right button? ZOOM!
        if (Input.GetMouseButton(1))
        {
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow)
                || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)
                || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W)
                || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S))
            {
                if (IsUIInputFieldActive()) return;

                movementCamera();
            }
            rotationCamera();
        }
        else if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow)
                || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)
                || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W)
                || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S))

        {
            if (IsUIInputFieldActive()) return;

            movementCamera();
        }

        // Move Up and Down
        MoveUpAndDownObject();
    }


    /// <summary>
    /// Move Up And Down
    /// </summary>
    void MoveUpAndDownObject()
    {
        if (Input.GetKey(KeyCode.E)) // forward
        {
            if (IsUIInputFieldActive()) return;

            if (isShiftKeyDown == true)
            {
                transform.Translate(Vector3.up * Time.deltaTime * 20);
            }
            else
            {
                transform.Translate(Vector3.up * Time.deltaTime * 10);
            }
        }
        if (Input.GetKey(KeyCode.Q)) // backwards
        {
            if (IsUIInputFieldActive()) return;

            if (isShiftKeyDown == true)
            {
                transform.Translate(Vector3.down * Time.deltaTime * 20);
            }
            else
            {
                transform.Translate(Vector3.down * Time.deltaTime * 10);
            }
        }
    }


    /// <summary>
    /// Camera Move
    /// </summary>
    private void movementCamera()
    {
        if (isShiftKeyDown)
        {
            xMoveSpeed = xMaxMoveSpeed;
            yMoveSpeed = yMaxMoveSpeed;
        }
        else
        {
            xMoveSpeed = xMinMoveSpeed;
            yMoveSpeed = yMinMoveSpeed;
        }

        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");

        // Move Camera
        transform.Translate(new Vector3(inputHorizontal * yMoveSpeed, 0, inputVertical * xMoveSpeed) * Time.deltaTime);
    }


    /// <summary>
    /// Camera Rotation
    /// </summary>
    private void rotationCamera()
    {
        xDeg += Input.GetAxis("Mouse X") * xRotateSpeed * 0.02f;
        yDeg -= Input.GetAxis("Mouse Y") * yRotateSpeed * 0.02f;

        ////////OrbitAngle
        //Clamp the vertical axis for the orbit
        yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);

        // set camera rotation 
        desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
        currentRotation = transform.rotation;

        rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
        transform.rotation = rotation;


    }


    /// <summary>
    /// Clamp Angle
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }



    /// <summary>
    /// Prevent camera moving while focus in Input field
    /// </summary>
    public static bool IsUIInputFieldActive()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            UnityEngine.UI.InputField IF = EventSystem.current.currentSelectedGameObject.GetComponent<UnityEngine.UI.InputField>();

            return IF != null && IF.gameObject.activeInHierarchy;
        }
        return false;
    }
}

using System;
using Cinemachine;
using UnityEngine;

public class CameraSystem : MonoBehaviour {

    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    
    [SerializeField] private bool useEdgeScrolling = true;
    [SerializeField] private bool useDragPanMovement = true;
    
    [SerializeField] private float moveSpeed;
    
    [SerializeField] private float speedH = 2.0f;
    [SerializeField] private float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    
    [SerializeField] private int edgeScrollSize;
    [SerializeField] private float dragPanSpeed;
    [SerializeField] private float followOffsetMin = 5f;
    [SerializeField] private float followOffsetMax = 50f;
    [SerializeField] private float zoomSpeed = 10f;
    
    private bool dragPanMoveActive;
    private Vector2 lastMousePosition;
    private Vector3 followOffset;
    private Vector2 turn;

    private void Awake()
    {
        followOffset = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
    }

    private void Update()
    {
        HandleCameraMovement();
        HandleCameraRotation();
        HandleCameraZoom();
    }

    private void HandleCameraMovement()
    {
        var inputDir = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W)) inputDir.z = -1f;
        if (Input.GetKey(KeyCode.S)) inputDir.z = 1f;
        if (Input.GetKey(KeyCode.A)) inputDir.x = 1f;
        if (Input.GetKey(KeyCode.D)) inputDir.x = -1f;
        
        if (useEdgeScrolling) HandleEdgeScrolling(inputDir);
        if (useDragPanMovement) HandleDragPan(inputDir);
        
        MoveCamera(inputDir);
    }

    private void HandleEdgeScrolling(Vector3 inputDir)
    {
        var mousePos = Input.mousePosition;
        if (mousePos.x < edgeScrollSize) inputDir.x = -1f;
        if (mousePos.y < edgeScrollSize) inputDir.z = -1f;
        if (mousePos.x > Screen.width - edgeScrollSize) inputDir.x = 1f;
        if (mousePos.y > Screen.height - edgeScrollSize) inputDir.z = 1f;
        MoveCamera(inputDir);
    }

    private void HandleDragPan(Vector3 inputDir)
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragPanMoveActive = true;
            lastMousePosition = Input.mouseScrollDelta;
        }

        if (Input.GetMouseButtonUp(1))
        {
            dragPanMoveActive = false;
        }

        if (dragPanMoveActive)
        {
            var mouseMovementDelta = (Vector2) Input.mousePosition - lastMousePosition;
            inputDir.x = mouseMovementDelta.x * dragPanSpeed;
            inputDir.z = mouseMovementDelta.y * dragPanSpeed;
            lastMousePosition = Input.mousePosition;
        }

        MoveCamera(inputDir);

    }

    private void MoveCamera(Vector3 inputDir)
    {
        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    private void HandleCameraRotation()
    {
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");
        if (Input.GetMouseButton(1)) transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }

    private void HandleCameraZoom()
    {
        Vector3 zoomDir = followOffset.normalized;
        switch (Input.mouseScrollDelta.y)
        {
            case > 0:
                followOffset -= zoomDir;
                break;
            case < 0:
                followOffset += zoomDir;
                break;
        }

        if (followOffset.magnitude < followOffsetMin)
        {
            followOffset = zoomDir * followOffsetMin;
        }
        if (followOffset.magnitude > followOffsetMax)
        {
            followOffset = zoomDir * followOffsetMax;
        }
        
        var currentFollowOffset = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = Vector3.Lerp(currentFollowOffset, followOffset, Time.deltaTime * zoomSpeed);

    }
    
}

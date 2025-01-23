using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class CameraController : MonoBehaviour
{
    
    private const float SpeedH = 2.0f;
    private const float SpeedV = 2.0f;
    
    private Camera mainCamera;
    private Vector3 mainCameraStartPosition;
    private Quaternion cameraRotationPointStartRotation;
    private float yaw;
    private float pitch;
    
    private float elapsedTime;
    
    private Vector3 hitPoint;
    private Vector3 targetPoint;
    private bool moving;
    
    [Header("Rotation")]
    [SerializeField] private Transform cameraRotationPoint;
    [Header("Movement")]
    [SerializeField] private float desiredCameraMovementDuration = 1.5f;
    [SerializeField] private float distanceFromTargetPosition = 2f;
    [SerializeField] private bool checkForRadius = true;
    [SerializeField] private float cameraMovementStopRadius = 0.1f;
    [Header("Zoom")] 
    [SerializeField] private bool enableZoom = true;
    [SerializeField] private float cameraZoomSpeed = 1f;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        var rotationPointTransform = cameraRotationPoint.transform;
        var rotationPointAngles = rotationPointTransform.eulerAngles;
        yaw = rotationPointAngles.y;
        pitch = rotationPointAngles.x;
        
        var mainCameraTransform = mainCamera.transform;
        mainCameraTransform.LookAt(rotationPointTransform);
        mainCameraStartPosition = mainCameraTransform.position;
        cameraRotationPointStartRotation = cameraRotationPoint.rotation;
    }

    private void Update()
    {
        HandleCameraRotation();
        if (enableZoom) HandleCameraZoom(mainCamera.transform.position, hitPoint);
        HandleObjectClick();
        
        if (Input.GetKeyUp(KeyCode.R)) ResetCamera();
        if (moving) HandleMoveCamera(mainCamera.transform.position, targetPoint);
    }

    private void HandleCameraRotation()
    {
        if (!Input.GetMouseButton(1)) return;
        yaw += SpeedH * Input.GetAxis("Mouse X");
        pitch += SpeedV * Input.GetAxis("Mouse Y");
        cameraRotationPoint.transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }

    private void HandleCameraZoom(Vector3 currentPos, Vector3 targetPos)
    {
        //TODO: Fix Camera leaving vector
        switch (Input.GetAxis("Mouse ScrollWheel"))
        {
            case > 0f:
                //Zoom in
                var distanceIn = Vector3.Distance(targetPos, currentPos);
                if (distanceIn > distanceFromTargetPosition + cameraMovementStopRadius)
                {
                    var directionIn = Vector3.Normalize(targetPos - currentPos) * cameraZoomSpeed;
                    var translationIn = currentPos + directionIn;
                    mainCamera.transform.position = translationIn;
                }
                break;
            case < 0f:
                //Zoom out
                var directionOut = Vector3.Normalize(targetPos - currentPos) * cameraZoomSpeed;
                var translationOut = currentPos - directionOut;
                mainCamera.transform.position = translationOut;
                break;
        }
    }
    
    private void HandleObjectClick()
    {
        //TODO: Disable when over UI
        if (!Input.GetMouseButtonUp(0)) return;
        RaycastHit raycastHit;
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        
        if (!Physics.Raycast(ray, out raycastHit, 100.0f)) return;
        hitPoint = raycastHit.point;
        targetPoint = CalcCameraTargetPosition(hitPoint, mainCamera.transform.position);
        moving = true;
    }

    private void HandleMoveCamera(Vector3 startPosition, Vector3 endPosition)
    {
        elapsedTime += Time.deltaTime;
        var percentageComplete = elapsedTime / desiredCameraMovementDuration;
        mainCamera.transform.position = Vector3.Lerp(startPosition, endPosition, percentageComplete);
        if (CheckCameraAtDestination(startPosition, endPosition))
        {
            moving = false;
            elapsedTime = 0f;

            var cameraTransform = mainCamera.transform;
            var tempCameraPos = cameraTransform.position;
            cameraRotationPoint.transform.position = hitPoint;
            cameraTransform.position = tempCameraPos;
        }
    }

    private Vector3 CalcCameraTargetPosition(Vector3 endPos, Vector3 startPos)
    {
        var direction = Vector3.Normalize(endPos - startPos);
        var distance = Vector3.Distance(endPos, startPos) - distanceFromTargetPosition;
        return startPos + (distance * direction);
    }
    
    private void ResetCamera()
    {
        targetPoint = mainCameraStartPosition;
        cameraRotationPoint.rotation = cameraRotationPointStartRotation;
        hitPoint = new Vector3(0, 0, 0);
        cameraRotationPoint.position = new Vector3(0, 0, 0);
        moving = true;
    }

    private bool CheckCameraAtDestination(Vector3 currentPos, Vector3 targetPos)
    {
        return checkForRadius switch
        {
            true => Vector3.Distance(targetPos, currentPos) <= cameraMovementStopRadius,
            false => currentPos.Equals(targetPos)
        };
    }
    
}

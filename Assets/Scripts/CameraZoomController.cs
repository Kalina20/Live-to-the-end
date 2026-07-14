using UnityEngine;
using UnityEngine.InputSystem;

public class CameraZoomController : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private DrawnCardPresenter drawnCardPresenter;
    [SerializeField] private float touchZoomSpeed = 0.01f;
    [SerializeField] private float mouseZoomSpeed = 0.05f;
    [SerializeField] private float minFieldOfView = 35f;
    [SerializeField] private float maxFieldOfView = 75f;
    [SerializeField] private float boardPlaneHeight = 0f;
    [SerializeField] private Vector3 orbitTarget = new Vector3(6f, 0f, 9f);
    [SerializeField] private float mouseOrbitSpeed = 0.15f;
    [SerializeField] private float touchOrbitSpeed = 0.12f;
    [SerializeField] private float minPitch = 25f;
    [SerializeField] private float maxPitch = 75f;
    [SerializeField] private float minCameraHeight = 1f;
    [SerializeField] private float returnToStartSpeed = 5f;
    [SerializeField] private float returnThreshold = 0.1f;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool shouldReturnToStart;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = GetComponent<Camera>();
        }
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    private void Update()
    {
        if (drawnCardPresenter != null && drawnCardPresenter.IsPresented)
        {
            return;
        }
        HandleMouseZoom();
        HandleTouchZoom();
        HandleMouseOrbit();
        HandleTouchOrbit();
        HandleReturnToStart();
    }

    private void HandleMouseZoom()
    {
        if (Mouse.current == null)
        {
            return;
        }

        float scroll = Mouse.current.scroll.ReadValue().y;

        if (Mathf.Approximately(scroll, 0f))
        {
            return;
        }

        ApplyZoomAtScreenPoint(-scroll * mouseZoomSpeed, Mouse.current.position.ReadValue());
    }

    private void HandleMouseOrbit()
    {
        if (Mouse.current == null || !Mouse.current.rightButton.isPressed)
        {
            return;
        }

        Vector2 delta = Mouse.current.delta.ReadValue();
        ApplyOrbit(delta.x * mouseOrbitSpeed, -delta.y * mouseOrbitSpeed);
    }

    private void HandleTouchZoom()
    {
        if (Touchscreen.current == null)
        {
            return;
        }

        var touchZero = Touchscreen.current.touches[0];
        var touchOne = Touchscreen.current.touches[1];

        if (!touchZero.press.isPressed || !touchOne.press.isPressed)
        {
            return;
        }

        Vector2 touchZeroPosition = touchZero.position.ReadValue();
        Vector2 touchOnePosition = touchOne.position.ReadValue();

        Vector2 touchZeroDelta = touchZero.delta.ReadValue();
        Vector2 touchOneDelta = touchOne.delta.ReadValue();

        Vector2 touchZeroPreviousPosition = touchZeroPosition - touchZeroDelta;
        Vector2 touchOnePreviousPosition = touchOnePosition - touchOneDelta;

        float previousDistance = Vector2.Distance(touchZeroPreviousPosition, touchOnePreviousPosition);
        float currentDistance = Vector2.Distance(touchZeroPosition, touchOnePosition);

        float distanceDelta = currentDistance - previousDistance;

        Vector2 zoomCenter = (touchZeroPosition + touchOnePosition) * 0.5f;
        ApplyZoomAtScreenPoint(-distanceDelta * touchZoomSpeed, zoomCenter);
    }

    private void HandleTouchOrbit()
    {
        if (Touchscreen.current == null)
        {
            return;
        }

        var primaryTouch = Touchscreen.current.primaryTouch;

        if (!primaryTouch.press.isPressed)
        {
            return;
        }

        int pressedTouches = 0;

        foreach (var touch in Touchscreen.current.touches)
        {
            if (touch.press.isPressed)
            {
                pressedTouches++;
            }
        }

        if (pressedTouches != 1)
        {
            return;
        }

        Vector2 delta = primaryTouch.delta.ReadValue();
        ApplyOrbit(delta.x * touchOrbitSpeed, -delta.y * touchOrbitSpeed);
    }

    private void ApplyZoomAtScreenPoint(float fieldOfViewDelta, Vector2 screenPoint)
    {
        if (targetCamera == null)
        {
            return;
        }

        bool hasWorldPointBeforeZoom = TryGetWorldPointOnBoard(screenPoint, out Vector3 worldPointBeforeZoom);

        targetCamera.fieldOfView = Mathf.Clamp(
            targetCamera.fieldOfView + fieldOfViewDelta,
            minFieldOfView,
            maxFieldOfView
        );
        shouldReturnToStart = Mathf.Abs(targetCamera.fieldOfView - maxFieldOfView) <= returnThreshold;

        if (!hasWorldPointBeforeZoom || !TryGetWorldPointOnBoard(screenPoint, out Vector3 worldPointAfterZoom))
        {
            return;
        }

        transform.position += worldPointBeforeZoom - worldPointAfterZoom;
       
    }

    private void ApplyOrbit(float yawDelta, float pitchDelta)
    {
        Vector3 offset = transform.position - orbitTarget;
        float distance = offset.magnitude;

        if (distance <= 0.01f)
        {
            return;
        }

        float yaw = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg + yawDelta;
        float pitch = Mathf.Asin(Mathf.Clamp(offset.y / distance, -1f, 1f)) * Mathf.Rad2Deg;
        pitch = Mathf.Clamp(pitch + pitchDelta, minPitch, maxPitch);

        float yawRadians = yaw * Mathf.Deg2Rad;
        float pitchRadians = pitch * Mathf.Deg2Rad;

        Vector3 newOffset = new Vector3(
            Mathf.Sin(yawRadians) * Mathf.Cos(pitchRadians),
            Mathf.Sin(pitchRadians),
            Mathf.Cos(yawRadians) * Mathf.Cos(pitchRadians)
        ) * distance;

        Vector3 newPosition = orbitTarget + newOffset;
        newPosition.y = Mathf.Max(newPosition.y, boardPlaneHeight + minCameraHeight);

        transform.position = newPosition;
        transform.LookAt(orbitTarget);
        shouldReturnToStart = false;
    }

    private bool TryGetWorldPointOnBoard(Vector2 screenPoint, out Vector3 worldPoint)
    {
        Ray ray = targetCamera.ScreenPointToRay(screenPoint);
        Plane boardPlane = new Plane(Vector3.up, new Vector3(0f, boardPlaneHeight, 0f));

        if (boardPlane.Raycast(ray, out float enter))
        {
            worldPoint = ray.GetPoint(enter);
            return true;
        }

        worldPoint = Vector3.zero;
        return false;
    }

    private void HandleReturnToStart()
    {
        if (!shouldReturnToStart)
        {
            return;
        }

        transform.position = Vector3.Lerp(
            transform.position,
            startPosition,
            Time.deltaTime * returnToStartSpeed
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            startRotation,
            Time.deltaTime * returnToStartSpeed
        );

        float positionDistance = Vector3.Distance(transform.position, startPosition);
        float rotationDistance = Quaternion.Angle(transform.rotation, startRotation);

        if (positionDistance < 0.01f && rotationDistance < 0.1f)
        {
            transform.position = startPosition;
            transform.rotation = startRotation;
            shouldReturnToStart = false;
        }
    }
    public void ReturnToStart()
    {
        shouldReturnToStart = true;
    }
}

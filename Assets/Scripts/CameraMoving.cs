using UnityEngine;

public class CameraMoving : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float dragSpeed = 1f;
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 10f;
    [SerializeField] private float mapWidth = 20f;   // Ширина игрового поля
    [SerializeField] private float mapHeight = 12f;
    [SerializeField] private float zoomSmoothTime = 0.1f; // Время плавности для зума

    private Vector3 dragOrigin;
    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;
    private bool isDragging = false;

    // Новые поля для плавного зума
    private float targetZoom;
    private float zoomVelocity = 0f;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        targetPosition = transform.position;
        targetZoom = mainCamera.orthographicSize;
    }

    private void Update()
    {
        HandleDrag();
        SmoothMove();
        HandleZoomInput(); // Обрабатываем ввод для зума
        SmoothZoom();      // Плавно применяем зум
    }

    private void HandleZoomInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            // Сохраняем позицию мыши в мировых координатах
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            // Меняем целевой зум
            targetZoom -= scroll * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);

            // Рассчитываем смещение камеры для удержания позиции мыши
            // Используем текущий зум для расчета смещения
            float currentZoom = mainCamera.orthographicSize;
            float zoomDelta = targetZoom - currentZoom;

            if (zoomDelta != 0)
            {
                // Временное применение нового зума для расчета смещения
                mainCamera.orthographicSize = targetZoom;
                Vector3 newMouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mainCamera.orthographicSize = currentZoom; // Возвращаем обратно

                // Добавляем смещение к целевой позиции
                targetPosition += mouseWorldPos - newMouseWorldPos;
                LimitTargetPosition();
            }
        }
    }

    private void SmoothZoom()
    {
        if (Mathf.Abs(mainCamera.orthographicSize - targetZoom) > 0.01f)
        {
            mainCamera.orthographicSize = Mathf.SmoothDamp(
                mainCamera.orthographicSize,
                targetZoom,
                ref zoomVelocity,
                zoomSmoothTime
            );
        }
    }

    private void LimitTargetPosition()
    {
        // Получаем текущий зум камеры (еще может быть старым, но для ограничений используем целевой)
        float currentZoom = mainCamera.orthographicSize;
        float verticalSize = currentZoom;
        float horizontalSize = verticalSize * mainCamera.aspect;

        // Ограничиваем целевую позицию
        targetPosition.x = Mathf.Clamp(targetPosition.x, -mapWidth + horizontalSize, mapWidth - horizontalSize);
        targetPosition.y = Mathf.Clamp(targetPosition.y, -mapHeight + verticalSize, mapHeight - verticalSize);
    }

    private void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            dragOrigin = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            Vector3 currentMousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 difference = dragOrigin - currentMousePos;
            targetPosition += new Vector3(difference.x, difference.y, 0) * dragSpeed;
            dragOrigin = currentMousePos;
            LimitTargetPosition();
        }
    }

    private void SmoothMove()
    {
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }
}
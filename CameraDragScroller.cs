using UnityEngine;

public class TownCameraController : MonoBehaviour
{
    [Header("Drag Settings")]
    [SerializeField] private float dragSpeed = 1.0f;

    [Header("Map Settings (Pixels)")]
    [SerializeField] private Vector2 mapSize = new Vector2(4000f, 2500f);
    [SerializeField] private float pixelsPerUnit = 100f; // 유니티 스프라이트의 PPU 기본값

    private Camera mainCamera;
    private Vector3 dragOrigin;

    // 월드 단위로 변환된 맵 크기
    private float mapWidthInUnits;
    private float mapHeightInUnits;

    void Start()
    {
        mainCamera = Camera.main;

        // 픽셀 단위를 유니티 월드 단위(Unit)로 변환
        mapWidthInUnits = mapSize.x / pixelsPerUnit;
        mapHeightInUnits = mapSize.y / pixelsPerUnit;
    }

    void Update()
    {
        HandleMouseDrag();
    }

    private void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            return;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 currentMousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 difference = dragOrigin - currentMousePos;

            // 새로운 목표 위치 계산
            Vector3 targetPosition = transform.position + difference * dragSpeed;

            // 화면 모서리가 맵 밖으로 나가지 않도록 제한 적용
            transform.position = ClampCameraToMap(targetPosition);
        }
    }

    private Vector3 ClampCameraToMap(Vector3 targetPos)
    {
        // 카메라의 세로 절반 크기 (월드 단위)
        float camHeight = mainCamera.orthographicSize;
        // 카메라의 가로 절반 크기 (월드 단위, 화면 비율 반영)
        float camWidth = camHeight * mainCamera.aspect;

        // 화면 모서리가 맵 끝에 닿는 최소/최대 월드 좌표 계산
        float minX = -(mapWidthInUnits / 2f) + camWidth;
        float maxX = (mapWidthInUnits / 2f) - camWidth;
        float minY = -(mapHeightInUnits / 2f) + camHeight;
        float maxY = (mapHeightInUnits / 2f) - camHeight;

        // 만약 카메라 크기가 맵 크기보다 크다면 화면을 중앙에 고정
        if (minX > maxX) { minX = maxX = 0f; }
        if (minY > maxY) { minY = maxY = 0f; }

        // 계산된 범위 내로 카메라 좌표 가두기
        float clampedX = Mathf.Clamp(targetPos.x, minX, maxX);
        float clampedY = Mathf.Clamp(targetPos.y, minY, maxY);

        return new Vector3(clampedX, clampedY, targetPos.z);
    }
}

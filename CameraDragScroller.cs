using UnityEngine;
// 최신 입력 시스템을 사용하기 위해 패키지를 불러옵니다.
using UnityEngine.InputSystem; 

public class CameraDragScroller : MonoBehaviour
{
    [Header("마을배경 오브젝트를 넣어주세요")]
    public RectTransform townBackground; 

    [Header("드래그 속도 (UI 크기에 맞춰 0.5~1.5 추천)")]
    public float dragSpeed = 1f;

    private Vector3 dragOrigin;
    private Camera cam;
    private float minX, maxX, minY, maxY;
    private bool isDragging = false;

    void Start()
    {
        cam = Camera.main;

        if (townBackground != null)
        {
            CalculateUIBounds();
        }
        else
        {
            Debug.LogError("마을 배경(RectTransform)이 지정되지 않았습니다! 메인 카메라 인스펙터에서 넣어주세요.");
        }
    }

    void CalculateUIBounds()
    {
        Vector3[] corners = new Vector3[4];
        townBackground.GetWorldCorners(corners);

        float bgMinX = corners[0].x;
        float bgMaxX = corners[2].x;
        float bgMinY = corners[0].y;
        float bgMaxY = corners[2].y;

        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        minX = bgMinX + camWidth;
        maxX = bgMaxX - camWidth;
        minY = bgMinY + camHeight;
        maxY = bgMaxY - camHeight;

        if (minX > maxX) { float midX = (bgMinX + bgMaxX) / 2f; minX = maxX = midX; }
        if (minY > maxY) { float midY = (bgMinY + bgMaxY) / 2f; minY = maxY = midY; }
    }

    void Update()
    {
        // 최신 마우스/터치 입력 감지 방식
        Mouse currentMouse = Mouse.current;
        if (currentMouse == null) return;

        Vector2 mousePosition = currentMouse.position.ReadValue();

        // 1. 마우스 왼쪽 버튼을 처음 누른 순간
        if (currentMouse.leftButton.wasPressedThisFrame)
        {
            dragOrigin = cam.ScreenToWorldPoint(mousePosition);
            isDragging = true;
            return;
        }

        // 2. 마우스 왼쪽 버튼을 뗀 순간 드래그 종료
        if (currentMouse.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }

        // 3. 드래그 중일 때 카메라 이동 계산
        if (isDragging && currentMouse.leftButton.isPressed)
        {
            Vector3 currentPos = cam.ScreenToWorldPoint(mousePosition);
            Vector3 difference = dragOrigin - currentPos;
            
            difference.z = 0; 

            Vector3 targetPos = transform.position + difference * dragSpeed;

            // 마을 영역 밖을 절대 못 나가게 차단
            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
            targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

            transform.position = targetPos;
        }
    }
}

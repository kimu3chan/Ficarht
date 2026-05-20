using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class CardObject : MonoBehaviour
{
    [Header("카드 데이터")]
    public CardData data;

    [Header("프리팹 연결")]
    public SpriteRenderer cardFrontRenderer;
    public GameObject     cardBackObject;

    [Header("호버 설정")]
    public float hoverHeight  = 0.1f;
    public float moveSpeed    = 15f;

    private Camera   cam;
    private Rigidbody rb;

    private bool isDragging;
    private bool isHovered;

    private Vector3    worldOffset;
    private float      dragDepth;
    private Vector3    originPosition;
    private Quaternion originRotation;
    private Vector3    targetPosition;

    private CardSlot hoveredSlot;

    private static CardObject currentHoveredCard;

    private void Awake()
    {
        cam = Camera.main;
        rb  = GetComponent<Rigidbody>();
        rb.useGravity    = false;
        rb.isKinematic   = true;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // 부드러운 이동
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);

        HandleHover();

        if (!isDragging) return;
        DragUpdate();
        if (Mouse.current.leftButton.wasReleasedThisFrame) StopDrag();
    }

    // -------------------------------------------------------
    //  초기화
    // -------------------------------------------------------
    public void Setup(CardData newData)
    {
        data = newData;
        if (cardFrontRenderer != null)
            cardFrontRenderer.color = GetColorByType(data.cardType);
        ShowFront();
    }

    /// <summary>
    /// HandLayoutManager 정렬 후 호출. 현재 위치를 기준점으로 저장.
    /// </summary>
    public void InitPosition()
    {
        originPosition = transform.position;
        targetPosition = transform.position;
    }

    /// <summary>
    /// 렌더러 표시/숨김 (SetActive 대신 사용)
    /// </summary>
    public void SetVisible(bool visible)
    {
        foreach (var rend in GetComponentsInChildren<Renderer>())
            rend.enabled = visible;
    }

    // -------------------------------------------------------
    //  앞면 / 뒷면
    // -------------------------------------------------------
    public void ShowFront()
    {
        if (cardFrontRenderer != null) cardFrontRenderer.gameObject.SetActive(true);
        if (cardBackObject    != null) cardBackObject.SetActive(false);
    }

    public void ShowBack()
    {
        if (cardFrontRenderer != null) cardFrontRenderer.gameObject.SetActive(false);
        if (cardBackObject    != null) cardBackObject.SetActive(true);
    }

    // -------------------------------------------------------
    //  호버
    // -------------------------------------------------------
    private void HandleHover()
    {
        if (isDragging) return;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            CardObject card = hit.collider.GetComponent<CardObject>();

            if (card == this)
            {
                if (!isHovered) HoverEnter();
                if (Mouse.current.leftButton.wasPressedThisFrame) StartDrag();
            }
            else
            {
                if (isHovered) HoverExit();
            }
        }
        else
        {
            if (isHovered) HoverExit();
        }
    }

    private void HoverEnter()
    {
        if (currentHoveredCard != null && currentHoveredCard != this)
            currentHoveredCard.HoverExit();

        currentHoveredCard = this;
        isHovered          = true;

        targetPosition = new Vector3(originPosition.x, originPosition.y + hoverHeight, originPosition.z);
    }

    private void HoverExit()
    {
        if (isDragging) return;
        isHovered = false;
        targetPosition = originPosition;

        if (currentHoveredCard == this) currentHoveredCard = null;
    }

    // -------------------------------------------------------
    //  드래그
    // -------------------------------------------------------
    public void StartDrag()
    {
        if (CardSystemManager.Instance == null || !CardSystemManager.Instance.IsTurnActive) return;

        isDragging     = true;
        isHovered      = false;
        originRotation = transform.rotation;
        dragDepth      = cam.WorldToScreenPoint(originPosition).z;
        worldOffset    = originPosition - ScreenToWorld(Mouse.current.position.ReadValue());
    }

    private void DragUpdate()
    {
        Vector3 target = ScreenToWorld(Mouse.current.position.ReadValue()) + worldOffset;
        target.y       = originPosition.y + hoverHeight;
        targetPosition = target;
    }

    private void StopDrag()
    {
        isDragging = false;

        if (hoveredSlot != null)
        {
            bool placed = hoveredSlot.TryPlaceCard(this);
            if (!placed) ReturnToOrigin();
        }
        else
        {
            ReturnToOrigin();
        }
    }

    private void ReturnToOrigin()
    {
        targetPosition = originPosition;
        transform.rotation = originRotation;
    }

    // -------------------------------------------------------
    //  슬롯 감지
    // -------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        CardSlot slot = other.GetComponent<CardSlot>();
        if (slot != null) hoveredSlot = slot;
    }

    private void OnTriggerExit(Collider other)
    {
        CardSlot slot = other.GetComponent<CardSlot>();
        if (slot != null && hoveredSlot == slot) hoveredSlot = null;
    }

    // -------------------------------------------------------
    //  유틸
    // -------------------------------------------------------
    private Vector3 ScreenToWorld(Vector2 screenPos)
    {
        Vector3 pos = screenPos;
        pos.z = dragDepth;
        return cam.ScreenToWorldPoint(pos);
    }

    private Color GetColorByType(CardType type)
    {
        switch (type)
        {
            case CardType.Character: return Color.cyan;
            case CardType.Buff:      return Color.green;
            case CardType.Skill:     return Color.magenta;
            case CardType.Trap:      return Color.red;
            default:                 return Color.white;
        }
    }
}
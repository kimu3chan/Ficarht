using UnityEngine;

/// <summary>
/// 월드 공간의 3D 카드 슬롯.
/// - IDropHandler(UI) 대신 Physics Trigger 방식 사용
/// - 슬롯 GameObject에 Collider(isTrigger = true) 필수
/// - TryPlaceCard()는 CardObject.OnMouseUp에서 호출됨
/// </summary>
[RequireComponent(typeof(Collider))]
public class CardSlot : MonoBehaviour
{
    [Header("슬롯 설정")]
    public CardType allowedType;
    public CardObject currentCard;

    [Header("배치 위치 (비워두면 슬롯 자체 위치 사용)")]
    public Transform snapPoint; // 카드가 놓일 정확한 위치/회전 지정용 (선택)

    private void Awake()
    {
        // 슬롯 Collider는 반드시 Trigger여야 함
        Collider col = GetComponent<Collider>();
        if (!col.isTrigger)
        {
            col.isTrigger = true;
            Debug.LogWarning($"[CardSlot] {gameObject.name}의 Collider를 isTrigger=true로 자동 설정했습니다.");
        }
    }

    /// <summary>
    /// CardObject.OnMouseUp()에서 호출.
    /// 배치 성공 여부를 반환.
    /// </summary>
    public bool TryPlaceCard(CardObject card)
    {
        // 1. 타입 불일치
        if (card.data.cardType != allowedType)
        {
            Debug.LogWarning($"[CardSlot] 타입 불일치: 슬롯={allowedType}, 카드={card.data.cardType}");
            return false;
        }

        // 2. 슬롯이 이미 차 있음
        if (currentCard != null)
        {
            Debug.LogWarning($"[CardSlot] {gameObject.name} 슬롯이 이미 사용 중입니다.");
            return false;
        }

        // --- 배치 성공 ---
        currentCard = card;

        // 슬롯의 자식으로 등록하고 스냅 위치/회전에 맞춤
        Transform target = snapPoint != null ? snapPoint : transform;
        card.transform.SetParent(target);
        card.transform.localPosition = Vector3.zero;
        card.transform.localRotation = Quaternion.identity;

        // 매니저에 핸드에서 제거 요청
        if (CardSystemManager.Instance != null)
            CardSystemManager.Instance.OnCardPlaced(card);

        Debug.Log($"[CardSlot] {allowedType} 슬롯에 '{card.data.cardName}' 배치 성공!");
        return true;
    }

    /// <summary>
    /// 슬롯을 초기화하고 카드를 파괴.
    /// </summary>
    public void ClearSlot()
    {
        if (currentCard != null)
        {
            Destroy(currentCard.gameObject);
            currentCard = null;
        }
    }

    /// <summary>
    /// 슬롯에서 카드를 제거하되 파괴하지 않음 (다시 핸드로 돌릴 때).
    /// </summary>
    public CardObject PopCard()
    {
        CardObject card = currentCard;
        currentCard = null;
        if (card != null) card.transform.SetParent(null);
        return card;
    }
}

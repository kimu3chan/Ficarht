using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;

public class CardSystemManager : MonoBehaviour
{
    public static CardSystemManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    [Header("--- 덱 ---")]
    public List<CardData> characterDeck = new List<CardData>();
    public List<CardData> buffDeck      = new List<CardData>();
    public List<CardData> skillDeck     = new List<CardData>();

    [Header("--- 카드 프리팹 ---")]
    public GameObject cardPrefab;

    [Header("--- 드로우 포지션 (월드) ---")]
    public List<Transform> characterDrawPositions = new List<Transform>();
    public List<Transform> buffDrawPositions      = new List<Transform>();
    public List<Transform> skillDrawPositions     = new List<Transform>();

    [Header("--- 슬롯 ---")]
    public List<CardSlot> characterSlots = new List<CardSlot>();
    public List<CardSlot> buffSlots      = new List<CardSlot>();
    public List<CardSlot> skillSlots     = new List<CardSlot>();

    [Header("--- UI ---")]
    public TextMeshProUGUI timerText;

    [Header("--- 타이머 ---")]
    public float turnDuration = 60f;

    public bool IsTurnActive => isTurnActive;
    public List<CardObject> playerHand = new List<CardObject>();

    private bool         isTurnActive = false;
    private float        turnTimer    = 0f;
    private RuntimeStats myStats      = null;

    private void Start() => SetTimerText("3D 박스를 클릭하세요!");

    private void Update()
    {
        HandleCardBoxClick();
        HandleTimer();
    }

    private void HandleCardBoxClick()
    {
        if (isTurnActive) return;
        if (Pointer.current == null || !Pointer.current.press.wasPressedThisFrame) return;

        Ray ray = Camera.main.ScreenPointToRay(Pointer.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
            if (hit.collider.CompareTag("CardBox") || hit.collider.name == "CardBox")
                StartGame();
    }

    public void StartGame()
    {
        Debug.Log("[CardSystemManager] 게임 시작!");
        ClearAll();

        DrawCards(characterDeck, 1, characterDrawPositions);
        DrawCards(buffDeck,      3, buffDrawPositions);
        DrawCards(skillDeck,     2, skillDrawPositions);

        // 부채꼴 정렬 (정렬 완료 후 카드 표시됨)
        HandLayoutManager.Instance?.ArrangeHand(playerHand);

        CardRevealSystem.Instance?.HideOpponentCards();

        turnTimer    = turnDuration;
        isTurnActive = true;
    }

    private void DrawCards(List<CardData> deck, int count, List<Transform> positions)
    {
        if (deck == null || deck.Count == 0 || cardPrefab == null) return;

        for (int i = 0; i < count; i++)
        {
            if (positions == null || positions.Count == 0) break;

            Transform spawnTf = positions[Mathf.Min(i, positions.Count - 1)];
            CardData  data    = deck[Random.Range(0, deck.Count)];

            GameObject go   = Instantiate(cardPrefab, spawnTf.position, spawnTf.rotation);
            CardObject card = go.GetComponent<CardObject>();

            if (card != null)
            {
                card.SetVisible(false); // 정렬 전까지 숨기기
                card.Setup(data);
                playerHand.Add(card);
                Debug.Log($"[CardSystemManager] '{data.cardName}' 드로우");
            }
        }
    }

    public void OnCardPlaced(CardObject card)
    {
        playerHand.Remove(card);
        HandLayoutManager.Instance?.ReArrange(playerHand);
        Debug.Log($"[CardSystemManager] '{card.data.cardName}' 배치됨. 남은 핸드: {playerHand.Count}장");
    }

    private void HandleTimer()
    {
        if (!isTurnActive) return;
        turnTimer -= Time.deltaTime;
        SetTimerText($"남은 시간: {Mathf.CeilToInt(turnTimer)}초");
        if (turnTimer <= 0f) OnTimerEnd();
    }

    private void OnTimerEnd()
    {
        isTurnActive = false;
        SetTimerText("시간 종료!");

        myStats = GetCharacterStats();
        if (myStats != null)
        {
            BuffApplier.ApplyAll(buffSlots, myStats);
            Debug.Log($"[CardSystemManager] 버프 적용 후 스탯: {myStats}");
        }

        SkillRegistry.Instance?.RegisterFromSlots(skillSlots);
        CardRevealSystem.Instance?.RevealAllCards();
        FinalizeCards();

        // TODO: PlayerController.Instance.Initialize(myStats, SkillRegistry.Instance.GetSkills());
    }

    private RuntimeStats GetCharacterStats()
    {
        foreach (var slot in characterSlots)
        {
            if (slot?.currentCard == null) continue;
            if (slot.currentCard.data.cardType != CardType.Character) continue;
            if (slot.currentCard.data.characterStats == null)
            {
                Debug.LogError($"[CardSystemManager] '{slot.currentCard.data.cardName}'에 CharacterStats가 없습니다!");
                return null;
            }
            return slot.currentCard.data.characterStats.CreateRuntime();
        }
        Debug.LogWarning("[CardSystemManager] 배치된 캐릭터 카드가 없습니다.");
        return null;
    }

    private void FinalizeCards()
    {
        List<CardSlot> allSlots = new List<CardSlot>();
        allSlots.AddRange(characterSlots);
        allSlots.AddRange(buffSlots);
        allSlots.AddRange(skillSlots);

        foreach (var slot in allSlots)
        {
            if (slot?.currentCard == null) continue;
            Collider col = slot.currentCard.GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }

        foreach (var card in playerHand)
        {
            if (card == null) continue;
            Collider col = card.GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }
    }

    public void ClearAll()
    {
        foreach (var card in playerHand) if (card != null) Destroy(card.gameObject);
        playerHand.Clear();

        List<CardSlot> allSlots = new List<CardSlot>();
        allSlots.AddRange(characterSlots);
        allSlots.AddRange(buffSlots);
        allSlots.AddRange(skillSlots);
        foreach (var slot in allSlots) slot?.ClearSlot();

        SkillRegistry.Instance?.Clear();
        myStats = null;
    }

    public RuntimeStats GetFinalStats() => myStats;

    private void SetTimerText(string msg) { if (timerText != null) timerText.text = msg; }
}
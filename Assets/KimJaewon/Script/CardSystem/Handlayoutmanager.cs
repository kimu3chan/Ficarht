using UnityEngine;
using System.Collections.Generic;

public class HandLayoutManager : MonoBehaviour
{
    public static HandLayoutManager Instance { get; private set; }

    [Header("부채꼴 중심 위치")]
    public Vector3 centerPosition = new Vector3(0.1f, 1.3f, -1.5f);

    [Header("부채꼴 설정")]
    public float cardSpacing = 0.5f;
    public float fanAngle    = 30f;

    [Range(0f, 1f)]
    public float overlapAmount = 0.3f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void ArrangeHand(List<CardObject> hand)
    {
        if (hand == null || hand.Count == 0) return;

        int count = hand.Count;

        if (count == 1)
        {
            hand[0].transform.position = centerPosition;
            hand[0].InitPosition();
            hand[0].SetVisible(true);
            return;
        }

        float spacing    = cardSpacing * (1f - overlapAmount);
        float totalWidth = spacing * (count - 1);
        float startX     = centerPosition.x - totalWidth / 2f;
        float angleStep  = fanAngle / (count - 1);
        float startAngle = -fanAngle / 2f;

        for (int i = 0; i < count; i++)
        {
            if (hand[i] == null) continue;

            float xPos          = startX + spacing * i;
            float normalizedPos = (float)i / (count - 1);
            float zOffset       = Mathf.Sin(normalizedPos * Mathf.PI) * 0.05f;

            hand[i].transform.position = new Vector3(xPos, centerPosition.y, centerPosition.z + zOffset);

            float     zAngle      = startAngle + angleStep * i;
            Quaternion baseRotation = hand[i].transform.rotation;
            hand[i].transform.rotation = baseRotation * Quaternion.Euler(0f, 0f, -zAngle);
        }

        // 정렬 완료 후 위치 초기화 및 표시
        foreach (var card in hand)
        {
            if (card == null) continue;
            card.InitPosition();
            card.SetVisible(true);
        }

        Debug.Log($"[HandLayoutManager] 카드 {count}장 정렬 완료.");
    }

    public void ReArrange(List<CardObject> hand) => ArrangeHand(hand);
}
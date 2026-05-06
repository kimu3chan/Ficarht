using UnityEngine;
using DG.Tweening;

public class Exit_Animation : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private Vector3 targetScale = Vector3.one;
    public float timeX = 0.5f;
    public float timeZ = 0.5f;
    public void OnEnable()
    {
        transform.localScale = Vector3.one * 0.001f;
        PlayEnterAnimation();
    }

    public void PlayEnterAnimation()
    {
        transform.DOKill();

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScaleZ(targetScale.z, timeZ).SetEase(Ease.InCubic))
            .Append(transform.DOScaleX(targetScale.x, timeX).SetEase(Ease.OutCubic));
    }

    public void CloseExitAnimation()
    {
        transform.DOKill();
        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOScaleX(0.001f, timeX).SetEase(Ease.InCubic))
            .Append(transform.DOScaleZ(0.001f, timeZ).SetEase(Ease.OutCubic));

        seq.OnComplete(() => gameObject.SetActive(false));
    }
    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
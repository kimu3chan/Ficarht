using DG.Tweening;
using UnityEngine;

public class Light_Animation : MonoBehaviour
{
    private Light _light;

    public float lightTime = 0.7f;
    public float intensity = 45.2f;

    private void Awake()
    {
        _light = GetComponent<Light>();
    }

    public void Light_On()
    {
        DOTween.Kill(_light);
        DOTween.To(() => 0f,  x => _light.intensity = x, intensity, lightTime);
    }

    public void Light_Off()
    {
        DOTween.Kill(_light);
        DOTween.To(() => _light.intensity,  x => _light.intensity = x, 0, lightTime);
    }
}

using System;
using UnityEngine;

public class Trail : MonoBehaviour
{
    TrailRenderer trailRenderer;

    void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }
    private void OnEnable()
    {
        trailRenderer.enabled = false;
        
        trailRenderer.enabled = true;
    }
}

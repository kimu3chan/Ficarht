using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class SnapToGridEditor : MonoBehaviour
{
    void Update()
    {
        if (!Application.isPlaying)
        {
            Vector3 pos = transform.position;

            Vector3 snapped = new Vector3(
                Mathf.Round(pos.x),
                Mathf.Round(pos.y),
                Mathf.Round(pos.z)
            );

            if (pos != snapped)
            {
                transform.position = snapped;
            }
        }
    }
}
using DG.Tweening;
using UnityEngine;

public class Camera_Animation : MonoBehaviour
{
    public void GameStartCamearaAnimation()
    {
        Transform cameraPosition = this.gameObject.GetComponent<Transform>();

        cameraPosition.DOKill();
        
    }
}

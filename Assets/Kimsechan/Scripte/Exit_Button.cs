using UnityEngine;

public class Exit_Button : MonoBehaviour
{
    public GameObject exit;

    public void OnExit()
    {
        exit.SetActive(false);
        exit.SetActive(true);
    }
}

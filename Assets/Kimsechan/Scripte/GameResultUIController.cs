using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(UIDocument))]
public class GameResultUIController : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // 싱글턴 (전투 씬 내에서 유일)
    // ─────────────────────────────────────────────
    public static GameResultUIController Instance { get; private set; }

    [Header("씬 이동")]
    public string mainMenuSceneName = "CardMap_MainDesplay";

    [Header("결과 UXML (Inspector에서 연결)")]
    public VisualTreeAsset winUxml;   // Clear.uxml
    public VisualTreeAsset loseUxml;  // Defeat.uxml

    private UIDocument _doc;
    private VisualElement _quitConfirmOverlay;

    // ─────────────────────────────────────────────
    // 초기화
    // ─────────────────────────────────────────────
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _doc = GetComponent<UIDocument>();

        // 게임 시작 시 결과 UI 숨김
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // ─────────────────────────────────────────────
    // 외부 호출 (PlayerNetwork.TargetShowResult에서 호출)
    // ─────────────────────────────────────────────
    public void ShowResult(bool isWinner)
    {
        // 승리/패배에 맞는 UXML 적용
        _doc.visualTreeAsset = isWinner ? winUxml : loseUxml;

        gameObject.SetActive(true);

        // UXML 전환 후 버튼 다시 연결
        WireButtons();
    }

    // ─────────────────────────────────────────────
    // 버튼 연결
    // ─────────────────────────────────────────────
    private void WireButtons()
    {
        VisualElement root = _doc.rootVisualElement;
        if (root == null) return;

        _quitConfirmOverlay = root.Q<VisualElement>("quit-confirm-overlay");

        Button mainMenuButton       = root.Q<Button>("main-menu-button");
        Button quitButton           = root.Q<Button>("quit-button");
        Button quitConfirmYesButton = root.Q<Button>("quit-confirm-yes-button");
        Button quitConfirmNoButton  = root.Q<Button>("quit-confirm-no-button");

        if (mainMenuButton       != null) mainMenuButton.clicked       += GoToMainMenu;
        if (quitButton           != null) quitButton.clicked           += ShowQuitConfirm;
        if (quitConfirmYesButton != null) quitConfirmYesButton.clicked += QuitGame;
        if (quitConfirmNoButton  != null) quitConfirmNoButton.clicked  += HideQuitConfirm;

        HideQuitConfirm();
    }

    // ─────────────────────────────────────────────
    // 버튼 핸들러
    // ─────────────────────────────────────────────
    private void GoToMainMenu()
    {
        StopNetworkIfNeeded();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void ShowQuitConfirm()
    {
        if (_quitConfirmOverlay != null)
            _quitConfirmOverlay.style.display = DisplayStyle.Flex;
    }

    private void HideQuitConfirm()
    {
        if (_quitConfirmOverlay != null)
            _quitConfirmOverlay.style.display = DisplayStyle.None;
    }

    private void QuitGame()
    {
        StopNetworkIfNeeded();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void StopNetworkIfNeeded()
    {
        if (NetworkManager.singleton == null) return;

        if (NetworkServer.active && NetworkClient.isConnected)
            NetworkManager.singleton.StopHost();
        else if (NetworkClient.isConnected)
            NetworkManager.singleton.StopClient();
        else if (NetworkServer.active)
            NetworkManager.singleton.StopServer();
    }
}

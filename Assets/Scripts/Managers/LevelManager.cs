using Cinemachine;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    public static LevelManager instance;

    [SerializeField] private CanvasGroup gameOverCanvasGroup;
    [SerializeField] private Vector2 spawnPosition;
    [SerializeField] private GameObject player;
    [SerializeField] private string nextLevelName;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private bool m_isShowingYouDiedScreen;

    private void Awake() {
        if (FindObjectOfType<LevelManager>() != null &&
            FindObjectOfType<LevelManager>().gameObject != gameObject) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        m_isShowingYouDiedScreen = false;
        gameOverCanvasGroup.alpha = 0;
        if (PlayerManager.instance == null) {
            Instantiate(player);
        }
    }

    private void Start() {
        GameManager.s_instance.changeGameSate(GameState.Playing);
        PlayerManager.instance.transform.position = spawnPosition;
        PlayerManager.instance.changePlayerSate(PlayerState.Idle);
        PlayerController.instance.enabled = true;
        virtualCamera.Follow = PlayerManager.instance.transform;
    }

    private void Update() {
        showingGameOverScreen();
    }

    public void showGameOverScreen() {
        m_isShowingYouDiedScreen = true;
    }

    public void restartLevel() {
        GameManager.s_instance.changeGameSate(GameState.RestartLevel);
    }

    public void returnToMenu() {
        GameManager.s_instance.changeGameSate(GameState.LoadMainMenu);
    }

    public void endLevel() {
        GameManager.s_instance.setNewLevelName(nextLevelName);
        GameManager.s_instance.changeGameSate(GameState.LoadLevel);
    }

    private void showingGameOverScreen() {
        if (!m_isShowingYouDiedScreen) {
            return;
        }
        gameOverCanvasGroup.alpha += Time.deltaTime;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [SerializeField] private CanvasGroup youDiedCanvasGroup;
    [SerializeField] private CanvasGroup youWinCanvasGroup;
    [SerializeField] private float restartLevelTimeInSeconds = 2f;

    private bool isShowingYouDiedScreen;
    private bool isShowingYouWinScreen;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        isShowingYouDiedScreen = false;
        youDiedCanvasGroup.alpha = 0;
        youWinCanvasGroup.alpha = 0;
    }

    private void Update() {
        showingYouDiedScreen();
        showingYouWinSceen();
    }

    private void showingYouWinSceen() {
        if (!isShowingYouWinScreen) {
            return;
        }

        youWinCanvasGroup.alpha += Time.deltaTime;
    }

    private void showingYouDiedScreen() {
        if (!isShowingYouDiedScreen) {
            return;
        }

        youDiedCanvasGroup.alpha += Time.deltaTime;
    }

    public void showYouWinScreen() {
        isShowingYouWinScreen = true;
        StartCoroutine(RestartLevel());
    }

    public void ShowYouDiedScreen() {
        isShowingYouDiedScreen = true;
        StartCoroutine(RestartLevel());
    }

    private IEnumerator RestartLevel() {

        yield return new WaitForSeconds(restartLevelTimeInSeconds);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

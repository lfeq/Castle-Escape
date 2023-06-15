using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [SerializeField] private CanvasGroup gameOverCanvasGroup;
    [SerializeField] private Vector2 spawnPosition;

    private bool m_isShowingYouDiedScreen;
    private bool m_isShowingYouWinScreen;

    void Awake()
    {
        if(FindObjectOfType<LevelManager>() != null && 
           FindObjectOfType<LevelManager>().gameObject != gameObject) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        m_isShowingYouDiedScreen = false;
        gameOverCanvasGroup.alpha = 0;
    }

    private void Start() {
        GameManager.s_instance.changeGameSate(GameState.Playing);
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

    private void showingGameOverScreen() {
        if (!m_isShowingYouDiedScreen) {
            return;
        }
        gameOverCanvasGroup.alpha += Time.deltaTime;
    }
}

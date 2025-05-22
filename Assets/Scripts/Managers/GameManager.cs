using UnityEngine;
using UnityEngine.SceneManagement;
using System; // Required for System.Enum

/// <summary>
/// Manages the overall game state, scene transitions, and persists across scenes.
/// Implements a singleton pattern to ensure only one instance exists.
/// </summary>
public class GameManager : MonoBehaviour {

    /// <summary>
    /// Static singleton instance of the GameManager.
    /// Allows global access to GameManager functionalities.
    /// </summary>
    public static GameManager s_instance;

    private GameState m_gameState; // Current state of the game.
    private string m_newLevel; // Name of the level to be loaded.

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Implements the singleton pattern and initializes the GameManager.
    /// </summary>
    private void Awake() {
        // Singleton pattern implementation:
        // If an instance already exists and it's not this one, destroy this new one.
        if (s_instance != null && s_instance != this) {
            Destroy(gameObject);
            return;
        }
        
        // If no instance exists, this becomes the instance.
        s_instance = this;
        
        // Persist this GameObject across scene loads.
        // This is crucial for a GameManager that needs to manage state across the entire game.
        DontDestroyOnLoad(gameObject);
        
        m_gameState = GameState.None; // Initialize with a default state.
    }

    /// <summary>
    /// Changes the current game state and triggers actions associated with the new state.
    /// </summary>
    /// <param name="t_newState">The new game state to transition to.</param>
    public void changeGameSate(GameState t_newState) {
        // Avoid redundant state changes or re-triggering state actions.
        if (m_gameState == t_newState) {
            return;
        }

        m_gameState = t_newState;
        Debug.Log($"GameManager: State changed to {m_gameState}"); // Log state changes for debugging

        switch (m_gameState) {
            case GameState.None:
                // Initial or idle state, no specific action.
                break;
            case GameState.LoadMainMenu:
                // Triggers loading of the main menu scene.
                loadMenu();
                break;
            case GameState.MainMenu:
                // State when the game is in the main menu.
                // Typically, UI scripts would handle interactions in this state.
                break;
            case GameState.LoadLevel:
                // Triggers loading of a specific game level, requires m_newLevel to be set.
                loadLevel();
                break;
            case GameState.Playing:
                // State when the game is actively being played.
                // Game logic (player input, enemy AI, etc.) is active.
                Time.timeScale = 1f; // Ensure game is running
                break;
            case GameState.RestartLevel:
                // Triggers a reload of the currently active scene.
                restartLevel();
                break;
            case GameState.GameOver:
                // State when the player has lost the game.
                // Typically, a game over screen is shown.
                Time.timeScale = 0f; // Pause game activity
                break;
            case GameState.Credits:
                // State for showing game credits.
                // Might involve loading a specific credits scene or UI panel.
                break;
            case GameState.QuitGame:
                // Triggers the application to close.
                quitGame();
                break;
            default:
                // Handles any undefined or unexpected game states.
                throw new UnityException($"GameManager: Invalid Game State encountered: {m_gameState}");
        }
    }

    /// <summary>
    /// Allows changing the game state via a string name, typically for editor tools or debugging.
    /// This method is more robust against typos by using Enum.TryParse.
    /// </summary>
    /// <param name="t_newState">The string representation of the GameState to transition to.</param>
    public void changeGameStateInEditor(string t_newState) {
        GameState parsedState;
        if (Enum.TryParse(t_newState, out parsedState)) {
            changeGameSate(parsedState);
        } else {
            Debug.LogWarning($"GameManager: Attempted to change to an invalid game state from editor: {t_newState}");
        }
    }

    /// <summary>
    /// Gets the current game state.
    /// </summary>
    /// <returns>The current <see cref="GameState"/>.</returns>
    public GameState getGameState() {
        return m_gameState;
    }

    /// <summary>
    /// Sets the name of the level to be loaded when transitioning to the <see cref="GameState.LoadLevel"/> state.
    /// </summary>
    /// <param name="t_newLevel">The string name of the scene to load.</param>
    public void setNewLevelName(string t_newLevel) {
        // It's crucial that this is set correctly before changing state to LoadLevel.
        m_newLevel = t_newLevel;
    }

    /// <summary>
    /// Loads the main menu scene.
    /// </summary>
    /// <remarks>
    /// Consider using SceneManager.LoadSceneAsync for smoother transitions, especially if the menu scene is complex.
    /// Error handling for invalid scene names (e.g., "MainMenu" not in build settings) could be added.
    /// </remarks>
    public void loadMenu() {
        // Potential Improvement: Implement asynchronous loading with a loading screen.
        // e.g., StartCoroutine(LoadSceneAsyncWrapper("MainMenu"));
        // Potential Improvement: Add try-catch for scene loading errors.
        SceneManager.LoadScene("MainMenu");
        changeGameSate(GameState.MainMenu); // Transition to MainMenu state after loading
    }

    /// <summary>
    /// Restarts the currently active level by reloading it.
    /// </summary>
    /// <remarks>
    /// Consider using SceneManager.LoadSceneAsync for smoother transitions.
    /// </remarks>
    private void restartLevel() {
        // Potential Improvement: Implement asynchronous loading.
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
        // Typically, after reloading, the game state might need to be explicitly set to Playing
        // or an initialization state by a script in the reloaded scene.
        // For now, we assume the reloaded scene handles its own state or PlayerManager/LevelManager will set it.
    }

    /// <summary>
    /// Loads the level specified by the <see cref="m_newLevel"/> field.
    /// </summary>
    /// <remarks>
    /// This method relies on <see cref="m_newLevel"/> being set prior to calling it (usually via <see cref="setNewLevelName"/>).
    /// Consider using SceneManager.LoadSceneAsync for smoother transitions, especially for large levels.
    /// Error handling for invalid scene names stored in m_newLevel could be added.
    /// </remarks>
    private void loadLevel() {
        if (string.IsNullOrEmpty(m_newLevel)) {
            Debug.LogError("GameManager: Attempted to load a level, but m_newLevel is not set. Aborting load.");
            // Optionally, transition to a safe state like MainMenu or show an error.
            // changeGameSate(GameState.LoadMainMenu); 
            return;
        }
        // Potential Improvement: Implement asynchronous loading with a loading screen.
        // e.g., StartCoroutine(LoadSceneAsyncWrapper(m_newLevel));
        // Potential Improvement: Add try-catch for scene loading errors.
        SceneManager.LoadScene(m_newLevel);
        changeGameSate(GameState.Playing); // Transition to Playing state after loading
    }

    /// <summary>
    /// Quits the application.
    /// Note: This works in standalone builds but has no effect in the Unity Editor.
    /// </summary>
    private void quitGame() {
        Debug.Log("GameManager: Quitting application...");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop playing in the editor
        #else
        Application.Quit(); // Quit in standalone builds
        #endif
    }
}

/// <summary>
/// Defines the possible states of the game.
/// </summary>
public enum GameState {
    /// <summary>Initial state or when no specific game phase is active.</summary>
    None,
    /// <summary>State indicating the main menu scene is being loaded.</summary>
    LoadMainMenu,
    /// <summary>State when the game is in the main menu interface.</summary>
    MainMenu,
    /// <summary>State indicating a game level is being loaded. Requires <see cref="GameManager.m_newLevel"/> to be set.</summary>
    LoadLevel,
    /// <summary>State when the game is actively being played.</summary>
    Playing,
    /// <summary>State indicating the current level should be restarted.</summary>
    RestartLevel,
    /// <summary>State when the player has lost the game.</summary>
    GameOver,
    /// <summary>State for displaying game credits.</summary>
    Credits,
    /// <summary>State indicating the game should quit.</summary>
    QuitGame,
}
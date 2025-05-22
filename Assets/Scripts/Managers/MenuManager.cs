using Application_Manager.Events; // Assuming ApplicationEvent is defined in this namespace.
using Application_Manager.States; // Assuming ChangingSceneState is defined in this namespace.
using UnityEngine;

/// <summary>
/// Manages main menu UI interactions and events, such as starting the game or exiting.
/// Implements a singleton pattern for easy access within the main menu scene.
/// </summary>
public class MenuManager : MonoBehaviour {

    /// <summary>
    /// Static singleton instance of the MenuManager, similar to GameManager's `s_instance`.
    /// </summary>
    [Tooltip("Static singleton instance of the MenuManager.")]
    public static MenuManager s_instance;

    /// <summary>
    /// Event raised when the main menu is initialized or becomes active.
    /// Must be assigned in the Inspector.
    /// </summary>
    [Tooltip("Event raised when the menu becomes active. Must be assigned.")]
    [SerializeField] private ApplicationEvent menuEvent;

    /// <summary>
    /// Event raised to signal the start of the game.
    /// Must be assigned in the Inspector.
    /// </summary>
    [Tooltip("Event raised to start the game. Must be assigned.")]
    [SerializeField] private ApplicationEvent startGameEvent;

    /// <summary>
    /// Event raised to signal exiting the game.
    /// Must be assigned in the Inspector.
    /// </summary>
    [Tooltip("Event raised to exit the game. Must be assigned.")]
    [SerializeField] private ApplicationEvent exitGameEvent;

    /// <summary>
    /// Reference to a ScriptableObject state that handles scene changes.
    /// Used here to set the target scene name when starting the game.
    /// Must be assigned in the Inspector.
    /// </summary>
    [Tooltip("Reference to the ChangingSceneState ScriptableObject for loading the next scene. Must be assigned.")]
    [SerializeField] private ChangingSceneState nextScene; // Note: Modifying sceneName on a ScriptableObject asset at runtime can have side effects if shared.

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Implements a simple singleton pattern for the menu scene.
    /// </summary>
    private void Awake() {
        // Simplified singleton check: if another MenuManager exists, destroy this one.
        // This MenuManager is typically specific to the main menu scene.
        MenuManager[] managers = FindObjectsOfType<MenuManager>();
        if (managers.Length > 1 && managers[0] != this) { // Check if another instance exists and it's not this one
            Destroy(gameObject);
            return;
        }
        s_instance = this; // Set this as the singleton instance.
    }

    /// <summary>
    /// Called before the first frame update, after Awake.
    /// Raises the menuEvent to signal that the menu is active.
    /// </summary>
    private void Start() {
        // Signal that the menu is active (e.g., for UI setup or enabling menu controls).
        if (menuEvent != null) {
            menuEvent.Raise();
        } else {
            Debug.LogError("MenuManager: menuEvent is not assigned in the Inspector.", this);
        }
    }

    /// <summary>
    /// Initiates the game start sequence.
    /// Sets the target scene name and raises the startGameEvent.
    /// </summary>
    public void startGame() {
        // Set the scene name on the ScriptableObject state.
        // Warning: Modifying ScriptableObject assets at runtime is generally discouraged if the asset is shared.
        // It's often better to pass data like scene names through events or have a dedicated scene loader service.
        if (nextScene != null) {
            nextScene.sceneName = "Level1"; // Hardcoded scene name for starting the game.
        } else {
            Debug.LogError("MenuManager: nextScene (ChangingSceneState) is not assigned in the Inspector. Cannot set scene name.", this);
            return; // Do not proceed if nextScene is null, as startGameEvent might rely on it.
        }

        // Raise the event to signal that the game should start.
        // Listeners (e.g., GameManager or ApplicationManager) should handle the actual scene loading.
        if (startGameEvent != null) {
            startGameEvent.Raise();
        } else {
            Debug.LogError("MenuManager: startGameEvent is not assigned in the Inspector. Cannot start game.", this);
        }
        
        // The lines below show a previous direct coupling to GameManager.
        // The current event-driven approach (startGameEvent.Raise()) is generally preferred for better decoupling.
        // GameManager.s_instance.setNewLevelName("Level1");
        // GameManager.s_instance.changeGameSate(GameState.LoadLevel);
    }

    /// <summary>
    /// Initiates the game exit sequence by raising the exitGameEvent.
    /// </summary>
    public void exitGame() {
        // Raise the event to signal that the game should exit.
        // Listeners (e.g., GameManager or ApplicationManager) should handle the actual application quit logic.
        if (exitGameEvent != null) {
            exitGameEvent.Raise();
        } else {
            Debug.LogError("MenuManager: exitGameEvent is not assigned in the Inspector. Cannot exit game.", this);
        }

        // The line below shows a previous direct coupling to GameManager.
        // The current event-driven approach (exitGameEvent.Raise()) is generally preferred.
        // GameManager.s_instance.changeGameSate(GameState.QuitGame);
    }
}
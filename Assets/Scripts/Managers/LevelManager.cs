using Application_Manager.Events; // Assuming ApplicationEvent is defined in this namespace.
using Cinemachine; // For CinemachineVirtualCamera.
using UnityEngine;

/// <summary>
/// Manages level-specific logic, including player spawning, game over UI, and level transitions.
/// Implements a singleton pattern for easy access within the current level.
/// </summary>
public class LevelManager : MonoBehaviour {

    /// <summary>
    /// Static singleton instance of the LevelManager for the current level.
    /// </summary>
    [Tooltip("Static singleton instance of the LevelManager.")]
    public static LevelManager instance;

    /// <summary>
    /// CanvasGroup for the game over screen UI. Used to fade it in.
    /// Must be assigned in the Inspector.
    /// </summary>
    [Tooltip("CanvasGroup for the game over screen. Must be assigned.")]
    [SerializeField] private CanvasGroup gameOverCanvasGroup;

    /// <summary>
    /// The position where the player will be spawned at the start of the level.
    /// </summary>
    [Tooltip("Player spawn position at the start of the level.")]
    [SerializeField] private Vector2 spawnPosition;

    /// <summary>
    /// Player prefab to instantiate if no PlayerManager (and thus player) exists.
    /// This is a fallback and assumes the prefab contains necessary player components.
    /// Must be assigned in the Inspector if this fallback is desired.
    /// </summary>
    [Tooltip("Player prefab to instantiate if no PlayerManager exists. Assign if needed.")]
    [SerializeField] private GameObject player;

    /// <summary>
    /// Name of the next level to load. (Currently unused in this script but kept for potential future use).
    /// </summary>
    [Tooltip("Name of the next level (currently for reference, not directly used by this script's methods).")]
    [SerializeField] private string nextLevelName; // Note: This field is serialized but not actively used by current methods.

    /// <summary>
    /// Reference to the Cinemachine Virtual Camera that follows the player.
    /// Must be assigned in the Inspector.
    /// </summary>
    [Tooltip("Cinemachine Virtual Camera to follow the player. Must be assigned.")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    /// <summary>
    /// Event raised when the level has been entered and basic setup is complete.
    /// Must be assigned in the Inspector.
    /// </summary>
    [Tooltip("Event raised when the level is entered. Must be assigned.")]
    [SerializeField] private ApplicationEvent enteredLevelEvent;

    /// <summary>
    /// Event raised to signal that the current level should be restarted.
    /// Must be assigned in the Inspector.
    /// </summary>
    [Tooltip("Event raised to restart the current level. Must be assigned.")]
    [SerializeField] private ApplicationEvent restartLevelEvent;

    /// <summary>
    /// Event raised to signal a return to the main menu.
    /// Must be assigned in the Inspector.
    /// </summary>
    [Tooltip("Event raised to return to the main menu. Must be assigned.")]
    [SerializeField] private ApplicationEvent goToMenuEvent;

    /// <summary>
    /// Event raised to signal proceeding to the next level.
    /// Must be assigned in the Inspector.
    /// </summary>
    [Tooltip("Event raised to proceed to the next level. Must be assigned.")]
    [SerializeField] private ApplicationEvent goToNextLevelEvent;

    private bool m_isShowingYouDiedScreen; // Flag to control the game over screen fade-in.

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Implements a simple singleton pattern for the level and initializes UI and player.
    /// </summary>
    private void Awake() {
        // Simplified singleton check: if another LevelManager exists, destroy this one.
        // This LevelManager is typically specific to a scene.
        LevelManager[] managers = FindObjectsOfType<LevelManager>();
        if (managers.Length > 1 && managers[0] != this) { // Check if another instance (usually the first found) exists and it's not this one
            Destroy(gameObject);
            return;
        }
        instance = this; // Set this as the singleton instance.

        m_isShowingYouDiedScreen = false; // Initialize game over screen flag.

        // Initialize game over screen alpha to transparent.
        if (gameOverCanvasGroup != null) {
            gameOverCanvasGroup.alpha = 0;
        } else {
            Debug.LogError("LevelManager: gameOverCanvasGroup is not assigned in the Inspector.", this);
        }

        // Fallback player instantiation: if no PlayerManager exists (implying no player),
        // instantiate the player prefab. This is a basic safeguard.
        if (PlayerManager.instance == null) {
            if (player != null) {
                Instantiate(player);
                Debug.Log("LevelManager: PlayerManager.instance not found, instantiated player prefab as a fallback.", this);
            } else {
                Debug.LogError("LevelManager: PlayerManager.instance not found and player prefab is not assigned. Cannot instantiate player.", this);
            }
        }
    }

    /// <summary>
    /// Called before the first frame update, after Awake.
    /// Sets up player position, state, controls, and camera. Raises level entered event.
    /// </summary>
    private void Start() {
        // Signal that the level has been entered and basic setup is starting.
        if (enteredLevelEvent != null) {
            enteredLevelEvent.Raise();
        } else {
            Debug.LogError("LevelManager: enteredLevelEvent is not assigned in the Inspector.", this);
        }

        if (PlayerManager.instance != null) {
            // Position the player at the designated spawn point.
            PlayerManager.instance.transform.position = spawnPosition;
            // Set initial player state (e.g., Idle).
            PlayerManager.instance.changePlayerSate(PlayerState.Idle);

            // Ensure player controls are enabled.
            if (PlayerController.instance != null) {
                PlayerController.instance.enabled = true;
            } else {
                Debug.LogError("LevelManager: PlayerController.instance is null. Cannot enable player controls.", this);
            }

            // Set the Cinemachine virtual camera to follow the player.
            if (virtualCamera != null) {
                virtualCamera.Follow = PlayerManager.instance.transform;
            } else {
                Debug.LogError("LevelManager: virtualCamera is not assigned in the Inspector.", this);
            }
        } else {
            Debug.LogError("LevelManager: PlayerManager.instance is null in Start. Cannot setup player position, state, or camera.", this);
        }
    }

    /// <summary>
    /// Called every frame.
    /// Handles the fade-in effect for the game over screen if it's supposed to be showing.
    /// </summary>
    private void Update() {
        // Continuously update the game over screen's visibility if triggered.
        showingGameOverScreen();
    }

    /// <summary>
    /// Initiates the display of the game over screen.
    /// Sets a flag that `Update` will use to start fading in the UI.
    /// </summary>
    public void showGameOverScreen() {
        m_isShowingYouDiedScreen = true;
    }
    
    /// <summary>
    /// Signals that the current level should be restarted.
    /// Raises the restartLevelEvent.
    /// </summary>
    public void restartLevel() {
        Debug.Log("LevelManager: Restarting level...", this);
        if (restartLevelEvent != null) {
            restartLevelEvent.Raise();
        } else {
            Debug.LogError("LevelManager: restartLevelEvent is not assigned in the Inspector.", this);
        }
    }
    
    /// <summary>
    /// Signals that the game should return to the main menu.
    /// Raises the goToMenuEvent.
    /// </summary>
    public void returnToMenu() {
        Debug.Log("LevelManager: Returning to menu...", this);
        if (goToMenuEvent != null) {
            goToMenuEvent.Raise();
        } else {
            Debug.LogError("LevelManager: goToMenuEvent is not assigned in the Inspector.", this);
        }
    }
    
    /// <summary>
    /// Signals that the current level is completed and the game should proceed to the next level.
    /// Raises the goToNextLevelEvent.
    /// </summary>
    public void endLevel() {
        Debug.Log("LevelManager: Ending level, proceeding to next...", this);
        if (goToNextLevelEvent != null) {
            goToNextLevelEvent.Raise();
        } else {
            Debug.LogError("LevelManager: goToNextLevelEvent is not assigned in the Inspector.", this);
        }
    }

    /// <summary>
    /// Handles the gradual fade-in of the game over screen UI.
    /// Called from Update.
    /// </summary>
    private void showingGameOverScreen() {
        if (!m_isShowingYouDiedScreen) {
            return; // Do nothing if the screen shouldn't be showing.
        }
        if (gameOverCanvasGroup != null) {
            // Increase alpha over time, clamping at 1 (fully opaque).
            gameOverCanvasGroup.alpha = Mathf.Min(gameOverCanvasGroup.alpha + Time.deltaTime, 1f);
        }
        // No else needed here as error for null gameOverCanvasGroup is logged in Awake.
    }
}
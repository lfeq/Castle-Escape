// Assuming Application_Manager.Events is not directly used here, but kept if other scripts rely on it.
// using Application_Manager.Events; 
using UnityEngine;

/// <summary>
/// Manages the player's state, animations, and interactions with game objects.
/// It handles the core logic of the player but not direct input control (which is typically in PlayerController).
/// Implements a persistent singleton pattern.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerManager : MonoBehaviour {

    /// <summary>
    /// Static singleton instance of the PlayerManager.
    /// Allows global access to player logic and state.
    /// </summary>
    [Tooltip("Static singleton instance of the PlayerManager.")]
    public static PlayerManager instance;

    /// <summary>
    /// Reference to the currently interactable GameObject the player is near.
    /// This is typically set when the player enters an interactable object's trigger.
    /// </summary>
    [Tooltip("Reference to the currently interactable GameObject.")]
    public GameObject interactableObject;

    /// <summary>
    /// Flag indicating whether the player currently possesses a key.
    /// Used for interactions that require a key.
    /// </summary>
    [Tooltip("Does the player currently have a key?")]
    public bool hasKey;

    /// <summary>
    /// UI GameObject used to prompt the player for interaction (e.g., an "E" key icon).
    /// Should be assigned in the Inspector.
    /// </summary>
    [Tooltip("UI GameObject to show interaction prompts. Must be assigned.")]
    [SerializeField] private GameObject promptObject;

    private Animator m_animator; // Reference to the Animator component for controlling player animations.
    private PlayerState m_playerState; // Current logical state of the player.
    private Rigidbody2D m_rb2d; // Reference to the Rigidbody2D component for physics operations.

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes components, implements the singleton pattern, and ensures persistence.
    /// </summary>
    private void Awake() {
        // Get the Animator component attached to this GameObject.
        m_animator = GetComponent<Animator>();
        if (m_animator == null) {
            // This should ideally not happen due to [RequireComponent]
            Debug.LogError("PlayerManager: Animator component not found on this GameObject!", this);
        }
        
        // Get the Rigidbody2D component.
        m_rb2d = GetComponent<Rigidbody2D>();
        if (m_rb2d == null) {
            // This should ideally not happen due to [RequireComponent]
            Debug.LogError("PlayerManager: Rigidbody2D component not found on this GameObject!", this);
        }

        // Singleton pattern implementation:
        // Ensures only one instance of PlayerManager exists.
        // If another instance exists, destroy this new one.
        PlayerManager[] managers = FindObjectsOfType<PlayerManager>();
        if (managers.Length > 1 && managers[0] != this) {
             Destroy(gameObject);
             return;
        }
        instance = this; // Set this as the singleton instance.
        
        // Persist this PlayerManager GameObject across scene loads.
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Called before the first frame update, after Awake.
    /// Initializes player state and UI elements.
    /// </summary>
    private void Start() {
        // Ensure the interaction prompt is initially hidden.
        if (promptObject != null) {
            promptObject.SetActive(false);
        } else {
            Debug.LogError("PlayerManager: promptObject is not assigned in the Inspector. Interaction prompts will not work.", this);
        }
        m_playerState = PlayerState.None; // Set initial player state.
    }

    /// <summary>
    /// Changes the player's current state and updates the Animator accordingly.
    /// Prevents redundant state changes.
    /// </summary>
    /// <param name="t_newSate">The new PlayerState to transition to.</param>
    public void changePlayerSate(PlayerState t_newSate) {
        // Avoid redundant state changes or re-triggering animations if already in the desired state.
        if (m_playerState == t_newSate) {
            return;
        }

        if (m_animator != null) {
            resetAnimatorParameters(); // Clear previous animation states.
        }
        m_playerState = t_newSate; // Update to the new state.

        // Update Animator based on the new state.
        // Noted Typos in Animator Parameters (from original code): "isIdeling", "isJumpng".
        // These should match the actual parameter names in the Animator Controller.
        // For this review, the original names are kept.
        if (m_animator != null) {
            switch (m_playerState) {
                case PlayerState.None:
                    // No specific animation for 'None' state, or handled by resetAnimatorParameters.
                    break;
                case PlayerState.Idle:
                    m_animator.SetBool("isIdeling", true); // Typo: "isIdling" is standard.
                    break;
                case PlayerState.Running:
                    m_animator.SetBool("isRunning", true);
                    break;
                case PlayerState.Jump:
                    m_animator.SetBool("isJumpng", true); // Typo: "isJumping" is standard.
                    break;
                case PlayerState.JumpFall:
                case PlayerState.FreeFall: // Both JumpFall and FreeFall use the "isFalling" animation.
                    m_animator.SetBool("isFalling", true);
                    break;
                case PlayerState.Dead:
                    if(m_rb2d != null) m_rb2d.linearVelocity = Vector2.zero; // Stop player movement on death.
                    m_animator.SetBool("isDying", true); // Typo: "isDead" or "Died" might be more common.
                    break;
            }
        } else {
            Debug.LogWarning("PlayerManager: m_animator is null. Cannot update player animations.", this);
        }
    }

    /// <summary>
    /// Resets all boolean parameters in the Animator to false.
    /// This ensures that only the animation for the current state is active.
    /// </summary>
    private void resetAnimatorParameters() {
        if (m_animator == null) return;
        // Iterate through all parameters in the Animator Controller.
        foreach (AnimatorControllerParameter parameter in m_animator.parameters) {
            // If the parameter is a boolean, set it to false.
            if (parameter.type == AnimatorControllerParameterType.Bool) {
                m_animator.SetBool(parameter.name, false);
            }
        }
    }

    /// <summary>
    /// Called when another Collider2D enters a trigger attached to the Player GameObject.
    /// Used to detect interactable objects.
    /// </summary>
    /// <param name="t_collision">The Collider2D data associated with the other object.</param>
    private void OnTriggerEnter2D(Collider2D t_collision) {
        // Check if the collided object is tagged as "Interactable".
        if (t_collision.CompareTag("Interactable")) {
            if (promptObject != null) {
                promptObject.SetActive(true); // Show interaction prompt.
            }
            interactableObject = t_collision.gameObject; // Store reference to the interactable object.
        }
    }

    /// <summary>
    /// Called when another Collider2D exits a trigger attached to the Player GameObject.
    /// Used to clear references to interactable objects.
    /// </summary>
    /// <param name="t_collision">The Collider2D data associated with the other object.</param>
    private void OnTriggerExit2D(Collider2D t_collision) {
        // Check if the object being exited is tagged as "Interactable".
        if (t_collision.CompareTag("Interactable")) {
            if (promptObject != null) {
                promptObject.SetActive(false); // Hide interaction prompt.
            }
            interactableObject = null; // Clear reference to the interactable object.
        }
    }

    /// <summary>
    /// Sets the player's `hasKey` flag to true.
    /// Typically called when the player picks up a key item.
    /// </summary>
    public void getKey() {
        hasKey = true;
        Debug.Log("PlayerManager: Player has obtained a key.", this);
    }
}

/// <summary>
/// Defines the various states the player character can be in.
/// </summary>
public enum PlayerState {
    /// <summary>No specific state / default state.</summary>
    None,
    /// <summary>Player is idle.</summary>
    Idle,
    /// <summary>Player is running.</summary>
    Running,
    /// <summary>Player is actively jumping (upwards movement).</summary>
    Jump,
    /// <summary>Player is falling after a jump.</summary>
    JumpFall,
    /// <summary>Player is falling from a height (not necessarily from a jump).</summary>
    FreeFall,
    /// <summary>Player is dead or in a dying state.</summary>
    Dead
}
using UnityEngine;

/// <summary>
/// Se encarga el control del player, pero no de la logica de juego
/// </summary>
[RequireComponent(typeof(PlayerManager))]
public class PlayerController : MonoBehaviour {


    #region Public
    /// <summary>
    /// Singleton instance of the PlayerController.
    /// Provides a global access point to the PlayerController.
    /// </summary>
    public static PlayerController instance;

    /// <summary>
    /// LayerMask defining what constitutes "ground" for the player.
    /// Used in ground detection checks.
    /// </summary>
    public LayerMask whatIsGround;
    #endregion

    #region Private
    private Rigidbody2D m_rb2D; // Reference to the Rigidbody2D component for physics operations.
    private bool m_isFacingRight; // Tracks the direction the player is currently facing. True if right, false if left.
    private bool m_isGrounded; // Indicates whether the player is currently on the ground.
    #endregion

    #region Serialize Fields
    [SerializeField] private float xSpeed = 5f; // Speed of horizontal movement.
    [SerializeField] private float jumpForce = 10f; // Force applied when the player jumps.
    [SerializeField] private float footRadius = 0.1f; // Radius of the OverlapCircle used for ground detection at the player's feet.
    [SerializeField] private Transform footPosition = null; // Transform representing the position of the player's feet, used for ground detection.
    #endregion

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes the singleton instance and default player direction.
    /// </summary>
    private void Awake() {
        // Singleton pattern implementation
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }

        m_isFacingRight = true; // Player starts facing right by default
        m_isGrounded = false; // Player starts airborne (will be updated in FixedUpdate)
    }

    /// <summary>
    /// Called before the first frame update.
    /// Gets the Rigidbody2D component attached to this GameObject.
    /// </summary>
    void Start() {
        m_rb2D = GetComponent<Rigidbody2D>();
        if (m_rb2D == null) {
            Debug.LogError("PlayerController: Rigidbody2D component not found on the GameObject.");
        }
        if (footPosition == null) {
            Debug.LogError("PlayerController: Foot Position Transform is not assigned in the Inspector.");
        }
    }

    /// <summary>
    /// Called every fixed framerate frame.
    /// Handles physics-based updates like ground detection and movement.
    /// </summary>
    private void FixedUpdate() {
        // Ground Check:
        // An OverlapCircle is cast at the footPosition to detect colliders on the 'whatIsGround' layer.
        // The additional condition 'm_rb2D.linearVelocity.y < 0.1f' ensures that the player is considered grounded
        // only if they are not moving upwards (e.g., during the initial phase of a jump).
        // This helps prevent sticking to ceilings or re-grounding too early while still ascending.
        if (footPosition != null) { // Null check for footPosition
            m_isGrounded = Physics2D.OverlapCircle(footPosition.position, footRadius, whatIsGround) &&
                                    (m_rb2D != null && m_rb2D.linearVelocity.y < 0.1f); // Null check for m_rb2D
        } else {
            m_isGrounded = false; // If footPosition is not set, assume not grounded to prevent errors.
        }


        // Apply movement if Rigidbody2D is available
        if (m_rb2D != null) {
            horizontalMovement();
            verticalMovement();
        }
    }

    /// <summary>
    /// Handles the player's horizontal movement based on input.
    /// Updates player state to Running or Idle if grounded.
    /// </summary>
    private void horizontalMovement() {
        float xMove = Input.GetAxisRaw("Horizontal"); // Get raw horizontal input (-1, 0, or 1)

        // Apply horizontal velocity
        m_rb2D.linearVelocity = new Vector2(xMove * xSpeed, m_rb2D.linearVelocity.y);

        // Flip the player sprite if moving in the opposite direction of where they are facing
        if ((xMove < 0 && m_isFacingRight) || (xMove > 0 && !m_isFacingRight)) {
            flip();
        }

        // Update player state based on movement if grounded
        if (m_isGrounded) {
            if (xMove != 0) {
                // Player is moving horizontally
                if (PlayerManager.instance != null) PlayerManager.instance.changePlayerSate(PlayerState.Running);
            } else {
                // Player is stationary
                if (PlayerManager.instance != null) PlayerManager.instance.changePlayerSate(PlayerState.Idle);
            }
        }
    }

    /// <summary>
    /// Handles the player's vertical state changes (Jump, JumpFall).
    /// This method determines the player's state based on their vertical velocity when not grounded.
    /// </summary>
    private void verticalMovement() {
        // This logic only applies if the player is airborne
        if (m_isGrounded) {
            return;
        }

        // Check vertical velocity to determine jump state
        if (m_rb2D.linearVelocity.y >= 0.1f) {
            // Player is moving upwards
            if (PlayerManager.instance != null) PlayerManager.instance.changePlayerSate(PlayerState.Jump);
        } else if (m_rb2D.linearVelocity.y < -0.1f) {
            // Player is moving downwards
            if (PlayerManager.instance != null) PlayerManager.instance.changePlayerSate(PlayerState.JumpFall);
        }
    }

    /// <summary>
    /// Called once per frame.
    /// Handles frame-based input like jumping and interactions.
    /// </summary>
    void Update() {
        // Handle jump input
        if (Input.GetButtonDown("Jump")) {
            jump();
        }

        // Handle interaction input (e.g., pressing 'F' key)
        if (Input.GetKeyDown(KeyCode.F)) {
            HandleInteraction();
        }
    }
    
    /// <summary>
    /// Processes player interaction with interactable objects.
    /// Called when the interaction key (e.g., 'F') is pressed.
    /// Checks for an interactable object, its ItemBehaviour component,
    /// and whether a key is required for interaction.
    /// </summary>
    private void HandleInteraction() {
        // Ensure PlayerManager instance and interactable object are available
        if (PlayerManager.instance == null || PlayerManager.instance.interactableObject == null) {
            // Optional: Log a warning if interaction is attempted without necessary setup
            // Debug.LogWarning("PlayerManager instance or interactableObject is missing.");
            return;
        }

        GameObject interactable = PlayerManager.instance.interactableObject;
        ItemBehaviour itemBehaviour = interactable.GetComponent<ItemBehaviour>();

        // Check if the interactable object has an ItemBehaviour component
        if (itemBehaviour == null) {
            // Optional: Log a warning if the interactable object is missing the required component
            // Debug.LogWarning($"Interactable object '{interactable.name}' is missing ItemBehaviour component.");
            return;
        }

        // Check if the item requires a key and if the player has the key
        if (itemBehaviour.requiresKey && (PlayerManager.instance.hasKey == false)) {
            // Optional: Provide feedback to the player that they need a key
            // Debug.Log("This item requires a key.");
            return;
        }
        
        // If all checks pass, activate the item
        itemBehaviour.onActivate.Invoke();
    }


    /// <summary>
    /// Makes the player jump if they are grounded.
    /// Applies an upward force to the player's Rigidbody.
    /// </summary>
    private void jump() {
        // Player can only jump if grounded and Rigidbody is present
        if (!m_isGrounded || m_rb2D == null) {
            return;
        }
        // Apply vertical force for the jump
        m_rb2D.linearVelocity = new Vector2(m_rb2D.linearVelocity.x, jumpForce);
        // Optionally, change player state to Jump immediately after initiating a jump
        // This might be redundant if verticalMovement already handles it, but can make state transitions more immediate.
        // if (PlayerManager.instance != null) PlayerManager.instance.changePlayerSate(PlayerState.Jump);
    }

    /// <summary>
    /// Flips the player character horizontally by rotating the transform.
    /// Updates the m_isFacingRight state.
    /// </summary>
    private void flip() {
        // Rotate the player 180 degrees around the Y-axis
        transform.Rotate(0, 180, 0);
        // Toggle the facing direction state
        m_isFacingRight = !m_isFacingRight;
    }

    /// <summary>
    /// Called in the editor to draw gizmos for visualization.
    /// Draws a sphere at the footPosition to visualize the ground check area.
    /// </summary>
    private void OnDrawGizmos() {
        if (footPosition == null) return; // Avoid errors if footPosition is not set

        Gizmos.color = Color.cyan; // Set gizmo color
        Gizmos.DrawSphere(footPosition.position, footRadius); // Draw ground check sphere
    }
}
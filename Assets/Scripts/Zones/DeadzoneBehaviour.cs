using UnityEngine;

/// <summary>
/// Defines a "dead zone" area. If a GameObject tagged as "Player" enters this zone's trigger,
/// it initiates the game over sequence for the player.
/// </summary>
public class DeadzoneBehaviour : MonoBehaviour {
    // Flag to ensure the death sequence is triggered only once,
    // preventing issues if OnTriggerEnter2D is called multiple times rapidly for the same player.
    private bool m_isPlayerDeadReported = false;

    /// <summary>
    /// Called when another Collider2D enters the trigger attached to this Deadzone GameObject.
    /// Checks if the colliding object is the player and, if so, triggers the game over sequence.
    /// </summary>
    /// <param name="t_collision">The Collider2D data associated with the other object involved in the collision.</param>
    private void OnTriggerEnter2D(Collider2D t_collision) {
        // Check if the death sequence has already been triggered by this dead zone instance.
        if (m_isPlayerDeadReported) {
            return;
        }

        // Efficiently checks if the colliding object has the "Player" tag.
        // Ensure your player GameObject is tagged "Player" in the Inspector.
        if (t_collision.CompareTag("Player")) {
            Debug.Log("Player entered a dead zone.", this);
            m_isPlayerDeadReported = true; // Set flag to true to prevent re-triggering.

            // --- Sequence of actions to handle player death ---

            // 1. Change Player State to Dead:
            // This updates the player's logical state, potentially used by other systems (UI, achievements, etc.).
            if (PlayerManager.instance != null) {
                PlayerManager.instance.changePlayerSate(PlayerState.Dead);
            } else {
                Debug.LogError("DeadzoneBehaviour: PlayerManager.instance is null. Cannot change player state.", this);
            }

            // 2. Disable Player Controls:
            // Prevents further input from the player.
            // Consider: A more centralized method like PlayerManager.instance.DisablePlayer() 
            // could handle this and other player-specific death behaviors (e.g., animations, sound) more cleanly.
            if (PlayerController.instance != null) {
                PlayerController.instance.enabled = false;
            } else {
                Debug.LogError("DeadzoneBehaviour: PlayerController.instance is null. Cannot disable player controls.", this);
            }

            // 3. Show Game Over Screen:
            // Triggers the UI or logic to display the game over interface.
            if (LevelManager.instance != null) {
                LevelManager.instance.showGameOverScreen();
            } else {
                Debug.LogError("DeadzoneBehaviour: LevelManager.instance is null. Cannot show game over screen.", this);
            }
        }
    }
}

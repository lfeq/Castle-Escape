using System;
using UnityEngine;

/// <summary>
/// Controls the behavior of an enemy unit, including movement towards the player and collision handling.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))] // Ensure Rigidbody2D is present
public class EnemyBehaviour : MonoBehaviour {

    /// <summary>
    /// The speed at which the enemy moves.
    /// </summary>
    [SerializeField] private float moveSpeed = 5f;
    
    private Rigidbody2D m_rb2d; // Reference to the Rigidbody2D component for physics-based movement.
    private Vector2 m_direction; // Current movement direction of the enemy.
    
    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes the Rigidbody2D component and sets the initial movement direction.
    /// </summary>
    private void Start() {
        m_rb2d = GetComponent<Rigidbody2D>();
        if (m_rb2d == null) {
            Debug.LogError("EnemyBehaviour: Rigidbody2D component not found on this GameObject. Disabling script.", this);
            enabled = false; // Disable this script component if Rigidbody2D is missing
            return;
        }
        
        // Set the initial direction towards the player.
        resetDirection();
    }

    /// <summary>
    /// Called every fixed framerate frame.
    /// Applies movement to the enemy based on the current direction and speed.
    /// </summary>
    private void FixedUpdate() {
        // Only move if Rigidbody2D is available (it should be due to Start() check, but as a safeguard)
        if (m_rb2d != null) {
            // Apply velocity to move the enemy
            m_rb2d.linearVelocity = m_direction * moveSpeed;
        }
    }

    /// <summary>
    /// Called when another Collider2D enters a trigger attached to this object.
    /// Handles collision with the player, triggering game over logic.
    /// </summary>
    /// <param name="t_collision">The Collider2D data associated with the other object involved in the collision.</param>
    private void OnTriggerEnter2D(Collider2D t_collision) {
        // Check if the colliding object is tagged as "Player"
        if (t_collision.CompareTag("Player")) {
            // Handle player collision
            
            // Change player state to Dead, if PlayerManager exists
            if (PlayerManager.instance != null) {
                PlayerManager.instance.changePlayerSate(PlayerState.Dead);
            } else {
                Debug.LogWarning("EnemyBehaviour: PlayerManager.instance is null. Cannot change player state.", this);
            }

            // Disable player controls, if PlayerController exists
            // Consider: A method like PlayerManager.instance.DisablePlayerControl() might be a cleaner approach for encapsulation.
            if (PlayerController.instance != null) {
                PlayerController.instance.enabled = false;
            } else {
                Debug.LogWarning("EnemyBehaviour: PlayerController.instance is null. Cannot disable player controls.", this);
            }

            // Show game over screen, if LevelManager exists
            if (LevelManager.instance != null) {
                LevelManager.instance.showGameOverScreen();
            } else {
                Debug.LogWarning("EnemyBehaviour: LevelManager.instance is null. Cannot show game over screen.", this);
            }
        }
    }

    /// <summary>
    /// Recalculates the movement direction to face the current player position.
    /// If the player cannot be found, the enemy will stop moving (direction set to zero).
    /// </summary>
    public void resetDirection() {
        if (PlayerManager.instance != null && PlayerManager.instance.transform != null) {
            // Calculate the direction vector from the enemy to the player
            // Normalize the vector to get a unit vector (direction only, magnitude 1)
            m_direction = (PlayerManager.instance.transform.position - transform.position).normalized;
        } else {
            // If PlayerManager or its transform is not available, stop movement
            Debug.LogWarning("EnemyBehaviour: PlayerManager.instance or its transform is null. Enemy will stop moving.", this);
            m_direction = Vector2.zero; 
        }
    }
}
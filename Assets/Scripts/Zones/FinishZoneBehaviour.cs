using Application_Manager.Events; // Assumes ApplicationEvent is defined in this namespace.
using UnityEngine;

namespace Zones {
    /// <summary>
    /// Defines a "finish zone" that the player can enter to complete a level.
    /// When a GameObject tagged as "Player" enters this zone's trigger, it raises an event
    /// to signal level completion and typically initiate loading of the next level or a similar action.
    /// </summary>
    public class FinishZoneBehaviour : MonoBehaviour {

        /// <summary>
        /// The ApplicationEvent that is raised when the player enters the finish zone.
        /// This event should be configured in the Unity Inspector and have listeners
        /// (e.g., a GameManager or LevelManager) that handle the transition to the next level,
        /// showing a completion screen, etc.
        /// </summary>
        [Tooltip("The event to raise when the player reaches the finish zone. Must be assigned in the Inspector.")]
        [SerializeField] private ApplicationEvent goToNextLevelEvent;
        
        // Flag to ensure the level completion event is raised only once,
        // preventing issues if OnTriggerEnter2D is called multiple times for the same player.
        private bool m_isLevelFinished = false;

        /// <summary>
        /// Called when another Collider2D enters the trigger attached to this FinishZone GameObject.
        /// Checks if the colliding object is the player and, if so, raises the level completion event.
        /// </summary>
        /// <param name="t_collision">The Collider2D data associated with the other object involved in the collision.</param>
        private void OnTriggerEnter2D(Collider2D t_collision) {
            // Check if the level finished event has already been triggered by this zone instance.
            if (m_isLevelFinished) {
                return;
            }

            // Efficiently checks if the colliding object has the "Player" tag.
            // Ensure your player GameObject is tagged "Player" in the Unity Inspector.
            if (t_collision.CompareTag("Player")) {
                
                // This log is useful for debugging in development to confirm the finish zone was triggered.
                // Consider wrapping with #if UNITY_EDITOR || DEVELOPMENT_BUILD if it should not appear in release builds.
                Debug.Log("Player has reached the finish zone.", this);

                // Check if the goToNextLevelEvent is assigned in the Inspector.
                if (goToNextLevelEvent != null) {
                    // Raise the event. Listeners (e.g., a GameManager) will react to this.
                    // An ApplicationEvent is typically a ScriptableObject that allows for a decoupled event system.
                    // The Raise() method invokes all registered listener actions.
                    goToNextLevelEvent.Raise();
                    
                    // Set flag to true to prevent re-triggering.
                    m_isLevelFinished = true; 
                } else {
                    // Log an error if the event is not assigned, as this is a required dependency for the component to function.
                    Debug.LogError("FinishZoneBehaviour: goToNextLevelEvent is not assigned in the Inspector. Cannot signal level completion.", this);
                }
            }
        }
    }
}
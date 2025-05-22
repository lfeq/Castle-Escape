using Application_Manager.Events; // Assumes ApplicationEvent is defined in this namespace.
using UnityEngine;

/// <summary>
/// Manages the display of game credits and provides functionality to return to the main menu.
/// It uses ApplicationEvents to signal when credits should be shown and when to transition back to the menu.
/// </summary>
public class CreditsManager : MonoBehaviour {

    /// <summary>
    /// Event raised when the credits scene/sequence should start.
    /// This should be assigned in the Inspector.
    /// </summary>
    [Tooltip("Event to raise when the credits sequence should begin. Must be assigned.")]
    [SerializeField] private ApplicationEvent creditsEvent;

    /// <summary>
    /// Event raised to signal a return to the main menu.
    /// This should be assigned in the Inspector.
    /// </summary>
    [Tooltip("Event to raise to return to the main menu. Must be assigned.")]
    [SerializeField] private ApplicationEvent goToMenuEvent;
    
    /// <summary>
    /// Called when the script instance is being loaded.
    /// Raises the creditsEvent to signal the start of the credits sequence.
    /// </summary>
    private void Start() {
        // Signal that the credits sequence should begin (e.g., start animations, display text).
        if (creditsEvent != null) {
            creditsEvent.Raise();
        } else {
            Debug.LogError("CreditsManager: creditsEvent is not assigned in the Inspector. Credits will not be triggered.", this);
        }
    }

    /// <summary>
    /// Public method that can be called (e.g., by a UI button) to trigger the transition back to the main menu.
    /// </summary>
    public void returnToMenu() {
        // Signal that the game should return to the main menu.
        if (goToMenuEvent != null) {
            goToMenuEvent.Raise();
        } else {
            Debug.LogError("CreditsManager: goToMenuEvent is not assigned in the Inspector. Cannot return to menu via event.", this);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Defines the behavior of an interactable item in the game.
/// Allows UnityEvents to be triggered upon activation and can optionally require a key.
/// </summary>
public class ItemBehaviour : MonoBehaviour {

    /// <summary>
    /// UnityEvent that is invoked when the item is activated.
    /// This can be configured in the Inspector to call public methods on other GameObjects,
    /// e.g., opening a door, triggering a sound, enabling another component, etc.
    /// </summary>
    public UnityEvent onActivate;

    /// <summary>
    /// If true, this item requires the player to possess a key (e.g., `PlayerManager.instance.hasKey == true`)
    /// for its `onActivate` event to be invoked.
    /// The actual key checking logic is typically handled by the interacting script (e.g., PlayerController).
    /// </summary>
    public bool requiresKey = false;

    /// <summary>
    /// Call this method to make the item "consumed" or used up, preventing further interaction
    /// by destroying this ItemBehaviour component from the GameObject.
    /// This is a common way to implement single-use items.
    /// </summary>
    /// <remarks>
    /// Destroying the component is a permanent action. If the item needs to be reset or used multiple times
    /// under different conditions, consider using a boolean flag (e.g., `isActivated = true;`) 
    /// and checking that flag before invoking `onActivate` instead of destroying the component.
    /// </remarks>
    public void ConsumeItemAndDestroyComponent() {
        // By destroying this component, the item effectively becomes non-interactable
        // through this script's mechanisms anymore.
        Destroy(this); 
    }
}

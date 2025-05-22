// Assuming Application_Manager, Application_Manager.Events, and Application_Manager.States
// are relevant namespaces, though not all are directly used in the visible code here.
// using Application_Manager;
// using Application_Manager.Events;
using Application_Manager.States; // BaseState is expected to be in this namespace.
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A ScriptableObject-based state that, when entered, loads the main menu scene.
/// This is typically used by a state machine to transition the application to the main menu.
/// </summary>
// The [CreateAssetMenu] attribute allows easy creation of instances of this ScriptableObject
// directly from the Assets/Create menu in the Unity Editor.
// "menuRootName" is presumably a constant defined elsewhere, likely in BaseState or a shared constants file.
[CreateAssetMenu(fileName = "New MainMenu State", menuName = BaseState.menuRootName + "MainMenuState")]
public class MainMenuState : BaseState {
    
    /// <summary>
    /// Called when this state is entered by the state machine.
    /// This method loads the "MainMenu" scene.
    /// </summary>
    public override void OnEnter() {
        // Calls the OnEnter method of the base class (BaseState).
        // The specific behavior of base.OnEnter() depends on the BaseState implementation.
        base.OnEnter();

        // Synchronously loads the "MainMenu" scene. 
        // This will freeze the application until the scene is fully loaded.
        // Ensure that a scene named "MainMenu" exists and is added to the Build Settings.
        // For smoother transitions, especially if the menu scene is complex or involves loading assets,
        // consider using SceneManager.LoadSceneAsync.
        // Example: StartCoroutine(LoadSceneAsyncWrapper("MainMenu")); (would require a MonoBehaviour context)
        Debug.Log("MainMenuState: Loading MainMenu scene synchronously.", this);
        SceneManager.LoadScene("MainMenu");
    }
}
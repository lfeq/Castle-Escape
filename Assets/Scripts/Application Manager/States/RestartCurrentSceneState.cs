using Application_Manager.States; // BaseState is expected to be in this namespace.
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Application_Manager.States {
    /// <summary>
    /// A ScriptableObject-based state that, when entered, reloads the currently active scene.
    /// This is typically used by a state machine to handle a "Restart Level" action.
    /// </summary>
    // The [CreateAssetMenu] attribute allows easy creation of instances of this ScriptableObject
    // directly from the Assets/Create menu in the Unity Editor.
    // "menuRootName" is presumably a constant defined elsewhere, likely in BaseState or a shared constants file.
    [CreateAssetMenu(fileName = "New Restart Current Scene State", menuName = BaseState.menuRootName + "Restarting Scene State")]
    public class RestartCurrentSceneState : BaseState {

        /// <summary>
        /// Called when this state is entered by the state machine.
        /// This method reloads the currently active scene.
        /// </summary>
        public override void OnEnter() {
            // Calls the OnEnter method of the base class (BaseState).
            // The specific behavior of base.OnEnter() depends on the BaseState implementation.
            base.OnEnter();

            // Get the name of the currently active scene.
            string currentSceneName = SceneManager.GetActiveScene().name;

            // Synchronously reloads the current scene.
            // This will freeze the application until the scene is fully reloaded.
            // For smoother transitions, especially if the scene is complex,
            // consider using SceneManager.LoadSceneAsync.
            // Example: StartCoroutine(LoadSceneAsyncWrapper(currentSceneName)); (would require a MonoBehaviour context)
            Debug.Log($"RestartCurrentSceneState: Restarting scene '{currentSceneName}' synchronously.", this);
            SceneManager.LoadScene(currentSceneName);
        }
    }
}
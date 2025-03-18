using Application_Manager.Events;
using UnityEngine;

public class MenuManager : MonoBehaviour {
    public static MenuManager s_instance;
    [SerializeField] private ApplicationEvent menuEvent;

    private void Awake() {
        if (FindObjectOfType<MenuManager>() != null &&
            FindObjectOfType<MenuManager>().gameObject != gameObject) {
            Destroy(gameObject);
            return;
        }
        s_instance = this;
    }

    private void Start() {
        menuEvent.Raise();
    }

    public void startGame() {
        GameManager.s_instance.setNewLevelName("Level1");
        GameManager.s_instance.changeGameSate(GameState.LoadLevel);
    }

    public void exitGame() {
        GameManager.s_instance.changeGameSate(GameState.QuitGame);
    }
}
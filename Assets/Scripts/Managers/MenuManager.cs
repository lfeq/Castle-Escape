using UnityEngine;

public class MenuManager : MonoBehaviour {
    public static MenuManager s_instance;

    private void Awake() {
        if(FindObjectOfType<MenuManager>() != null && 
           FindObjectOfType<MenuManager>().gameObject != gameObject) {
            Destroy(gameObject);
            return;
        }
        s_instance = this;
    }

    private void Start()
    {
        GameManager.s_instance.changeGameSate(GameState.MainMenu);
    }

    public void startGame() {
        GameManager.s_instance.setNewLevelName("Level1");
        GameManager.s_instance.changeGameSate(GameState.LoadLevel);
    }
}

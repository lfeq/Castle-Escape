using UnityEngine;

public class CreditsManager : MonoBehaviour
{
    private void Start() {
        GameManager.s_instance.changeGameSate(GameState.Credits);
    }

    public void returnToMenu() {
        GameManager.s_instance.changeGameSate(GameState.LoadMainMenu);
    }
}

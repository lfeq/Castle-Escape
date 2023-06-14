using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadzoneBehaviour : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D t_collision) {
        if (t_collision.CompareTag("Player")) {
            print("Muere");
            PlayerManager.instance.changePlayerSate(PlayerState.Dead);
            PlayerController.instance.enabled = false;
            LevelManager.instance.ShowYouDiedScreen();
            GameManager.s_instance.changeGameSate(GameState.GameOver);
        }
    }
}

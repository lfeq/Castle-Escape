using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadzoneBehaviour : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            PlayerManager.instance.changePlayerSate(PlayerState.Dead);
            PlayerController.instance.enabled = false;
            LevelManager.instance.ShowYouDiedScreen();
        }
    }
}

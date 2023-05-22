using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase encargada de toda la logica del Player, pero no del control
/// </summary>
public class PlayerManager : MonoBehaviour {

    /// <summary>
    /// Instancia publica de la clase PlayerManager
    /// </summary>
    public static PlayerManager instance;
    public GameObject interactableObject;
    public bool hasKey;

    [SerializeField] private GameObject promptObject;

    private Animator animator;
    private PlayerState playerState;
    private Rigidbody2D rb2d;

    private void Awake() {
        instance = this;
        playerState = PlayerState.None;
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start() {
        animator = GetComponent<Animator>();
        promptObject.SetActive(false);
    }

    public void changePlayerSate(PlayerState newSate) {
        if (playerState == newSate) {
            return;
        }
        resetAnimatorParameters();
        playerState = newSate;
        switch (playerState) {
            case PlayerState.None:
                break;
            case PlayerState.Idle:
                animator.SetBool("isIdeling", true);
                break;
            case PlayerState.Running:
                animator.SetBool("isRunning", true);
                break;
            case PlayerState.Jump:
                animator.SetBool("isJumpng", true);
                break;
            case PlayerState.JumpFall:
                animator.SetBool("isFalling", true);
                break;
            case PlayerState.FreeFall:
                animator.SetBool("isFalling", true);
                break;
            case PlayerState.Dead:
                rb2d.velocity = new Vector2(0, 0);
                animator.SetBool("isDying", true);
                break;
        }
    }

    private void resetAnimatorParameters() {
        foreach (AnimatorControllerParameter parameter in animator.parameters) {
            if (parameter.type == AnimatorControllerParameterType.Bool) {
                animator.SetBool(parameter.name, false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Interactable")) {
            promptObject.SetActive(true);
            interactableObject = collision.gameObject;
        }

        if (collision.CompareTag("Finish")) {
            LevelManager.instance.showYouWinScreen();
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Interactable")) {
            promptObject.SetActive(false);
            interactableObject = null;
        }
    }

    public void GetKey() {
        hasKey = true;
    }
}

public enum PlayerState {
    None,
    Idle,
    Running,
    Jump,
    JumpFall,
    FreeFall,
    Dead
}


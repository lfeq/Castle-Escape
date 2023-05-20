using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Se encarga el control del player, pero no de la logica de juego
/// </summary>
[RequireComponent(typeof(PlayerManager))]
public class PlayerController : MonoBehaviour {


    #region Public
    /// <summary>
    /// Instancia singleton de PlayerController
    /// </summary>
    public static PlayerController instance;
    public LayerMask whatIsGround;
    #endregion

    #region Private
    private Rigidbody2D rb2D;
    private bool isFacingRight, isGrounded;
    #endregion

    #region Serialize Fields
    [SerializeField] private float xSpeed, jumpForce, footRadious;
    [SerializeField] private Transform footPosition;
    #endregion

    private void Awake() {
        instance = this;
        isFacingRight = true;
        isGrounded = false;
    }

    // Start is called before the first frame update
    void Start() {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        isGrounded = Physics2D.OverlapCircle(footPosition.position, footRadious, whatIsGround) &&
                                rb2D.velocity.y < 0.1f;

        horizontalMovement();
        verticalMovement();
    }

    private void horizontalMovement() {
        float xMove = Input.GetAxisRaw("Horizontal");
        rb2D.velocity = new Vector2(xMove * xSpeed, rb2D.velocity.y);
        if ((xMove < 0 && isFacingRight) || (xMove > 0 && !isFacingRight)) {
            flip();
        }
        if (isGrounded) {
            if (xMove != 0) {
                PlayerManager.instance.changePlayerSate(PlayerState.Running);
            } else if (xMove == 0) {
                PlayerManager.instance.changePlayerSate(PlayerState.Idle);
            }
        }
    }

    private void verticalMovement() {
        if (isGrounded) {
            return;
        }
        if (rb2D.velocity.y >= 0.1f) {
            PlayerManager.instance.changePlayerSate(PlayerState.Jump);
        } else if (rb2D.velocity.y < -0.1f) {
            PlayerManager.instance.changePlayerSate(PlayerState.JumpFall);
        }
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Jump")) {
            jump();
        }

        if (Input.GetKeyDown(KeyCode.F)) {
            if(PlayerManager.instance.interactableObject != null) {
                ItemBehaviour handleBehaviour = PlayerManager.instance.interactableObject.GetComponent<ItemBehaviour>();

                if(handleBehaviour != null) {
                    if(!handleBehaviour.requiresKey) {
                        handleBehaviour.onActivate.Invoke();
                    } else {
                        if (PlayerManager.instance.hasKey) {
                            handleBehaviour.onActivate.Invoke();
                        }
                    }
                }
            }
        }
    }

    private void jump() {
        if (!isGrounded) {
            return;
        }
        rb2D.velocity = new Vector2(rb2D.velocity.x, jumpForce);
    }

    private void flip() {
        transform.Rotate(0, 180, 0);
        isFacingRight = !isFacingRight;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(footPosition.position, footRadious);
    }
}
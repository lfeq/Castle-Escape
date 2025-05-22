using System.Collections;
using System.Collections.Generic;
using Application_Manager; // Assuming ApplicationManager is in this namespace
using Application_Manager.States; // Assuming BaseState is in this namespace
using UnityEngine;

/// <summary>
/// Manages the spawning of enemy projectiles (fireballs) using a simple object pool.
/// Spawning behavior is linked to the game being in a specific "playing" state.
/// </summary>
public class EnemyManager : MonoBehaviour {

    /// <summary>
    /// The prefab for the enemy projectile (fireball) to be spawned.
    /// Must be assigned in the Inspector.
    /// </summary>
    [Tooltip("Prefab of the fireball to be spawned. Must be assigned.")]
    [SerializeField] private GameObject fireBall;

    /// <summary>
    /// The initial position where fireballs will be spawned.
    /// </summary>
    [Tooltip("The world position where fireballs will be spawned.")]
    [SerializeField] private Vector2 startPos;

    /// <summary>
    /// The maximum number of fireballs allowed in the object pool.
    /// Once this limit is reached by instantiation, the system will reuse existing fireballs.
    /// </summary>
    [Tooltip("Maximum number of fireballs in the object pool.")]
    [SerializeField] private int objectPoolLimit = 5;

    /// <summary>
    /// The time interval (in seconds) between spawning attempts.
    /// </summary>
    [Tooltip("Time in seconds between each spawn attempt.")]
    [SerializeField] private float spawnTimeInSeconds = 3f;

    /// <summary>
    /// Reference to the game state that represents active gameplay.
    /// Spawning will only occur when the ApplicationManager is in this state. Must be assigned.
    /// </summary>
    [Header("Game States")]
    [Tooltip("The game state during which enemies should be spawned. Must be assigned.")]
    [SerializeField] private BaseState playingState;
    
    private Queue<GameObject> m_fireBallPool; // Queue to hold the pooled fireball GameObjects.
    private bool m_canInstantiate = true; // Flag to control whether new fireballs can be instantiated or if pooling should be enforced.

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes the fireball pool and starts the spawning coroutine.
    /// </summary>
    private void Start() {
        // Critical dependency check for the fireball prefab.
        if (fireBall == null) {
            Debug.LogError("EnemyManager: fireBall prefab is not assigned in the Inspector. Disabling manager.", this);
            enabled = false; // Disable this component if the prefab is missing.
            return;
        }

        // Critical dependency check for the playing state.
        if (playingState == null) {
            Debug.LogError("EnemyManager: playingState is not assigned in the Inspector. Disabling manager.", this);
            enabled = false; // Disable this component if the state is missing.
            return;
        }

        m_fireBallPool = new Queue<GameObject>();
        // Start the coroutine that handles the spawning logic.
        StartCoroutine(spawning());
    }
    
    /// <summary>
    /// Coroutine that manages the spawning of fireballs.
    /// It runs while the game is in the 'playingState', spawning or reactivating fireballs at intervals.
    /// </summary>
    // TODO: Update this methods to improved game manager (Original TODO)
    private IEnumerator spawning() {
        // Continuously attempt to spawn while the game is in the specified playing state.
        // And ensure ApplicationManager.Instance is available.
        while (ApplicationManager.Instance != null && ApplicationManager.Instance.GetCurrentState() == playingState) {
            yield return new WaitForSeconds(spawnTimeInSeconds); // Wait for the defined spawn interval.

            if (m_canInstantiate) {
                // If allowed, instantiate a new fireball and add it to the pool.
                GameObject newFireball = Instantiate(fireBall, startPos, Quaternion.identity);
                m_fireBallPool.Enqueue(newFireball);
                // If the pool limit is reached, stop instantiating new ones.
                if (m_fireBallPool.Count >= objectPoolLimit) {
                    m_canInstantiate = false;
                }
            }
            else { // Reuse existing fireballs from the pool.
                if (m_fireBallPool.Count > 0) { // Check if there are objects to reuse.
                    GameObject tempGo = m_fireBallPool.Dequeue();
                    if (tempGo != null) { // Check if the dequeued object hasn't been destroyed.
                        tempGo.transform.position = startPos; // Reset position.
                        tempGo.SetActive(true); // Reactivate the GameObject.
                        EnemyBehaviour enemyBehaviour = tempGo.GetComponent<EnemyBehaviour>();
                        if (enemyBehaviour != null) {
                            enemyBehaviour.resetDirection(); // Reset its direction/AI.
                        } else {
                            Debug.LogWarning("EnemyManager: Reused fireball is missing EnemyBehaviour component.", tempGo);
                        }
                        m_fireBallPool.Enqueue(tempGo); // Add it back to the end of the queue.
                    } else {
                        // If a null object was dequeued (e.g., destroyed externally),
                        // it's safest to allow instantiation again temporarily to repopulate or just log it.
                        // For simplicity here, we'll just log it. A more robust pool might try to instantiate a new one.
                        Debug.LogWarning("EnemyManager: Dequeued a null fireball from the pool. It might have been destroyed externally.", this);
                        // Potentially, we could try to remove all nulls from the pool here or allow instantiation.
                        // For now, this might lead to pool size effectively shrinking if objects are destroyed.
                    }
                } else {
                     Debug.LogWarning("EnemyManager: Attempted to reuse fireball, but pool is empty and instantiation is off. This shouldn't normally happen if objectPoolLimit > 0.", this);
                }
            }
        }
        Debug.Log("EnemyManager: Spawning coroutine stopped. Game is no longer in playing state or ApplicationManager is unavailable.", this);
    }
}
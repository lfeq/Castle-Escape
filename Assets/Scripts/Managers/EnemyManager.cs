using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    [SerializeField] private GameObject fireBall;
    [SerializeField] private Vector2 startPos;
    [SerializeField] private int objectPoolLimit = 5;
    [SerializeField] private float spawnTimeInSeconds = 3f;

    private Queue<GameObject> m_pipePool;
    private bool m_canInstantiate = true;

    private void Start() {
        StartCoroutine(spawning());
        m_pipePool = new Queue<GameObject>();
    }

    private IEnumerator spawning() {
        while (GameManager.s_instance.getGameState() != GameState.GameOver) {
            yield return new WaitForSeconds(spawnTimeInSeconds);
            if (GameManager.s_instance.getGameState() != GameState.Playing) {
                continue;
            }
            if (m_canInstantiate) {
                m_pipePool.Enqueue(Instantiate(fireBall, startPos, Quaternion.identity));
                if (m_pipePool.Count > objectPoolLimit) {
                    m_canInstantiate = false;
                }
            }
            else {
                GameObject tempGo = m_pipePool.Dequeue();
                tempGo.transform.position = startPos;
                tempGo.SetActive(true);
                m_pipePool.Enqueue(tempGo);
            }
        }
    }
}
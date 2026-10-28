using UnityEngine;

public class ChangeState : MonoBehaviour
{


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (GameObject enemy in enemies)
            {
                if (enemy.TryGetComponent<StartState>(out StartState startState))
                {
                    startState.ChangeStateAnim();
                }
            }
        }
    }
}

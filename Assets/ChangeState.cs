using UnityEngine;

public class ChangeState : MonoBehaviour
{

    bool active;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !active)
        {
            
            active = true;

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

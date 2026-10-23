using Fusion;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public EnemyBehavior enemyBehavior;

    public NetworkObject player;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player in vision");
            
            PlayerControler playerC = collision.gameObject.GetComponent<PlayerControler>();
            
            if(enemyBehavior.Target == null && !playerC.IsHidden)
            {
                player = collision.gameObject.GetComponent<NetworkObject>();
                enemyBehavior.SetTarget(player);
            }
        }
    }
}

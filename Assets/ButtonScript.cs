using UnityEngine;

public class ButtonScript : MonoBehaviour
{

    public Lever lever;

    public Animator anim;
    
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Object") || collision.CompareTag("Enemy"))
        {
            anim.SetBool("Push", true);
            if(lever != null)
            {
                lever.SwitchLever();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Object") || collision.CompareTag("Enemy"))
        {
            anim.SetBool("Push", false);
        }
    }
}

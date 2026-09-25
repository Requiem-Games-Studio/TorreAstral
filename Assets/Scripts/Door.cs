using UnityEngine;

public class Door : MonoBehaviour
{

    [Range(0f, 3f)]
    public int dificult;

    public bool isLocking;
    bool open;
    public Animator anim;

    

    public void InteractingDoor()
    {
        if (isLocking)
        {
            isLocking = false;
        }

        if (open)
        {
            open = false;
        }else
        {
            open = true;           
        }
        anim.SetBool("Open", open);
    }
}

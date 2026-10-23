using UnityEngine;

public class Mechanism : MonoBehaviour
{
    public Lever lever;


    public void UseMechanism()
    {
        if(lever != null)
        {
            lever.SwitchLever();
        }
    }
}

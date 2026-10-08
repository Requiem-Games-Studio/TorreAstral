using Fusion;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class MirrorPortal : MonoBehaviour
{    
    public Animator anim;
    bool open;
    
    public void CheckMirror()
    {
        open = !open;

        anim.SetBool("Open", open);

    }
}

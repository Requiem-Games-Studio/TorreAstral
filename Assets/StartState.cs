using UnityEngine;

public class StartState : MonoBehaviour
{
    public Animator animator;
    public string startAnim;
    public string boolAnim;



    private void Start()
    {
         animator.Play(startAnim);
    }


    public void ChangeStateAnim()
    {
        animator.SetBool(boolAnim, true);
    }


}

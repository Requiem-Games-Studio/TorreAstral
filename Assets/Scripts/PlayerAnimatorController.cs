using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    public PlayerStats playerStats;
    public PlayerControler playerControler;

    public void SetBlock()
    {
        playerStats.SetBlock(false);
    }

    public void SetPerfectBlock()
    {
        playerStats.SetBlock(true);
    }

    public void StopBlock()
    {
        playerStats.StopBlock();    
    }

    public void PlayAnimationP(string animation)
    {
        playerControler.animatorP.Play(animation);
        playerControler.StopVelocity();
    }
}

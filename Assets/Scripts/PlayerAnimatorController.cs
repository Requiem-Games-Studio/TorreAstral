using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    public PlayerStats playerStats;
    public PlayerControler playerControler;

    public HandScript hand;

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

    public void StopVelocityByAnimation()
    {
        playerControler.StopVelocity();
    }

    //Hand controller
    public void ActiveHand()
    {
        hand.ActiveHand(true);
    }
    public void DesactiveHand()
    {
        hand.ActiveHand(false);
    }

    public void ActiveHand2()
    {
        hand.ActiveHandDown(true);
    }
    public void DesactiveHand2()
    {
        hand.ActiveHandDown(false);
    }

    public void HandTrowObject()
    {
        hand.TrowObject();
    }

    public void HandAddOrDrop()
    {
        hand.AddOrDrop();
    }
}

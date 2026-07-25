using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Animator anim;
    public SpriteRenderer weaponSprite;


    public void SetNewWeapon(GameObject newWeapon)
    {
        anim = newWeapon.GetComponent<Animator>();

    }
}

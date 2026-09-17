using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Animator anim;
    public SpriteRenderer weaponSprite;

    GameObject weapon;

    public void SetNewWeapon(GameObject newWeapon)
    {
        weapon = newWeapon;
        anim = weapon.GetComponent<Animator>();
        weaponSprite = weapon.GetComponent<SpriteRenderer>();
    }

    public void UnequipWeapon()
    {
        anim = null;
        weaponSprite = null;
        Destroy(weapon);
    }
}

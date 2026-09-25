using UnityEngine;
using static DamageSystem;
//using UnityEditor.Animations;

public class ArmorManager : MonoBehaviour
{
    public GameObject player_h, player_c,player_a, player_l;
    public Animator[] animators;
    public RuntimeAnimatorController[] defaultController;

    public ArmorResistance defaultRes;
 
    public ArmorPiece helmet;
    public ArmorPiece chest;
    public ArmorPiece arms;
    public ArmorPiece legs;

    

    public float GetDefense(DamageType type)
    {
        float defense = 0f;

        if (helmet != null)
            defense += helmet.resistance.GetResistance(type);

        if (chest != null)
            defense += chest.resistance.GetResistance(type);

        if (arms != null)
            defense += arms.resistance.GetResistance(type);

        if (legs != null)
            defense += legs.resistance.GetResistance(type);

        return defense;
    }

    public void SetNewHelment(GameObject newHelment)
    {
        animators[0].runtimeAnimatorController = newHelment.GetComponent<Animator>().runtimeAnimatorController;
        helmet.resistance = newHelment.GetComponent<ArmorPiece>().resistance;
        Destroy(newHelment);
    }
    public void SetNewChest(GameObject newChest)
    {
        animators[1].runtimeAnimatorController = newChest.GetComponent<Animator>().runtimeAnimatorController;
        chest.resistance = newChest.GetComponent<ArmorPiece>().resistance;
        Destroy(newChest);
    }
    public void SetNewArms(GameObject newArms)
    {
        animators[2].runtimeAnimatorController = newArms.GetComponent<Animator>().runtimeAnimatorController;
        arms.resistance = newArms.GetComponent<ArmorPiece>().resistance;
        Destroy(newArms);
    }
    public void SetNewBoots(GameObject newBoots)
    {
        animators[3].runtimeAnimatorController = newBoots.GetComponent<Animator>().runtimeAnimatorController;
        legs.resistance = newBoots.GetComponent<ArmorPiece>().resistance; 
        Destroy(newBoots);
    }



    public void UnequipHelment()
    {
        animators[0].runtimeAnimatorController = defaultController[0];
        helmet.resistance = defaultRes;
    }
    public void UnequipChest()
    {
        animators[1].runtimeAnimatorController = defaultController[1];
        chest.resistance = defaultRes;
    }
    public void UnequipArms()
    {
        animators[2].runtimeAnimatorController = defaultController[2];
        arms.resistance = defaultRes;
    }
    public void UnequipBoots()
    {
        animators[3].runtimeAnimatorController = defaultController[3];
        legs.resistance = defaultRes;
    }

}

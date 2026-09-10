using UnityEngine;
using static DamageSystem;

public class ArmorManager : MonoBehaviour
{
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
}

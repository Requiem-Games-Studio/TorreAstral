using UnityEngine;

public class DamageSystem
{
    public enum DamageType
    {
        Contundente,
        Perforante,
        Cortante,
        Fuego,
        Escarcha
    }

    [System.Serializable]
    public class ArmorResistance
    {
        [Range(0f, 100f)]
        public float contundente;

        [Range(0f, 100f)]
        public float perforante;

        [Range(0f, 100f)]
        public float cortante;

        [Range(0f, 100f)]
        public float fuego;

        [Range(0f, 100f)]
        public float escarcha;

        public float GetResistance(DamageType type)
        {
            return type switch
            {
                DamageType.Contundente => contundente,
                DamageType.Perforante => perforante,
                DamageType.Cortante => cortante,
                DamageType.Fuego => fuego,
                DamageType.Escarcha => escarcha,

                _ => 0f
            };
        }
    }

    [System.Serializable]
    public struct DamageData
    {
        public float damage;
        public DamageType type;

        public DamageData(float damage, DamageType type)
        {
            this.damage = damage;
            this.type = type;
        }
    }

    public static class DamageCalculator
    {
        public static float CalculateDamage(float damage, float defense)
        {
            return damage * (100f / (100f + defense));
        }
    }

}

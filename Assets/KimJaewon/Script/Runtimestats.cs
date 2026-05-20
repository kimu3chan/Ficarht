using UnityEngine;

[System.Serializable]
public class RuntimeStats
{
    public float maxHealth;
    public float currentHealth;
    public float stamina;
    public float power;
    public float defense;
    public float intelligence;

    public RuntimeStats(CharacterCardStats baseStats)
    {
        maxHealth = baseStats.health;
        currentHealth = baseStats.health;
        stamina = baseStats.stamina;
        power = baseStats.power;
        defense = baseStats.defense;
        intelligence = baseStats.intelligence;
    }

    public void ApplyBuff(BuffEffect effect)
    {
        currentHealth += effect.healthMod;
        maxHealth += effect.healthMod;
        stamina += effect.staminaMod;
        power += effect.powerMod;
        defense += effect.defenseMod;
        intelligence += effect.intelligenceMod;

        currentHealth = Mathf.Max(currentHealth, 1f);
        maxHealth = Mathf.Max(maxHealth, 1f);
        stamina = Mathf.Max(stamina, 0f);
        power = Mathf.Max(power, 0f);
        defense = Mathf.Max(defense, 0f);
        intelligence = Mathf.Max(intelligence, 0f);
    }

    public override string ToString()
    {
        return $"HP:{currentHealth}/{maxHealth} STM:{stamina} PWR:{power} DEF:{defense} INT:{intelligence}";
    }
}
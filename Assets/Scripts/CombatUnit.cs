using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    Player,
    Ally,
    Enemy
}

public enum ClassType
{
    Warrior,
    Mage,
    Hunter
}

[System.Serializable]
public class CombatUnit
{
    public string Name;
    public UnitType UnitCategory;
    public ClassType UnitClass;
    public int Health;
    public int Attack;
    public int Defense;
    public float Speed;
    public List<CombatAction> Actions;

    public CombatUnit(string name, UnitType unitCategory, ClassType unitClass, int health, int attack, List<CombatAction> actions)
    {
        Name = name;
        UnitCategory = unitCategory;
        UnitClass = unitClass;
        Health = health;
        Attack = attack;
        Actions = actions;
    }
}

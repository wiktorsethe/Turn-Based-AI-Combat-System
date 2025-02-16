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

public class CombatUnit
{
    public string Name { get; private set; }
    public UnitType UnitCategory { get; private set; }
    public ClassType UnitClass { get; private set; }
    public int Health { get; private set; }
    public int Attack { get; private set; }
    public int Defense { get; private set; }
    public int Speed { get; private set; }
    
    public CombatUnit(string name, UnitType unitCategory, ClassType unitClass, int health, int attack)
    {
        Name = name;
        UnitCategory = unitCategory;
        UnitClass = unitClass;
        Health = health;
        Attack = attack;
    }
}

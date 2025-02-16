using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Start()
    {
        CombatUnit[] units = new[]
        {
            new CombatUnit("Player", UnitType.Player, ClassType.Warrior, 100, 10),
            new CombatUnit("Ally1", UnitType.Ally, ClassType.Warrior, 100, 10),
            new CombatUnit("Ally2", UnitType.Ally, ClassType.Mage, 100, 10),
            new CombatUnit("Enemy1", UnitType.Enemy, ClassType.Warrior, 100, 10),
            new CombatUnit("Enemy2", UnitType.Enemy, ClassType.Hunter, 100, 10)
        };
        CombatSystem.CreateBattle(units);
    }
}

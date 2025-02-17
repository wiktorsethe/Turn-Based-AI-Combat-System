using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Test : MonoBehaviour
{
    public CombatUnit[] combatUnits;

    private void Start()
    {
        // if (combatActionsDatabase == null || combatActionsDatabase.allActions.Count == 0)
        // {
        //     Debug.LogError("Brak bazy akcji! Upewnij się, że CombatActionsDatabase zawiera akcje.");
        //     return;
        // }
        //
        // // Pobieramy akcje dla poszczególnych klas
        // List<CombatAction> warriorActions = combatActionsDatabase.allActions.FindAll(a => a.type == CombatAction.TypeOfAction.ATTACK);
        // List<CombatAction> mageActions = combatActionsDatabase.allActions.FindAll(a => a.type == CombatAction.TypeOfAction.SUPPORT);
        // List<CombatAction> hunterActions = combatActionsDatabase.allActions.FindAll(a => a.type == CombatAction.TypeOfAction.ATTACK);
        //
        // CombatUnit[] units = new[]
        // {
        //     new CombatUnit("Player", UnitType.Player, ClassType.Warrior, 100, 10, warriorActions),
        //     new CombatUnit("Ally1", UnitType.Ally, ClassType.Warrior, 100, 10, warriorActions),
        //     new CombatUnit("Ally2", UnitType.Ally, ClassType.Mage, 100, 10, mageActions),
        //     new CombatUnit("Enemy1", UnitType.Enemy, ClassType.Warrior, 100, 10, warriorActions),
        //     new CombatUnit("Enemy2", UnitType.Enemy, ClassType.Hunter, 100, 10, hunterActions)
        // };

        CombatSystem.CreateBattle(combatUnits);
    }
}
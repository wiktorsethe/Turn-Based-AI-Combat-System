using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    private static GameObject _combatPrefab;
    private static Combat _combat;
    
    public static void CreateBattle(CombatUnit[] combatUnits)
    {
        _combatPrefab = Resources.Load<GameObject>("Combat");
        if (_combat == null) 
        {
            _combat = Instantiate(_combatPrefab).GetComponent<Combat>();
            _combat.gameObject.GetComponent<Canvas>().worldCamera = Camera.main;
        }
        else 
        {
            _combat.gameObject.SetActive(true);
        }
        
        List<CombatUnit> playerTeam = new List<CombatUnit>();
        List<CombatUnit> enemyTeam = new List<CombatUnit>();

        foreach (CombatUnit combatUnit in combatUnits)
        {
            if (combatUnit.UnitCategory == UnitType.Enemy)
            {
                enemyTeam.Add(combatUnit);
            }
            else
            {
                playerTeam.Add(combatUnit);
            }
        }
        
        _combat.SpawnUnits(playerTeam, enemyTeam);
    }
}

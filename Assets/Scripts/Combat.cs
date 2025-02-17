using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    [SerializeField] private CombatHUD combatHUD;

    [SerializeField] private GameObject unitHUDPrefab;
    private GameObject _unitHUD;

    private const float XSpacing = 1.5f;
    private const float YSpacing = 1.5f;

    private readonly Vector2 _enemyTeamPosition = new Vector2(0, 1);
    private readonly Vector2 _allyTeamPosition = new Vector2(0, -1);
    
    public void SpawnUnits(List<CombatUnit> playerTeam, List<CombatUnit> enemyTeam)
    {
        SpawnTeam(playerTeam, _allyTeamPosition, true);
        SpawnTeam(enemyTeam, _enemyTeamPosition, false);
    }
    
    private void SpawnTeam(List<CombatUnit> team, Vector3 startPos, bool isPlayerSide)
    {
        // Podział jednostek na klasy
        List<CombatUnit> warriors = new List<CombatUnit>();
        List<CombatUnit> mages = new List<CombatUnit>();
        List<CombatUnit> hunters = new List<CombatUnit>();

        foreach (var unit in team)
        {
            switch (unit.UnitClass)
            {
                case ClassType.Warrior: warriors.Add(unit); break;
                case ClassType.Mage: mages.Add(unit); break;
                case ClassType.Hunter: hunters.Add(unit); break;
            }
        }

        // Rozstawianie jednostek na liniach
        SpawnRow(warriors, startPos, isPlayerSide, 0);
        SpawnRow(mages, startPos, isPlayerSide, YSpacing);
        SpawnRow(hunters, startPos, isPlayerSide, YSpacing * 2);
    }
    
    private void SpawnRow(List<CombatUnit> units, Vector3 startPos, bool isPlayerSide, float yOffset)
    {
        if (units.Count == 0) return;

        float totalWidth = (units.Count - 1) * XSpacing;
        float startX = isPlayerSide ? startPos.x - totalWidth / 2 : startPos.x + totalWidth / 2;

        for (int i = 0; i < units.Count; i++)
        {

            float xOffset = i * XSpacing;
            float xPos = isPlayerSide ? startX + xOffset : startX - xOffset;
            
            float yPos = isPlayerSide ? startPos.y - yOffset : startPos.y + yOffset;
            
            Vector3 spawnPos = new Vector3(xPos, yPos, 0);

            _unitHUD = Instantiate(unitHUDPrefab, spawnPos, Quaternion.identity);
            _unitHUD.GetComponent<CombatUnitHUD>().Initialize(units[i]); // Przypisanie HUD
            if(units[i].UnitCategory == UnitType.Player) combatHUD.SetPlayerUnit(units[i]);
        }
    }
    
    public void ActionUsage(CombatAction action, CombatUnit unit)
    {
        //TODO: dokonczyc to
        
        // if(action.type == CombatAction.TypeOfAction.ATTACK) 
        //     unit.Health -= action.attackAmount;
        // else if (action.type == CombatAction.TypeOfAction.SUPPORT)
        //     unit.Health += action.healAmount;
        // else if (action.type == CombatAction.TypeOfAction.DEFEND)
        //     // Do dokończenia, prawdopodobnie od ataku przeciwnika bedzie odejmowana połowa mocy obrony
        //     unit.Health += action.defensePower;
        //_battleSystem.SetPlayerTurn(action);
        combatHUD.HideActionsView();
    }
}

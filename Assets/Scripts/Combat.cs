using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CombatState
{
    START,
    PLAYERTURN,
    ENEMYTURN,
    WON,
    LOST
}
public class Combat : MonoBehaviour
{
    public CombatState battleState;
    public bool turnAction = false;
    //private CombatAction _playerActionUsed;
    private Dictionary<CombatAction, CombatUnit> _playerActionUsed = new Dictionary<CombatAction, CombatUnit>();

    [SerializeField] private CombatHUD combatHUD;

    [SerializeField] private GameObject unitHUDPrefab;
    private GameObject _unitHUD;

    private const float XSpacing = 1.5f;
    private const float YSpacing = 1.5f;

    private readonly Vector2 _enemyTeamPosition = new Vector2(1, 1);
    private readonly Vector2 _allyTeamPosition = new Vector2(-1, 1);

    public CombatUnit playerUnit;
    
    private List<CombatUnit> _enemies = new List<CombatUnit>();
    private List<CombatUnit> _allies = new List<CombatUnit>();
    private void Start()
    {
        battleState = CombatState.START;
    }
    
    private void SetupBattle(List<CombatUnit> playerTeam, List<CombatUnit> enemyTeam)
    {
        string playerUnits = string.Join(", ", playerTeam.Select(u => u.Name));
        string enemyUnits = string.Join(", ", enemyTeam.Select(u => u.Name));

        _allies = playerTeam;
        _enemies = enemyTeam;
        
        combatHUD.AppendMessage($"The fight between {playerUnits} and {enemyUnits} begins!");
        battleState = CombatState.PLAYERTURN;
        StartCoroutine(PlayerTurn());
    }

    private void SetPlayerTurn(CombatAction action, CombatUnit unit)
    {
        turnAction = true;
        _playerActionUsed[action] = unit;
    }
    
    private IEnumerator PlayerTurn()
    {
        if (PlayerPrefs.GetInt($"{playerUnit.Name}Health") <= 0)
        {
            battleState = CombatState.LOST;
            //LostTurn();
            yield return null;
        }
        else
        {
            turnAction = false;
            Debug.Log("<color=yellow>----- Tura Gracza -----</color>");
            float elapsedTime = 0f;
            while (!turnAction && elapsedTime < 15f)
            {
                combatHUD.SetPlayerTurnTimer((int)elapsedTime);
                yield return null;
                elapsedTime += Time.deltaTime;
            }

            if (turnAction)
            {
                Debug.Log("<color=green>Tura udana!</color>");

                if (_playerActionUsed.Count > 0)
                {
                    var actionEntry = _playerActionUsed.First(); 
                    CombatAction actionUsed = actionEntry.Key;
                    CombatUnit targetUnit = actionEntry.Value;

                    if (actionUsed.type == CombatAction.TypeOfAction.ATTACK)
                    {
                        combatHUD.AppendMessage(playerUnit.Name + " used " + actionUsed.actionName +
                                                " and takes: " + actionUsed.attackAmount + " points of " +
                                                targetUnit.Name + " health.");
                    }
                    else if (actionUsed.type == CombatAction.TypeOfAction.SUPPORT)
                    {
                        combatHUD.AppendMessage(playerUnit.Name + " used " + actionUsed.actionName +
                                                " and receives: " + actionUsed.healAmount + 
                                                " points of health.");
                    }
                    else if (actionUsed.type == CombatAction.TypeOfAction.DEFEND)
                    {
                        combatHUD.AppendMessage(playerUnit.Name + " used " + actionUsed.actionName +
                                                " and receives: " + actionUsed.defensePower + 
                                                " points of shield.");
                    }

                    // Po użyciu akcji usuwamy ją ze słownika
                    _playerActionUsed.Remove(actionUsed);
                }

                battleState = CombatState.ENEMYTURN;
                SetEnemyTurn();
                yield break;
            }

        
            yield return null;
            Debug.LogWarning("<color=red>Tura zmarnowana!</color>");
            battleState = CombatState.ENEMYTURN;
            SetEnemyTurn();
        }
    }
    
    //TODO: zrobic zeby kazdy z druzuny przeciwnej atakowal
    private void SetEnemyTurn()
    {
        bool anyAlive = _enemies.Any(unit => PlayerPrefs.GetInt($"{unit.Name}Health") > 0);
    
        if (!anyAlive) 
        {
            battleState = CombatState.WON;
            //WinTurn();
            return;
        }
        
        StartCoroutine(EnemyTurn());
    }
    
    private IEnumerator EnemyTurn()
    {
        foreach (var enemy in _enemies)
        {
            if (PlayerPrefs.GetInt($"{enemy.Name}Health") <= 0)
                continue; // Jeśli przeciwnik jest martwy, pomijamy jego turę

            Debug.Log("<color=yellow>----- Tura Przeciwnika -----</color>");

            var aliveAllies = _allies.Where(unit => PlayerPrefs.GetInt($"{unit.Name}Health") > 0).ToList();

            CombatUnit target = aliveAllies[Random.Range(0, aliveAllies.Count)];
            CombatAction enemyAction = enemy.Actions[0]; // Można dodać AI wybierające akcję

            PlayerPrefs.SetInt($"{target.Name}Health", PlayerPrefs.GetInt($"{target.Name}Health") - enemyAction.attackAmount);
            combatHUD.AppendMessage($"{enemy.Name} used {enemyAction.actionName} and deals {enemyAction.attackAmount} damage to {target.Name}.");

            yield return new WaitForSeconds(1f); // Krótka przerwa między atakami
        }

        battleState = CombatState.PLAYERTURN;
        StartCoroutine(PlayerTurn());
    }
    
    public void SpawnUnits(List<CombatUnit> playerTeam, List<CombatUnit> enemyTeam)
    {
        SpawnTeam(playerTeam, _allyTeamPosition, true);
        SpawnTeam(enemyTeam, _enemyTeamPosition, false);
        SetupBattle(playerTeam, enemyTeam);
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
        SpawnRow(mages, startPos, isPlayerSide, XSpacing);
        SpawnRow(hunters, startPos, isPlayerSide, XSpacing * 2);
    }
    
    private void SpawnRow(List<CombatUnit> units, Vector3 startPos, bool isPlayerSide, float xOffset)
    {
        if (units.Count == 0) return;

        float totalHeight = (units.Count - 1) * YSpacing;
        float startY = isPlayerSide ? startPos.y - totalHeight / 2 : startPos.y + totalHeight / 2;

        for (int i = 0; i < units.Count; i++)
        {
            float yOffset = i * YSpacing;
            float yPos = isPlayerSide ? startY + yOffset : startY - yOffset;
            
            float xPos = isPlayerSide ? startPos.x - xOffset : startPos.x + xOffset;
            
            Vector3 spawnPos = new Vector3(xPos, yPos, 0);

            _unitHUD = Instantiate(unitHUDPrefab, spawnPos, Quaternion.identity);
            _unitHUD.GetComponent<CombatUnitHUD>().Initialize(units[i]);
            if (units[i].UnitCategory == UnitType.Player) playerUnit = units[i];
            
            PlayerPrefs.SetInt($"{units[i].Name}Health", units[i].Health);
        }
    }
    
    public void ActionUsage(CombatAction action, CombatUnit unit)
    {
        //TODO: dokonczyc to
        
        if(action.type == CombatAction.TypeOfAction.ATTACK) 
            PlayerPrefs.SetInt($"{unit.Name}Health", PlayerPrefs.GetInt($"{unit.Name}Health") - action.attackAmount);
        else if (action.type == CombatAction.TypeOfAction.SUPPORT)
            PlayerPrefs.SetInt($"{unit.Name}Health", PlayerPrefs.GetInt($"{unit.Name}Health") + action.healAmount);
        else if (action.type == CombatAction.TypeOfAction.DEFEND)
            // Do dokończenia, prawdopodobnie od ataku przeciwnika bedzie odejmowana połowa mocy obrony
            PlayerPrefs.SetInt($"{unit.Name}Health", PlayerPrefs.GetInt($"{unit.Name}Health") + action.defensePower);
        SetPlayerTurn(action, unit);
        combatHUD.HideActionsView();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum CombatState
{
    START,
    PLAYERTURN,
    ALLYTURN,
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
    
    public List<CombatUnit> _turnQueue = new List<CombatUnit>();
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
        
        _turnQueue = GenerateTurnOrder();

        int x = 0;
        foreach (CombatUnit unit in _turnQueue)
        {
            x++;
            Debug.Log(x + ". " + unit.Name);
        }
        
        combatHUD.AppendMessage($"The fight between {playerUnits} and {enemyUnits} begins!");
        battleState = CombatState.PLAYERTURN;
        StartCoroutine(PlayerTurn());
    }
    
    private List<CombatUnit> GenerateTurnOrder(int cycleSize = 10)
    {
        List<CombatUnit> allUnits = _allies.Concat(_enemies).ToList();

        List<CombatUnit> turnOrder = new List<CombatUnit>();
        
        // Znajdź największą wartość Speed, aby normalizować do niej
        float maxSpeed = allUnits.Max(u => u.Speed);
        
        // Tworzymy listę kolejek na podstawie proporcji Speed
        Dictionary<CombatUnit, int> unitTurns = new Dictionary<CombatUnit, int>();
        
        foreach (var unit in allUnits)
        {
            int turns = (int)Math.Round((unit.Speed / maxSpeed) * cycleSize);
            unitTurns[unit] = Math.Max(turns, 1); // Zapewniamy, że każda jednostka ma co najmniej 1 turę
        }
        
        // Dodawanie jednostek do kolejki tury
        foreach (var unit in unitTurns)
        {
            for (int i = 0; i < unit.Value; i++)
            {
                turnOrder.Add(unit.Key);
            }
        }
        
        // Sortowanie losowe, aby uniknąć powtarzalnych wzorców
        turnOrder = turnOrder.OrderBy(x => Guid.NewGuid()).ToList();
        
        return turnOrder;
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
            LostTurn();
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
                                                targetUnit.Name + " health.", "lime");
                    }
                    else if (actionUsed.type == CombatAction.TypeOfAction.SUPPORT)
                    {
                        combatHUD.AppendMessage(playerUnit.Name + " used " + actionUsed.actionName +
                                                " and receives: " + actionUsed.healAmount + 
                                                " points of health.", "lime");
                    }
                    else if (actionUsed.type == CombatAction.TypeOfAction.DEFEND)
                    {
                        combatHUD.AppendMessage(playerUnit.Name + " used " + actionUsed.actionName +
                                                " and receives: " + actionUsed.defensePower + 
                                                " points of shield.", "lime");
                    }

                    // Po użyciu akcji usuwamy ją ze słownika
                    _playerActionUsed.Remove(actionUsed);
                }

                yield return new WaitForSeconds(1f);
                
                battleState = CombatState.ALLYTURN;
                SetAllyTurn();
                yield break;
            }

        
            yield return null;
            Debug.LogWarning("<color=red>Tura zmarnowana!</color>");
            battleState = CombatState.ALLYTURN;
            SetAllyTurn();
        }
    }

    private void SetAllyTurn()
    {
        StartCoroutine(AllyTurn());
    }

    private IEnumerator AllyTurn()
    {
        foreach (var ally in _allies)
        {
            if (PlayerPrefs.GetInt($"{ally.Name}Health") <= 0 || ally.UnitCategory == UnitType.Player)
                continue; // Jeśli sojusznik jest martwy lub jest graczem, pomijamy jego turę

            Debug.Log("<color=yellow>----- Tura Sojusznika -----</color>");

            var aliveEnemies = _enemies.Where(enemy => PlayerPrefs.GetInt($"{enemy.Name}Health") > 0 && enemy.UnitCategory != UnitType.Player).ToList();

            if (aliveEnemies.Count == 0)
                continue; // Jeśli nie ma dostępnych celów, pomijamy turę sojusznika

            CombatUnit target = aliveEnemies[Random.Range(0, aliveEnemies.Count)];
            CombatAction allyAction = ally.Actions[0]; // Można dodać AI wybierające akcję

            PlayerPrefs.SetInt($"{target.Name}Health", PlayerPrefs.GetInt($"{target.Name}Health") - allyAction.attackAmount);
            combatHUD.AppendMessage($"{ally.Name} used {allyAction.actionName} and deals {allyAction.attackAmount} damage to {target.Name}.", "yellow");

            yield return new WaitForSeconds(1f); // Krótka przerwa między atakami
        }

        battleState = CombatState.ENEMYTURN;
        SetEnemyTurn();
    }

    
    private void SetEnemyTurn()
    {
        bool anyAlive = _enemies.Any(unit => PlayerPrefs.GetInt($"{unit.Name}Health") > 0);
    
        if (!anyAlive) 
        {
            battleState = CombatState.WON;
            WinTurn();
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
            combatHUD.AppendMessage($"{enemy.Name} used {enemyAction.actionName} and deals {enemyAction.attackAmount} damage to {target.Name}.", "red");

            yield return new WaitForSeconds(1f); // Krótka przerwa między atakami
        }

        battleState = CombatState.PLAYERTURN;
        StartCoroutine(PlayerTurn());
    }

    private void WinTurn()
    {
        string playerUnits = string.Join(", ", _allies.Select(u => u.Name));
        
        combatHUD.AppendMessage($"{playerUnits} won the battle!");
    }
    
    private void LostTurn()
    {
        string enemyUnits = string.Join(", ", _enemies.Select(u => u.Name));
        
        combatHUD.AppendMessage($"{enemyUnits} won the battle!");
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

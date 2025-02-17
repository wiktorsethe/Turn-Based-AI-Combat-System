using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Combat Action Database", menuName = "Combat Action Database")]
public class CombatActionsDatabase : ScriptableObject
{
    public List<CombatAction> allActions = new List<CombatAction>();
    
    public void AddAction(CombatAction action)
    {
        if (!allActions.Contains(action))
        {
            allActions.Add(action);
        }
    }

}



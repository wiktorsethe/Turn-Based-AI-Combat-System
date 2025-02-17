using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Combat Action", menuName = "New Combat Action")]
public class CombatAction : ScriptableObject
{
    public enum TypeOfAction
    {
        ATTACK,
        SUPPORT,
        DEFEND
    };
    public TypeOfAction type;

    public string actionName;
    public int healAmount;
    public int attackAmount;
    public int defensePower;
}

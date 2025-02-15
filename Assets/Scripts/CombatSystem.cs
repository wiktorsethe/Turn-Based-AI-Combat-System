using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    private static GameObject _combatPrefab;
    private static Combat _combat;
    
    public static void CreateBattle()
    {
        Debug.Log("Creating Battle");
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
    }
}

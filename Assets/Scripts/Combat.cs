using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    [SerializeField] private CombatHUD combatHUD;

    [SerializeField] private GameObject unitPrefab;
    private GameObject _unit;

    public void Start()
    {
        SpawnUnits();
    }

    private void SpawnUnits()
    {
        _unit = Instantiate(unitPrefab);
    }
}

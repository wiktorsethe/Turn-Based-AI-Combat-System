using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUnitHUD : MonoBehaviour
{
    private CombatHUD _combatHUD;
    [SerializeField] private Canvas canvas;
    
    public CombatUnit AssignedUnit { get; private set; }
    private void Start()
    {
        _combatHUD = FindObjectOfType(typeof(CombatHUD)) as CombatHUD;
        canvas.worldCamera = Camera.main;
    }
    
    public void Initialize(CombatUnit unit) 
    {
        AssignedUnit = unit; // Przypisanie jednostki do HUD
    }
    
    private void OnMouseEnter()
    {
        _combatHUD.ShowUnitView(this);
    }

    private void OnMouseExit()
    {
        _combatHUD.HideUnitView();
    }
    
    private void OnMouseDown()
    {
        if(_combatHUD.GetActionsViewActivity()) 
            _combatHUD.HideUnitView();
        _combatHUD.ShowActionsView(this, AssignedUnit);
        GetComponent<Collider2D>().enabled = false;
    }
}

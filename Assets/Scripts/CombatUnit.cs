using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUnit : MonoBehaviour
{
    private CombatHUD _combatHUD;
    [SerializeField] private Canvas canvas;
    private void Start()
    {
        _combatHUD = FindObjectOfType(typeof(CombatHUD)) as CombatHUD;
        canvas.worldCamera = Camera.main;
    }
    private void OnMouseEnter()
    {
        Debug.Log("Mouse Enter");
        _combatHUD.ShowUnitView(this);
    }

    private void OnMouseExit()
    {
        Debug.Log("Mouse Exit");
        _combatHUD.HideUnitView();
    }
    
    private void OnMouseDown()
    {
        if(_combatHUD.GetMovesViewActivity()) 
            _combatHUD.HideUnitView();
        _combatHUD.ShowMovesView(this);
        GetComponent<BoxCollider2D>().enabled = false;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatHUD : MonoBehaviour
{
    [SerializeField] private GameObject unitViewPrefab;
    [SerializeField] private GameObject movesViewPrefab;
    private GameObject _unitView;
    private GameObject _movesView;
    private CombatUnit _unit;
    private bool _isMovesViewActive;

    private void Start()
    {
        _isMovesViewActive = false;
    }

    public void ShowUnitView(CombatUnit unit)
    {
        if(!_unitView)
        {
            if (Camera.main != null)
            {
                _unitView = Instantiate(unitViewPrefab, unit.transform.Find("Canvas").transform);
            }
        }
        else
        {
            _unitView.SetActive(true);
        }

        if (Camera.main != null)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 offset = new Vector2(1f, -0.3f);
            _unitView.transform.position = mousePosition + offset;
        }


        // _unitView.transform.Find("UnitNameText").GetComponent<TMP_Text>().text =
        //     unit.unitName;
        // _unitView.transform.Find("UnitHPText").GetComponent<TMP_Text>().text =
        //     unit.currentHealth + "/" + unit.maxHealth;
    }
    
    public void HideUnitView()
    {
        _unitView.SetActive(false);
    }

    public void ShowMovesView(CombatUnit unit)
    {
        if(_unitView) _unitView.SetActive(false);
        
        if(!_movesView)
        {
            if (Camera.main != null)
            {
                _movesView = Instantiate(movesViewPrefab, unit.transform.Find("Canvas").transform);
            }
        }
        else
        {
            _movesView.SetActive(true);
        }

        if (Camera.main != null)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _movesView.transform.position = mousePosition;
        }

        _unit = unit;
        _isMovesViewActive = true;
    }

    public void HideMovesView()
    {
        if (_movesView.activeSelf)
        {
            for (int i=0; i<_movesView.transform.childCount; i++)
            {
                GameObject child = _movesView.transform.GetChild(i).gameObject;
                Destroy(child);
            }

            _isMovesViewActive = false;
            _movesView.SetActive(false);
            _unit.transform.GetComponent<Collider2D>().enabled = true;
        }
    }
    
    public bool GetMovesViewActivity()
    {
        return _isMovesViewActive;
    }
}

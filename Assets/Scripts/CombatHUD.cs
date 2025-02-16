using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CombatHUD : MonoBehaviour
{
    [SerializeField] private GameObject unitViewPrefab;
    [SerializeField] private GameObject actionsViewPrefab;
    [SerializeField] private GameObject actionButtonPrefab;
    private GameObject _unitView;
    private GameObject _actionsView;
    private CombatUnitHUD _unitHUD;
    private bool _isMovesViewActive;

    private void Start()
    {
        _isMovesViewActive = false;
    }

    public void ShowUnitView(CombatUnitHUD unit)
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


        _unitView.transform.Find("UnitNameText").GetComponent<TMP_Text>().text =
            unit.AssignedUnit.Name;
        _unitView.transform.Find("UnitHPText").GetComponent<TMP_Text>().text =
            unit.AssignedUnit.Health + "/" + unit.AssignedUnit.Health; //TODO: zalatwic sprawe currentHealth
    }
    
    public void HideUnitView()
    {
        _unitView.SetActive(false);
    }

    public void ShowActionsView(CombatUnitHUD unit)
    {
        if(_unitView) _unitView.SetActive(false);
        
        if(!_actionsView)
        {
            if (Camera.main != null)
            {
                _actionsView = Instantiate(actionsViewPrefab, unit.transform.Find("Canvas").transform);
            }
        }
        else
        {
            _actionsView.SetActive(true);
        }

        if (Camera.main != null)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _actionsView.transform.position = mousePosition;
        }

        _unitHUD = unit;
        _isMovesViewActive = true;

        AddButton();
        AddButton();
        
        UpdateActionsViewSize();
    }

    public void HideActionsView()
    {
        if (_actionsView.activeSelf)
        {
            for (int i=0; i<_actionsView.transform.childCount; i++)
            {
                GameObject child = _actionsView.transform.GetChild(i).gameObject;
                Destroy(child);
            }

            _isMovesViewActive = false;
            _actionsView.SetActive(false);
            _unitHUD.transform.GetComponent<Collider2D>().enabled = true;
        }
    }
    
    public bool GetMovesViewActivity()
    {
        return _isMovesViewActive;
    }

    private void AddButton()
    {
        GameObject actionButton = Instantiate(actionButtonPrefab, _actionsView.transform);
        actionButton.GetComponent<Button>().onClick.AddListener(HideActionsView);
        //combatButton.transform.GetComponentInChildren<TMP_Text>().text = battleAction.actionName;
        /*combatButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            action?.Invoke(battleAction);
        });*/
    }
    
    private void UpdateActionsViewSize()
    {
        if (!_actionsView) return;

        GridLayoutGroup gridLayout = _actionsView.GetComponent<GridLayoutGroup>();
        if (!gridLayout) return;

        int childCount = _actionsView.transform.childCount;
        float buttonHeight = 90f;
        float paddingTopBottom = gridLayout.padding.top + gridLayout.padding.bottom; // 10 + 10 = 20
        float spacingY = gridLayout.spacing.y * (childCount - 1); // 5 między elementami

        float newHeight = (childCount * buttonHeight) + paddingTopBottom + spacingY;

        RectTransform rt = _actionsView.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, newHeight);
    }
}

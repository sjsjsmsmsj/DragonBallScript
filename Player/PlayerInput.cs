using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private WarriorCardUIManager warriorCardUIManager;
    private ControllerMove controllerMove;
    private LineupManager lineupManager;
    private WarriorManager warriorManager;
    private void Start()
    {
        controllerMove = GetComponent<ControllerMove>();
        lineupManager = GetComponent<LineupManager>();
        warriorManager = GetComponent<WarriorManager>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            controllerMove.HandleMove();
        }
        //key
        if (Input.GetKeyDown(KeyCode.Q))
        {
            lineupManager.ProtectedFormation(warriorManager.GetWarriorList(), controllerMove.GetNewPos());
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            lineupManager.AttackFormation(warriorManager.GetWarriorList(), controllerMove.GetNewPos());
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            lineupManager.MassacreFormation(warriorManager.GetWarriorList(), controllerMove.GetNewPos());
        }
        else if (Input.GetKeyDown(KeyCode.Tab) && warriorCardUIManager != null)
        {
            warriorCardUIManager.OpenWarriorCardListUI();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && warriorCardUIManager != null)
        {
            warriorCardUIManager.CloseWarriorCardListUI();
        }

    }

}
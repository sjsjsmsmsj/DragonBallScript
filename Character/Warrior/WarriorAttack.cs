using UnityEngine;

public class WarriorAttack : CharacterAttack
{
    private WarriorManager warriorManager;
    protected override void Start()
    {
        base.Start();
        warriorManager = gameManager.warriorManager;
    }
    public override void Update()
    {
        base.Update();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        ControllerMove.PlayerMove += HandlePlayerMove;
        WarriorMove.Arrived += HandleFinishMove;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        ControllerMove.PlayerMove -= HandlePlayerMove;
        WarriorMove.Arrived -= HandleFinishMove;
    }
    public override Transform GetNearestTarget(Transform transform, float rangeAttack)
    {
        target = null;
        Collider2D[] targetCollider = Physics2D.OverlapCircleAll(transform.position, rangeAttack, targetLayer);
        float minDis = float.PositiveInfinity;
        foreach (var warrior in targetCollider)
        {
            float distance = Vector3.Distance(transform.position, warrior.transform.position);
            if (distance < minDis && !warrior.GetComponent<Character>().GetIsDie())
            {
                minDis = distance;
                target = warrior.transform;
            }
        }
        bool isIgnore = target == null;
        warriorManager.SetIgnoreCol(isIgnore);
        return target;
    }
    public void HandlePlayerMove()
    {
        isAttack = false;
    }
    public void HandleFinishMove(int id)
    {
        if (character.GetID() == id)
        {
            isAttack = true;
        }
    }

}
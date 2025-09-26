using System;
using System.Net.NetworkInformation;
using UnityEngine;

public class WarriorMove : CharacterMove
{
    [SerializeField] private float maxLimitDisToNewPos = 100f;
    public static event Action<int> Arrived;
    protected override void Start()
    {
        base.Start();
        status = Status.Moving;
    }
    protected override void Update()
    {
        base.Update();
        if (status == Status.BackHome)
        {
            if (Vector3.Distance(transform.position, newPos) < 0.0001)
            {
                StopMove();
            }
            GoToPos(newPos);
        }
        BackHomeState();
        if (status == Status.Moving && !gameManager.warriorManager.isIgnoreCol)
        {
            HandleArrived();
        }
    }
    private void BackHomeState()
    {
        if (Vector3.Distance(newPos, transform.position) > activityZone)
        {
            status = Status.BackHome;
        }
        if ((!target || target.GetComponent<Character>().GetIsDie()) && Vector3.Distance(transform.position, newPos) > 0.001)
        {
            status = Status.BackHome;
        }
    }
    protected override void HandleArrived()
    {
        Arrived?.Invoke(character.GetID());
        StopMove();
    }
    protected override void GoToPos(Vector3 pos)
    {
        base.GoToPos(pos);
        if (Vector3.Distance(transform.position, newPos) > maxLimitDisToNewPos)
        {
            transform.position = newPos;
            return;
        }
    }
    public override void SetTarget(Transform target)
    {
        if (status == Status.Moving) return;
        this.target = target;
        minDis = 0.0001f;
        status = target != null ? Status.Attack : status;
    }
    public override void SetTarget(Transform target, float minDis)
    {
        if (status == Status.Moving) return;
        base.SetTarget(target, minDis);
    }
}

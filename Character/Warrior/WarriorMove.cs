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
        if (status == Status.BackHome) GoToPos(newPos);
        BackHomeState();
        if(status == Status.Moving && !gameManager.warriorManager.isIgnoreCol) StopMove();
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
    public override void GoToPos(Vector3 newPos)
    {
        if (Vector3.Distance(transform.position, newPos) < 0.0001)
        {
            animator.SetBool("isRun", false);
            Arrived?.Invoke(character.GetID());
            StopMove();
            return;
        }
        transform.position = Vector3.MoveTowards(transform.position, newPos, character.GetSpeedMove() * Time.deltaTime);
        Flip(newPos);
        animator.SetBool("isRun", true);
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

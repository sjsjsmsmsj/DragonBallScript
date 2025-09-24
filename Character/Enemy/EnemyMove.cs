using UnityEngine;
public class EnemyMove : CharacterMove
{
    [SerializeField] private float distancePatrol = 2f;
    [SerializeField] private float timeDeplayMoveParol = 1f;
    private float timeNextMoveParol;
    private Vector3 initPos;
    private bool isParol;
    protected override void Start()
    {
        base.Start();
        initPos = transform.position;
        timeNextMoveParol = Time.time + timeDeplayMoveParol;
        status = Status.BackHome;
        isParol = false;
    }
    protected override void Update()
    {
        base.Update();
        if (status == Status.BackHome) GoToPos(initPos);
        PatrolState();
        BackHomeState();
    }
    private void RandomNewParolPos()
    {
        if (initPos == null) return;
        float x = initPos.x + Random.Range(-distancePatrol, distancePatrol);
        float y = initPos.y + Random.Range(-distancePatrol, distancePatrol);
        SetNewPos(new Vector3(x,y));
    }
    private void PatrolState()
    {
        if (status == Status.Attack) return;
        if (status == Status.Relax && isParol == false)
        {
            if (Vector3.Distance(transform.position, initPos) < 0.001)
            {
                timeNextMoveParol = Time.time + timeDeplayMoveParol;
                isParol = true;
            }
            return;
        }
        if (isParol == true && Time.time >= timeNextMoveParol)
        {
            RandomNewParolPos();
            timeNextMoveParol = Time.time + timeDeplayMoveParol;
        }
    }
    private void BackHomeState()
    {
        if (IsOutActivityZone(transform))
        {
            target = null;
            status = Status.BackHome;
        }
        if ((target == null || target.GetComponent<Character>().GetIsDie() ||
            IsOutActivityZone(target)) && status == Status.Attack)
        {
            target = null;
            status = Status.BackHome;
        }
    }
    public override void SetNewPos(Vector3 newPos)
    {
        if (status == Status.Attack) return;
        base.SetNewPos(newPos);
    }
    public bool IsOutActivityZone(Transform transform)
    {
        if(transform == null) return false;
        return Vector3.Distance(initPos, transform.position) > activityZone;
    }
    public override void SetTarget(Transform target)
    {
        if(IsOutActivityZone(target) || IsOutActivityZone(transform)) return;
        base.SetTarget(target);
        if(target != null) isParol = false;
    }
}

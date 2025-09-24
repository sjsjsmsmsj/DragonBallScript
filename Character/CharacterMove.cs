using System;
using Unity.VisualScripting;
using UnityEngine;

public abstract class CharacterMove : MonoBehaviour
{
    [SerializeField] protected float activityZone = 15f;
    public Status status { set; get; }
    public bool isMove {get; set;}
    public Character character {get; set;}
    public GameManager gameManager {get; set;}
    protected Animator animator;
    protected Transform target;
    protected Vector3 newPos;
    protected float minDis;
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        character = GetComponent<Character>();
        isMove = true;
        StopMove();
    }
    protected virtual void Update() 
    {
        if (character.GetIsDie() || !isMove || gameManager.isPauseGame)
        {
            StopMove();
            return;
        }
        if (status == Status.Moving) GoToPos(newPos);
        else if(status == Status.Attack) FollowToTarget(target);
    }
    public virtual void GoToPos(Vector3 newPos)
    {
        if (Vector3.Distance(transform.position, newPos) < 0.0001)
        {
            animator.SetBool("isRun", false);
            StopMove();
            return;
        }
        transform.position = Vector3.MoveTowards(transform.position, newPos, character.GetSpeedMove() * Time.deltaTime);
        Flip(newPos);
        animator.SetBool("isRun", true);
    }
    public virtual void FollowToTarget(Transform target)
    {
        if(target == null) return;
        if (Vector3.Distance(transform.position, target.position) < minDis)
        {
            animator.SetBool("isRun", false);
            StopMove();
            return;
        }
        transform.position = Vector3.MoveTowards(transform.position, target.position, character.GetSpeedMove() * Time.deltaTime);
        Flip(target.position);
        animator.SetBool("isRun", true);
    }
    public virtual void StopMove()
    {
        target = null;
        status = Status.Relax;
    }
    protected virtual void Flip(Vector3 targetPos)
    {
        if (transform.position.x < targetPos.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (transform.position.x > targetPos.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    public virtual void SetNewPos(Vector3 newPos)
    {
        target = null;
        this.newPos = newPos;
        status = Status.Moving;
    }
    public virtual void SetTarget(Transform target)
    {
        newPos = transform.position;
        this.target = target;
        minDis = 0.0001f;
        status = target != null ? Status.Attack : status;
    }
    public virtual void SetTarget(Transform target, float minDis)
    {
        SetTarget(target);
        this.minDis = minDis;
    }
}
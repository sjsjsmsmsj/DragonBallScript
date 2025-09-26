using System;
using System.Collections.Generic;
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
    protected FindPathManager findPathManager;
    protected Collider2D collider;
    protected List<Vector3> pathToPos;
    protected int currentIndex;
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        character = GetComponent<Character>();
        isMove = true;
        StopMove();
        if(character.gameManager != null) gameManager = character.gameManager;
        collider = GetComponent<Collider2D>();
        findPathManager = new FindPathManager(collider.bounds.size.x, collider.bounds.size.y, gameManager.cam, "Building");
    }
    protected virtual void Update() 
    {
        if (character.GetIsDie() || !isMove || gameManager.isPauseGame)
        {
            StopMove();
            return;
        }
        if (status == Status.Moving)
        {
            if (pathToPos == null || currentIndex == pathToPos.Count)
            {
                HandleArrived();
                pathToPos = null;
                return;
            }
            if (Vector3.Distance(transform.position, pathToPos[currentIndex]) < 0.0001)
            {
                currentIndex++;
                if (currentIndex == pathToPos.Count) return;
            }
            GoToPos(pathToPos[currentIndex]);
        }
        else if (status == Status.Attack)
        {
            FollowToTarget(target);
        }
    }
    protected virtual void HandleArrived()
    {
        StopMove();
    }
    protected virtual void GoToPos(Vector3 pos)
    {
        transform.position = Vector3.MoveTowards(transform.position, pos, character.GetSpeedMove() * Time.deltaTime);
        Flip(pos);
        animator.SetBool("isRun", true);
    }
    protected virtual void FollowToTarget(Transform target)
    {
        if(target == null) return;
        if (Vector3.Distance(transform.position, target.position) < minDis)
        {
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
        animator.SetBool("isRun", false);
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
    protected virtual void FindPathToPos(Vector3 pos)
    {
        Vector3 goal = new Vector3(pos.x, pos.y);
        Vector3 start = new Vector3(transform.position.x, transform.position.y);
        pathToPos = findPathManager.GetPath(start, goal, 1);
        currentIndex = 0;
    }
    public virtual void SetNewPos(Vector3 newPos)
    {
        target = null;
        this.newPos = newPos;
        status = Status.Moving;
        FindPathToPos(newPos);
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
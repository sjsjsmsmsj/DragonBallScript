using UnityEngine;
public abstract class Item : MonoBehaviour
{
    [SerializeField] protected float lifeTime;
    protected virtual int id => 0;
    protected GameManager gameManager;
    protected virtual void Start()
    {
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
            return;
        }
        Destroy(gameObject, lifeTime);
    }
    public virtual int GetID()
    {
        return id;
    }
    public virtual void SetGameManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

}
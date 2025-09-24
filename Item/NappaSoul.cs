using UnityEngine;
public class NappaSoul : Item
{
    protected override int id => ItemByID.NAPPA_SOUL;
    private WarriorManager warriorManager;
    protected override void Start()
    {
        if (warriorManager == null) return;
        if(warriorManager.IsExistWarriorInList(CharacterByID.NAPPA, warriorManager.GetWarriorPossessList()))
        {
            Destroy(gameObject);
            return;
        }
        base.Start();
    }
    private void OnMouseDown()
    {
        if (gameManager == null) return;
        gameManager.AddNewWarriorPossess(CharacterByID.NAPPA);
        Destroy(gameObject);
    }
    public void SetWarriorManager( WarriorManager warriorManager)
    {
        this.warriorManager = warriorManager;
    }
}

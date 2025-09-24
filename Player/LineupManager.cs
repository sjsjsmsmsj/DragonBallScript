using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
public enum Formation
{
    Attack, Protected, Massacre
}
public class LineupManager : MonoBehaviour
{
    [SerializeField] private float disWarrior = 1.5f;
    [SerializeField] private int numWarriorCircleIncrease = 5;
    [SerializeField] private float radiusWarriorCircle = 5f;
    private int edgeLength;
    private int numWarrior;
    private Formation currentFormation;

    public void AttackFormation(List<GameObject> warriors, Vector3 pos)
    {
        if (warriors == null || warriors.Count < 1) return;
        currentFormation = Formation.Attack;
        numWarrior = warriors.Count;
        edgeLength = Mathf.CeilToInt(Mathf.Sqrt(numWarrior));
        int count = 0;
        for(int i = 0; i < edgeLength; i++)
        {
            for (int j = 0; j < edgeLength; j++, count++)
            {
                if (count == numWarrior) return;
                float x = pos.x + i * disWarrior;
                float y = pos.y + j * disWarrior;
                WarriorMove warriorMove = warriors[count].GetComponent<WarriorMove>();
                warriorMove.SetNewPos(new Vector3(x, y));
            }
        }
    }
    public void ProtectedFormation(List<GameObject> warriors, Vector3 pos)
    {
        if (warriors == null || warriors.Count < 1) return;
        currentFormation = Formation.Protected;
        numWarrior = warriors.Count;
        int numWarriorInCircle = numWarriorCircleIncrease;
        int countWarriorInCircle = 0;
        int count = 1;
        warriors[0].GetComponent<WarriorMove>().SetNewPos(new Vector3(pos.x, pos.y));
        float r = disWarrior;
        while (count < numWarrior)
        {
            if (countWarriorInCircle == numWarriorInCircle)
            {
                countWarriorInCircle = 0;
                numWarriorInCircle += numWarriorInCircle;
                r += disWarrior;
                continue;
            }
            float angle = 2 * Mathf.PI / numWarriorInCircle;
            float x = Mathf.Cos(angle * countWarriorInCircle) * r + pos.x;
            float y = Mathf.Sin(angle * countWarriorInCircle) * r + pos.y;
            warriors[count].GetComponent<WarriorMove>().SetNewPos(new Vector3(x, y));
            count++;
            countWarriorInCircle++;
        }
    }
    public void MassacreFormation(List<GameObject> warriors, Vector3 pos)
    {
        if (warriors == null || warriors.Count < 1) return;
        currentFormation = Formation.Massacre;
        int numWarrior = warriors.Count;
        float angle = 2 * Mathf.PI / numWarrior;
        for (int i = 0; i < numWarrior; i++)
        {
            float x = Mathf.Cos(angle * i) * radiusWarriorCircle + pos.x;
            float y = Mathf.Sin(angle * i) * radiusWarriorCircle + pos.y;
            warriors[i].GetComponent<WarriorMove>().SetNewPos(new Vector3(x, y));
        }
    }
    public void FormationMoveToNewPos(List<GameObject> warriors, Vector3 pos)
    {
        if (currentFormation == Formation.Protected)
        {
            ProtectedFormation(warriors, pos);
        }
        else if (currentFormation == Formation.Attack)
        {
            AttackFormation(warriors, pos);
        }
        else if(currentFormation == Formation.Massacre)
        {
            MassacreFormation(warriors, pos);
        }
    }
    public Formation GetCurrentFormation()
    {
        return currentFormation;
    }
}

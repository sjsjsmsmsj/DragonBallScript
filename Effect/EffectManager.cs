using UnityEngine;
public class HandleEffectLifeTime : MonoBehaviour
{ 
    public void SetUpLifeTime(float lifeTime)
    {
        if(lifeTime <= 0)
        {
            Destroy(gameObject);
            return;
        }
        Destroy(gameObject, lifeTime);
    }
}
public class EffectManager : MonoBehaviour
{
    public const int POWER = 0;
    public const int ENERGY = 1;
    public const int CLICK = 2;
    public const int BUFFDAMAGE = 3;
    public const int BLOOD = 4;

    [SerializeField] private GameObject powerEffectRefap;
    [SerializeField] private GameObject energyEffecRefap;
    [SerializeField] private GameObject clickEffectRefap;
    [SerializeField] private GameObject buffDamageEffectRefap;
    [SerializeField] private GameObject bloodEffectRefap;
    public GameObject InitPowerEffectObject( Vector3 pos, float timeLife)
    {
        if (powerEffectRefap != null)
        {
            GameObject powerEffectObject = Instantiate(powerEffectRefap, pos, Quaternion.identity);
            HandleEffectLifeTime handleEffectLifeTime = powerEffectObject.AddComponent<HandleEffectLifeTime>();
            handleEffectLifeTime.SetUpLifeTime(timeLife);
            return powerEffectObject;
        }
        return null;
    }
    public GameObject InitEnergyEffectObject(Vector3 pos, float timeLife)
    {
        if (energyEffecRefap != null)
        {
            GameObject energyEffectObject = Instantiate(energyEffecRefap, pos, Quaternion.identity);
            HandleEffectLifeTime handleEffectLifeTime = energyEffectObject.AddComponent<HandleEffectLifeTime>();
            handleEffectLifeTime.SetUpLifeTime(timeLife);
            return energyEffectObject;
        }
        return null;
    }
    public GameObject InitClickEffectObject(Vector3 pos, float timeLife)
    {
        if (clickEffectRefap != null)
        {
            GameObject clickEffectObject = Instantiate(clickEffectRefap, pos, Quaternion.identity);
            HandleEffectLifeTime handleEffectLifeTime = clickEffectObject.AddComponent<HandleEffectLifeTime>();
            handleEffectLifeTime.SetUpLifeTime(timeLife);
            return clickEffectObject;
        }
        return null;
    }
    public GameObject InitBuffDameEffect(Vector3 pos, float timeLife)
    {
        if (buffDamageEffectRefap != null)
        {
            GameObject buffDamageEffectOBject = Instantiate(buffDamageEffectRefap, pos, Quaternion.identity);
            HandleEffectLifeTime handleEffectLifeTime = buffDamageEffectOBject.AddComponent<HandleEffectLifeTime>();
            handleEffectLifeTime.SetUpLifeTime(timeLife);
            return buffDamageEffectOBject;
        }
        return null;
    }
    public GameObject InitBloodEffect(Vector3 pos, float timeLife)
    {
        if (bloodEffectRefap != null)
        {
            GameObject bloodEffectOBject = Instantiate(bloodEffectRefap, pos, Quaternion.identity);
            HandleEffectLifeTime handleEffectLifeTime = bloodEffectOBject.AddComponent<HandleEffectLifeTime>();
            handleEffectLifeTime.SetUpLifeTime(timeLife);
            return bloodEffectOBject;
        }
        else
        {
            Debug.Log("BloodEffectRefap is null !");
            return null;
        }
    }
}

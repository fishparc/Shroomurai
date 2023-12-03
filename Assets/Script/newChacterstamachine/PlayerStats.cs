using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class PlayerStats : MonoBehaviour
{
    private Rigidbody2D rb;
    [Header("Basic stats")]
    public int maxHealth;
    public int currentHealth;
    public int maxEnergy;
    public int currentEnergy;
    public int energyConsumption;
    public int healthRegeneration;
    [Header("invunerability")]
    public float invunerableDuartion;
    
    public bool isInvunerable = false;
    public bool gotHurt=false;

    public UnityEvent<Transform> OnTakeDamage;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        currentEnergy = maxEnergy;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void TakeDamage(Attack attacker)
    {

        if (!isInvunerable)
        {
        if (currentHealth - attacker.damage > 0)
        {
            currentHealth -= attacker.damage;
            gotHurt=true;
            SetInvunerable();//hurtandinvinc
            OnTakeDamage?.Invoke(attacker.transform);
        }
        else
        {
            currentHealth = 0;
            //died
        }
        }
    }

    public void SetInvunerable()
    {
        isInvunerable = true;
        CancelInvoke("SetDamageable"); // in case the method has already been invoked
        Invoke("SetDamageable", invunerableDuartion);
    }

    void SetDamageable()
    {
        isInvunerable = false;
    }
    public void HealthRegen()
    {

        currentHealth = Mathf.Min(currentHealth + healthRegeneration, maxHealth);
    }
    public void EnergyConsume()
    {
        currentEnergy = Mathf.Max(currentEnergy - energyConsumption, 0);
    }
}

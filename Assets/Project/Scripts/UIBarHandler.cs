using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBarHandler : MonoBehaviour
{
    public Image healthBar;
    public Image staminaBar;
    public Image gasBar;

    // A positive value given to these functions will increase the bar's fill
    // A negative value will decrease it
    public float UpdateHealth(float amount)
    {
        // grab for easier referencing
        float health = GameManager.Instance.currHealth;
        float maxHealth = GameManager.Instance.maxHealth;

        Debug.Log("health: " + health);

        if (health + amount > maxHealth)
        {
            GameManager.Instance.currHealth = maxHealth;
        }
        else if (health + amount < 0)
        {
            GameManager.Instance.currHealth = 0;
        }
        else
        {
            GameManager.Instance.currHealth += amount;
        }
        
        UpdateBar(healthBar, (float) GameManager.Instance.currHealth / (float) GameManager.Instance.maxHealth);

        return GameManager.Instance.currHealth;
    }

    public float UpdateGas(float amount)
    {
        // grab for easier referencing
        float gas = GameManager.Instance.currGas;
        float maxGas = GameManager.Instance.maxGas;

        if (gas + amount > maxGas)
        {
            GameManager.Instance.currGas = maxGas;
        }
        else if (gas + amount < 0)
        {
            GameManager.Instance.currGas = 0;
        }
        else
        {
            GameManager.Instance.currGas += amount;
        }
        
        UpdateBar(gasBar, (float) GameManager.Instance.currGas / (float) GameManager.Instance.maxGas);

        return GameManager.Instance.currGas;
    }

    public float UpdateStamina(float amount)
    {
        // grab for easier referencing
        float stamina = GameManager.Instance.currStamina;
        float maxStamina = GameManager.Instance.maxStamina;

        if (stamina + amount > maxStamina)
        {
            GameManager.Instance.currStamina = maxStamina;
        }
        else if (stamina + amount < 0)
        {
            GameManager.Instance.currStamina = 0;
        }
        else
        {
            GameManager.Instance.currStamina += amount;
        }
        
        UpdateBar(staminaBar, (float) GameManager.Instance.currStamina / (float) GameManager.Instance.maxStamina);

        return GameManager.Instance.currStamina;
    }

    public void UpdateBar(Image bar, float fraction)
    {
        bar.fillAmount = fraction;
    }
}

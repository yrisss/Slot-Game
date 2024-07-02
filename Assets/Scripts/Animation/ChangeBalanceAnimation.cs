using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChangeBalanceAnimation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _balance;
    [SerializeField] private int _duration;
    [SerializeField] private int _countFPS;

    private int currentBalance;
    private int newBalance;
    
    public IEnumerator ChangeBalance()
    {
        WaitForSeconds Wait = new WaitForSeconds(1f / _countFPS);
        int stepAmount;

        stepAmount = 1;//Mathf.CeilToInt((newBalance - currentBalance) / (_duration));
        
        while (currentBalance < newBalance)
        {
            currentBalance += stepAmount;
            
            if (currentBalance > newBalance)
                currentBalance = newBalance;

            _balance.SetText(currentBalance + " $");

            yield return Wait;
        }
    }

    public void SetValues(int startBalance, int newBalance)
    {
        currentBalance = startBalance;
        this.newBalance = newBalance;
    }
}

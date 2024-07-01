using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private SlotsGameData slotsGameData;
    [SerializeField] private TextMeshProUGUI balance;
    [SerializeField] private GameObject Game1;

    public void LoadSlot(int index)
    {
        Instantiate(slotsGameData.SlotsGame[index]);
    }


    private void Start()
    {
        var currentBalance = PlayerPrefs.GetInt("Balance", 0);
        balance.text = currentBalance + " $";
    }

}

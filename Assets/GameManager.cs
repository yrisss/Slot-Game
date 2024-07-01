using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _balance;

    private void Start()
    {
        var currentBalance = PlayerPrefs.GetInt("Balance", 0);
       _balance.text = currentBalance + " $";
    }
}

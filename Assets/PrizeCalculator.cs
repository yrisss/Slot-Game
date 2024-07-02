using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrizeCalculator : MonoBehaviour
{
   [SerializeField] private ChangeBalanceAnimation _changeBalanceAnimation;

   private int startBalance;
   private int totalWin;
   private int newBalance;

   public void PrizeCalculate(List<int> winSymbolsCost)
   {
      startBalance = PlayerPrefs.GetInt("Balance", 0);
      totalWin = 0;
      foreach (var winSymbolCost in winSymbolsCost)
      {
         totalWin += winSymbolCost * 3;
      }

      newBalance = startBalance + totalWin;

      Debug.Log(totalWin + " " + newBalance);
      PlayerPrefs.SetInt("Balance", newBalance);
      _changeBalanceAnimation.SetValues(startBalance, newBalance);
   }
}

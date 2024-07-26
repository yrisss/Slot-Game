using System.Collections.Generic;
using Animation;
using UnityEngine;

namespace Infastructure.Services
{
   public class PrizeCalculator : MonoBehaviour
   {
      [SerializeField] private ChangeBalanceAnimation changeBalanceAnimation;

      private int _startBalance;
      private int _totalWin;

      public void PrizeCalculate(List<int> winSymbolsCost)
      {
         _startBalance = PlayerPrefs.GetInt("Balance", 0);
         _totalWin = 0;
         foreach (var winSymbolCost in winSymbolsCost)
         {
            _totalWin += winSymbolCost * 3;
         }
         
         changeBalanceAnimation.SetValues(_startBalance, _totalWin);
         changeBalanceAnimation.ChangeTotalWinBalance();
      }
   }
}

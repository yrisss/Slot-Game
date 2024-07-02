using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class Symbol : MonoBehaviour
{
    [SerializeField] private SymbolData _symbolData;

    public SymbolData SymbolInfo {get => _symbolData; set => _symbolData = value; }
}

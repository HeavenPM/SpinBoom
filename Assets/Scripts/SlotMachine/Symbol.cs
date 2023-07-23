using UnityEngine;

public class Symbol : MonoBehaviour
{
    public SymbolTypes SymbolType => _symbolType;

    [SerializeField] private SymbolTypes _symbolType; 
}

using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Reel : MonoBehaviour
{
    public List<Symbol> SymbolsInReel => _symbols;

    [SerializeField] private Symbol[] _symbolPrefabs;
    [SerializeField] private int _symbolsCount;
    [SerializeField] private float _generationOffset = 2.5f;
    [SerializeField] private float _rotatingTime;
    [SerializeField] private int _id;

    private List<Symbol> _symbols = new();
    private Vector3 _startPosition;
    private Vector3 _endPosition;

    private void Start()
    {
        GenerateNewSymbols(_symbolsCount);
        _startPosition = transform.position;

        float lastSymbolPositionY = 0f; 
        for (int i = 0; i < _symbolsCount; i++)
        {
            lastSymbolPositionY -= _generationOffset;
        }
        _endPosition = new(transform.position.x, lastSymbolPositionY + 2 * _generationOffset, transform.position.z);
    }

    private void GenerateNewSymbols(int countOfGenerations)
    {
        int startGenerationIndex = _symbols.Count == 0 ? 0 : 3;

        for (int i = startGenerationIndex; i < countOfGenerations; i++)
        {
            int randomIndex = Random.Range(0, _symbolPrefabs.Length);
            Vector3 generationPosition = new(transform.position.x, transform.position.y + _generationOffset * i, transform.position.z);
            Symbol newSymbol = Instantiate(_symbolPrefabs[randomIndex], generationPosition, transform.rotation);
            _symbols.Add(newSymbol);
            newSymbol.transform.SetParent(transform, true);
        }
    }

    private void OnReelStartedRotating(int id)
    {
        if (id == _id)
        {
            Vector3 endPosition = new(_endPosition.x, _endPosition.y, _endPosition.z);
            transform.DOMove(endPosition, _rotatingTime).OnComplete(OnCompleteRotating);
        }
    }

    private void OnCompleteRotating()
    {
        Vector3 startPosition = new(_startPosition.x, _startPosition.y, _startPosition.z);
        transform.position = startPosition;

        int visibleSymbolsCount = 3;

        while (_symbols.Count > visibleSymbolsCount)
        {
            Symbol symbolToRemove = _symbols[0];
            _symbols.Remove(symbolToRemove);
            Destroy(symbolToRemove.gameObject);
        }

        float symbolPosition = 0f;
        for (int i = 0; i < _symbols.Count; i++)
        {
            _symbols[i].transform.localPosition = new(0, symbolPosition, 0);
            symbolPosition += _generationOffset;
        }

        GenerateNewSymbols(_symbolsCount);

        int lastReelIndex = 4;
        if (_id == lastReelIndex)
        {
            EventManager.OnLastReelStopped();
        }
    }

    private void OnEnable()
    {
        EventManager.ReelStartedRotating += OnReelStartedRotating;
    }

    private void OnDisable()
    {
        EventManager.ReelStartedRotating -= OnReelStartedRotating;
    }
}

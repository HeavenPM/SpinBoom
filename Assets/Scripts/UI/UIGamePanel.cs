using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIGamePanel : MonoBehaviour
{
    [SerializeField] private SlotMachine _slotMachine;
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private Button _spinButton;
    [SerializeField] private Button _increaseButton;
    [SerializeField] private Button _reduceButton;
    [SerializeField] private Button _maxBetButton;
    [SerializeField] private Sprite _increaseButtonUnavailable;
    [SerializeField] private Sprite _reduceButtonUnavailable;
    [SerializeField] private Sprite _spinButtonUnavailable;
    [SerializeField] private Sprite _spinButtonPressed;
    [SerializeField] private Sprite _maxBetButtonPressed;
    [SerializeField] private Sprite _maxBetButtonUnavailable;

    private Sprite _spinButtonDefault;
    private Sprite _increaseButtonDefault;
    private Sprite _reduceButtonDefault;
    private Sprite _maxBetButtonDefault;

    private void Start()
    {
        _spinButtonDefault = _spinButton.image.sprite;
        _increaseButtonDefault = _increaseButton.image.sprite;
        _reduceButtonDefault = _reduceButton.image.sprite;
        _maxBetButtonDefault = _maxBetButton.image.sprite;
        _settingsPanel.SetActive(false);
    }

    private void Update()
    {
        if (_slotMachine.MaxBet <= _slotMachine.MoneyOnBalance)
        {
            _maxBetButton.enabled = true;
            if (_slotMachine.IsMaxBet) _maxBetButton.image.sprite = _maxBetButtonPressed;
            else _maxBetButton.image.sprite = _maxBetButtonDefault;
        }
        else
        {
            _maxBetButton.image.sprite = _maxBetButtonUnavailable;
            _maxBetButton.enabled = false;
        }
    }

    public void Spin()
    {
        if (_slotMachine.BettingMoney <= _slotMachine.MoneyOnBalance)
        {
            EventManager.OnSpinButtonPressed();
            _spinButton.enabled = false;
            _spinButton.image.sprite = _spinButtonPressed;
        }    
    }

    public void IncreaseBet()
    {
        EventManager.OnIncreaseBetButtonPressed();
        _reduceButton.enabled = true;
        _reduceButton.image.sprite = _reduceButtonDefault;
    }

    public void ReduceBet()
    {
        EventManager.OnReduceBetButtonPressed();
        _increaseButton.enabled = true;
        _increaseButton.image.sprite = _increaseButtonDefault;
    }

    public void SetMaxBet()
    {
        EventManager.OnSetMaxBet();
        if (_slotMachine.IsMaxBet)
        {
            OnMakeIncreaseBetButtonUnavailable();
            OnMakeReduceBetButtonUnavailable();
        }
        else
        {
            _increaseButton.enabled = true;
            _increaseButton.image.sprite = _increaseButtonDefault;
        }
    }

    public void OpenSettings()
    {
        _settingsPanel.SetActive(true);
    }

    private void OnMakeIncreaseBetButtonUnavailable()
    {
        _increaseButton.enabled = false;
        _increaseButton.image.sprite = _increaseButtonUnavailable;
    }

    private void OnMakeReduceBetButtonUnavailable()
    {
        _reduceButton.enabled = false;
        _reduceButton.image.sprite = _reduceButtonUnavailable;
    }

    private void OnMakeSpinButtonAvailable()
    {
        _spinButton.enabled = true;
        _spinButton.image.sprite = _spinButtonDefault;
    }

    private void OnMakeSpinButtonUnavailable()
    {
        _spinButton.enabled = false;
        _spinButton.image.sprite = _spinButtonUnavailable;
    }

    private void OnEndOfSpin()
    {
        if (_slotMachine.IsMaxBet)
        {
            _increaseButton.enabled = true;
            _increaseButton.image.sprite = _increaseButtonDefault;
            _reduceButton.enabled = true;
            _reduceButton.image.sprite = _reduceButtonDefault;
        }
    }

    private void OnEnable()
    {
        EventManager.EndOfSpin += OnEndOfSpin;
        EventManager.MakeIncreaseBetButtonUnavailable += OnMakeIncreaseBetButtonUnavailable;
        EventManager.MakeReduceBetButtonUnavailable += OnMakeReduceBetButtonUnavailable;
        EventManager.MakeSpinButtonUnavailable += OnMakeSpinButtonUnavailable;
        EventManager.MakeSpinButtonAvailable += OnMakeSpinButtonAvailable;
    }

    private void OnDisable()
    {
        EventManager.EndOfSpin -= OnEndOfSpin;
        EventManager.MakeIncreaseBetButtonUnavailable -= OnMakeIncreaseBetButtonUnavailable;
        EventManager.MakeReduceBetButtonUnavailable -= OnMakeReduceBetButtonUnavailable;
        EventManager.MakeSpinButtonUnavailable -= OnMakeSpinButtonUnavailable;
        EventManager.MakeSpinButtonAvailable -= OnMakeSpinButtonAvailable;
    }
}

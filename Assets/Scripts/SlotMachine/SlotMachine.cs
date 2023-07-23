using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    [System.Serializable]
    public class WinCombination
    {
        public SymbolTypes SymbolType;
        public int SymbolsInCombo;
        public float Payout;
    }

    public float MoneyOnBalance => _moneyOnBalance;
    public float BettingMoney => _bettingMoney;
    public bool IsMaxBet => _isMaxBet;
    public float MaxBet => _betVariants[^1];

    [SerializeField] private Reel[] _reels;
    [SerializeField] private WinCombination[] _winCombinations;
    [SerializeField] private float _moneyOnBalance;
    [SerializeField] private TMP_Text _moneyOnBalanceText;
    [SerializeField] private TMP_Text _wonMoneyText;
    [SerializeField] private TMP_Text _bettingMoneyText;
    [SerializeField] private float[] _betVariants;
    [SerializeField] private GameObject[] _glowingBoxes;
    [SerializeField] private AudioClip _spinningSound;
    [SerializeField] private AudioClip _wonSound;

    private float _wonMoney = 0f;
    private float _bettingMoney = 100f;
    private int _betVariantsPointer = 2;
    private int _foundCombinations = 0;
    private bool _isRotating = false;
    private bool _isMaxBet = false;
    private Vector3 _originalScale;
    private Color _originalColor;
    private Dictionary<string, float> _combinationsDictionary = new();

    private void Start()
    {
        _originalScale = _bettingMoneyText.transform.localScale;
        _originalColor = _bettingMoneyText.color;

        for (int i = 0; i < _winCombinations.Length; i++)
        {
            _combinationsDictionary.Add(_winCombinations[i].SymbolType.ToString() +
                " x" + _winCombinations[i].SymbolsInCombo,
                _winCombinations[i].Payout);
        }

        _moneyOnBalanceText.text = _moneyOnBalance.ToString();
        _wonMoneyText.text = _wonMoney.ToString();
        _bettingMoneyText.text = _bettingMoney.ToString();
        AnimateText(_moneyOnBalanceText);
        AnimateText(_wonMoneyText);
        AnimateText(_bettingMoneyText);
    }

    private void OnSpinButtonPressed()
    {
        if (_bettingMoney <= _moneyOnBalance)
        {
            _moneyOnBalance -= _bettingMoney;
            _moneyOnBalanceText.text = _moneyOnBalance.ToString();
            AnimateText(_moneyOnBalanceText);
            StartCoroutine(StartAllReels());
            _isRotating = true;
            AudioManager.Instance.PlaySound(_spinningSound, 1f);
        }     
    }

    private void OnLastReelStopped()
    {
        AudioManager.Instance.StopSound();
        const int LINES = 3;
        int[] comboSymbolsCountInLines = new int[LINES] { 0, 0, 0 };
        float[] payoutInLines = new float[LINES] { 0, 0, 0 };
        SymbolTypes[] typesInLines = new SymbolTypes[LINES] {SymbolTypes.Wild, SymbolTypes.Wild, SymbolTypes.Wild };

        _foundCombinations = 0;
        _isRotating = false;

        for (int i = 0; i < LINES; i++)
        {
            List<SymbolTypes> lineContent = new();
            for (int j = 0; j < _reels.Length; j++)
            {
                Symbol symbol = _reels[j].SymbolsInReel[i];
                lineContent.Add(symbol.SymbolType);
            }

            if (CheckIsAllWild(lineContent))
            {
                comboSymbolsCountInLines[i] = 5;
                payoutInLines[i] = CheckCombination(typesInLines[i], comboSymbolsCountInLines[i]);
            }
            else
            {
                if (lineContent[0] == SymbolTypes.Wild)
                {
                    comboSymbolsCountInLines[i]++;
                    for (int l = 1; l < lineContent.Count; l++)
                    {
                        if (lineContent[l] != SymbolTypes.Wild)
                        {
                            if (typesInLines[i] == SymbolTypes.Wild)
                            {
                                typesInLines[i] = lineContent[l];
                                comboSymbolsCountInLines[i]++;
                            }
                            else
                            {
                                if (lineContent[l] == typesInLines[i]) comboSymbolsCountInLines[i]++;
                                else break;
                            }
                        }
                        else comboSymbolsCountInLines[i]++;
                    }
                    payoutInLines[i] = CheckCombination(typesInLines[i], comboSymbolsCountInLines[i]);
                }
                else
                {
                    typesInLines[i] = lineContent[0];
                    for (int l = 0; l < lineContent.Count; l++)
                    {
                        if (lineContent[l] != typesInLines[i] && lineContent[l] != SymbolTypes.Wild) break;
                        else comboSymbolsCountInLines[i]++;
                    }
                    payoutInLines[i] = CheckCombination(typesInLines[i], comboSymbolsCountInLines[i]);
                }
            }
        }

        if (_foundCombinations == 0)
        {
            _wonMoney = 0; 
        }
        else
        {
            int wonLine = 0;

            for (int i = 0; i < payoutInLines.Length; i++)
            {
                if (payoutInLines[i] > 0 && payoutInLines[i] > payoutInLines[wonLine])
                {
                    wonLine = i;
                }
            }
            _wonMoney = _bettingMoney * payoutInLines[wonLine];
            _moneyOnBalance += _wonMoney;
            _moneyOnBalanceText.text = _moneyOnBalance.ToString();
            AnimateText(_moneyOnBalanceText);
            ActivateGlowingBoxes(wonLine, comboSymbolsCountInLines[wonLine]);
            AudioManager.Instance.PlaySound(_wonSound, 0.5f);
        }
        AnimateText(_wonMoneyText);
        _wonMoneyText.text = _wonMoney.ToString();
        if (_wonMoney == 0) CheckPossibilityOfSpin();
    }

    private void ActivateGlowingBoxes(int wonLine, int boxesCount)
    {
        for (int i = 0; i < boxesCount; i++)
        {
            ActivateAndAnimate(_glowingBoxes[wonLine].transform.GetChild(i).gameObject);
        }
    }

    private void ActivateAndAnimate(GameObject targetGameObject)
    {
        float fadeInDuration = 1f;
        float fadeOutDuration = 1f;
        float flickerDuration = 1f;
        float flickerScale = 0.1f;
        targetGameObject.SetActive(true);

        CanvasGroup canvasGroup = targetGameObject.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        float originalScale = targetGameObject.transform.localScale.x;
        Sequence animationSequence = DOTween.Sequence();
        animationSequence.Append(canvasGroup.DOFade(1f, fadeInDuration).SetEase(Ease.Linear));
        animationSequence.Join(targetGameObject.transform.DOScale(originalScale + flickerScale, flickerDuration * 0.5f).SetEase(Ease.InOutQuad));
        animationSequence.Append(targetGameObject.transform.DOScale(originalScale, flickerDuration * 0.5f).SetEase(Ease.InOutQuad));
        animationSequence.Append(canvasGroup.DOFade(0f, fadeOutDuration).SetDelay(flickerDuration));

        animationSequence.OnComplete(() =>
        {
            canvasGroup.alpha = 0f;
            targetGameObject.transform.localScale = Vector3.one * originalScale;
            Deactivate(targetGameObject);
        });
    }


    private void Deactivate(GameObject targetGameObject)
    {
        targetGameObject.SetActive(false);
        EventManager.OnEndOfSpin();
        CheckPossibilityOfSpin();
    }

    private float CheckCombination(SymbolTypes type, int count)
    {
        string key = type.ToString() + " x" + count.ToString();
        if (_combinationsDictionary.ContainsKey(key))
        {
            _foundCombinations++;
            return _combinationsDictionary[key];
        }
        else return 0f;
    }

    private bool CheckIsAllWild(List<SymbolTypes> lineContent)
    {
        for (int i = 0; i < lineContent.Count; i++)
        {
            if (lineContent[i] != SymbolTypes.Wild) return false;
        }
        return true;
    }

    private void CheckPossibilityOfSpin()
    {
        if (!_isRotating)
        {
            if (_bettingMoney > _moneyOnBalance)
            {
                EventManager.OnMakeSpinButtonUnavailable();
                if (_isMaxBet)
                {
                    EventManager.OnEndOfSpin();
                    _isMaxBet = false;
                    _betVariantsPointer = 2;
                    _bettingMoney = _betVariants[_betVariantsPointer];
                    _bettingMoneyText.text = _bettingMoney.ToString();
                    AnimateText(_bettingMoneyText);           
                }
            }
            else EventManager.OnMakeSpinButtonAvailable();
        }   
    }

    private void OnIncreaseBetButtonPressed()
    {
        if (_betVariantsPointer < _betVariants.Length - 1)
        {
            _betVariantsPointer++;
            _bettingMoney = _betVariants[_betVariantsPointer];
            _bettingMoneyText.text = _bettingMoney.ToString();
            AnimateText(_bettingMoneyText);
            CheckPossibilityOfSpin();
        }
        else EventManager.OnMakeIncreaseBetButtonUnavailable();
    }

    private void OnReduceBetButtonPressed()
    {
        if (_betVariantsPointer > 0)
        {
            _betVariantsPointer--;
            _bettingMoney = _betVariants[_betVariantsPointer];
            _bettingMoneyText.text = _bettingMoney.ToString();
            AnimateText(_bettingMoneyText);
            CheckPossibilityOfSpin();
        }
        else EventManager.OnMakeReduceBetButtonUnavailable();
    }

    private void OnSetMaxBet()
    {
        _isMaxBet = !_isMaxBet;
        if (_isMaxBet) _betVariantsPointer = _betVariants.Length - 1;
        else _betVariantsPointer = 0;
        _bettingMoney = _betVariants[_betVariantsPointer];
        _bettingMoneyText.text = _bettingMoney.ToString();
        AnimateText(_bettingMoneyText);
    }

    private void AnimateText(TMP_Text text)
    {
        float scaleMultiplier = 1.7f;
        float animationDuration = 0.5f;

        text.transform.DOScale(_originalScale * scaleMultiplier, animationDuration)
            .OnComplete(() => text.transform.DOScale(_originalScale, animationDuration));

        text.DOColor(new Color(_originalColor.r, _originalColor.g, _originalColor.b, 0.7f), animationDuration)
            .OnComplete(() => text.DOColor(_originalColor, animationDuration));
    }

    private IEnumerator StartAllReels()
    {
        int reelsCount = 5;
        float timeBetweenReels = 0.3f;
        for (int i = 0; i < reelsCount; i++)
        {
            EventManager.OnReelStartedRotating(i);
            yield return new WaitForSeconds(timeBetweenReels);
        }
    }

    private void OnEnable()
    {
        EventManager.SpinButtonPressed += OnSpinButtonPressed;
        EventManager.LastReelStopped += OnLastReelStopped;
        EventManager.IncreaseBetButtonPressed += OnIncreaseBetButtonPressed;
        EventManager.ReduceBetButtonPressed += OnReduceBetButtonPressed;
        EventManager.SetMaxBet += OnSetMaxBet;
    }

    private void OnDisable()
    {
        EventManager.SpinButtonPressed -= OnSpinButtonPressed;
        EventManager.LastReelStopped -= OnLastReelStopped;
        EventManager.IncreaseBetButtonPressed -= OnIncreaseBetButtonPressed;
        EventManager.ReduceBetButtonPressed -= OnReduceBetButtonPressed;
        EventManager.SetMaxBet -= OnSetMaxBet;
    }
}

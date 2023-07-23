using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UISettingsPanel : MonoBehaviour
{
    [SerializeField] private GameObject _musicIcon;
    [SerializeField] private GameObject _soundIcon;
    [SerializeField] private Sprite _soundOnSprite;
    [SerializeField] private Sprite _soundOffSprite;
    [SerializeField] private string _url;

    private bool _isSoundOn = true;
    private bool _isMusicOn = true;

    private void Start()
    {
        _musicIcon.GetComponent<Image>().sprite = _soundOnSprite;
        _soundIcon.GetComponent<Image>().sprite = _soundOnSprite;
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    public void ToggleSound()
    {
        _isSoundOn = !_isSoundOn;
        EventManager.OnToggleSoundState();
        if (_isSoundOn) _soundIcon.GetComponent<Image>().sprite = _soundOnSprite;
        else _soundIcon.GetComponent<Image>().sprite = _soundOffSprite;
    }

    public void ToggleMusic()
    {
        _isMusicOn = !_isMusicOn;
        EventManager.OnToggleMusicState();
        if (_isMusicOn) _musicIcon.GetComponent<Image>().sprite = _soundOnSprite;
        else _musicIcon.GetComponent<Image>().sprite = _soundOffSprite;
    }

    public void ShareApp()
    {
        string message = "Check out this awesome app!";
        Application.OpenURL("https://twitter.com/intent/tweet?text=" + UnityWebRequest.EscapeURL(message + "\n" + _url));
    }

    public void ContactUs()
    {
        Application.OpenURL(_url);
    }

    public void PrivatePolicy()
    {
        Application.OpenURL(_url);
    }
}

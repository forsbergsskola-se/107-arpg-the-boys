using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseMenuScript : MonoBehaviour
{
    public float duration;

    private bool _isPaused;
    private float _transitionTimer;
    private bool _shouldPop;

    public GameObject[] pauseObjects;
    public GameObject[] optionObjects;

    [SerializeField] 
    private AudioMixer masterMixer;

    void Update()
    {
        PauseMenu();
        OptionMenu();
    }

    void PauseMenu()
    {
        if (_menuState != MenuState.OptionMenu)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                Resume();
            }

            if (_isPaused)
            {
                Time.timeScale = Mathf.Lerp(1, 0, _transitionTimer / duration);
                _transitionTimer += Time.unscaledDeltaTime;
                if (_transitionTimer >= duration)
                    _shouldPop = true;
            }

            else
            {
                Time.timeScale = Mathf.Lerp(0, 1, _transitionTimer / duration);
                _transitionTimer += Time.unscaledDeltaTime;
                _menuState = MenuState.NoMenu;
                _shouldPop = true;
            }
            
            if (_shouldPop)
            {
                PopUp();
            }
        }
    }

    void OptionMenu()
    {
        if (_menuState == MenuState.OptionMenu)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                _menuState = MenuState.PauseMenu;
            }
        }
    }

    void Resume()
    {
        _transitionTimer = 0;
        _shouldPop = false;
        _isPaused = !_isPaused;
        _menuState = _isPaused ? MenuState.PauseMenu : MenuState.NoMenu;
    }

    void PopUp()
    {
        if (_menuState == MenuState.PauseMenu)
        {
            OpenButtons(pauseObjects);
            CloseButtons(optionObjects);
        }

        if (_menuState == MenuState.NoMenu)
        {
            CloseButtons(optionObjects);
            CloseButtons(pauseObjects);
        }
        
        if (_menuState == MenuState.OptionMenu)
        {
            OpenButtons(optionObjects);
            CloseButtons(pauseObjects);
        }
    }

    void CloseButtons(GameObject[] buttons)
    {
        for (var i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetActive(false);
        }
    }
    
    void OpenButtons(GameObject[] buttons)
    {
        for (var i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetActive(true);
        }
    }

    private MenuState _menuState = MenuState.NoMenu;

    enum MenuState
    {
        PauseMenu,
        OptionMenu,
        NoMenu
    }
    
    //button functions
    public void ResumeButton()
    {
        Resume();
    }

    public void OptionsButton()
    {
        _menuState = MenuState.OptionMenu;
        PopUp();
    }

    public void LeaveRunButton()
    {
        _menuState = MenuState.NoMenu;
        SceneManager.LoadScene(0);
    }
    
    public void QuitButton()
    {
        Application.Quit();
    }

    public void MasterSlider(float sliderValue)
    {
        masterMixer.SetFloat("Master Volume", Mathf.Log10(sliderValue) * 20);
    }

    public void SfxSlider(float sliderValue)
    {
        masterMixer.SetFloat("SFX Volume", Mathf.Log10(sliderValue) * 20);
    }

    public void MusicSlider(float sliderValue)
    {
        masterMixer.SetFloat("Music Volume", Mathf.Log10(sliderValue) * 20);
    }
}
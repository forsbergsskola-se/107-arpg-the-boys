using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuScript : MonoBehaviour
{
    public float duration;

    private bool _isPaused;
    private float _transitionTimer;
    private bool _shouldPop;

    public PlayerMovement playerMovement;
    public GameObject pauseMenu;
    public GameObject optionsMenu;

    [Header("Audio")]
    [SerializeField] 
    private AudioMixer mixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Awake()
    {
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        
    }

    private void Start()
    {
        masterSlider.value = PlayerPrefs.GetFloat("Master_Key", 0.4f);
        musicSlider.value = PlayerPrefs.GetFloat("Music_Key", 0.4f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFX_Key", 0.4f);
        
        SetMasterVolume(masterSlider.value);
        SetMusicVolume(musicSlider.value);
        SetSFXVolume(sfxSlider.value);
    }

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

            _shouldPop = true;
            
            if (_isPaused)
            {
                if (_shouldPop)
                {
                    PopUp();
                }
                Time.timeScale = Mathf.Lerp(1, 0, _transitionTimer / duration);
                _transitionTimer += Time.unscaledDeltaTime;
                if (_transitionTimer >= duration)
                    _shouldPop = true;
            }

            else
            {
                if (_shouldPop)
                {
                    PopUp();
                }
                Time.timeScale = Mathf.Lerp(0, 1, _transitionTimer / duration);
                _transitionTimer += Time.unscaledDeltaTime;
                _menuState = MenuState.NoMenu;
                _shouldPop = true;
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
            playerMovement.canMove = false;
            OpenMenu(pauseMenu);
            CloseMenu(optionsMenu);
        }

        if (_menuState == MenuState.NoMenu)
        {
            playerMovement.canMove = true;
            CloseMenu(optionsMenu);
            CloseMenu(pauseMenu);
        }
        
        if (_menuState == MenuState.OptionMenu)
        {
            playerMovement.canMove = false;
            OpenMenu(optionsMenu);
            CloseMenu(pauseMenu);
        }
    }

    void CloseMenu(GameObject menu)
    {
        menu.SetActive(false);
        
        PlayerPrefs.SetFloat("Master_Key", masterSlider.value);
        PlayerPrefs.SetFloat("Music_Key", musicSlider.value);
        PlayerPrefs.SetFloat("SFX_Key", sfxSlider.value);
    }
    
    void OpenMenu(GameObject menu)
    {
        menu.SetActive(true);
    }

    private MenuState _menuState = MenuState.NoMenu;

    enum MenuState
    {
        PauseMenu,
        OptionMenu,
        NoMenu
    }
    
    //button functions

    public void NewGameButton()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(0);
    }
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

    public void SetMasterVolume(float sliderValue)
    {
        mixer.SetFloat("Master Volume", Mathf.Log10(sliderValue) * 20);
    }

    public void SetSFXVolume(float sliderValue)
    {
        mixer.SetFloat("SFX Volume", Mathf.Log10(sliderValue) * 20);
    }

    public void SetMusicVolume(float sliderValue)
    {
        mixer.SetFloat("Music Volume", Mathf.Log10(sliderValue) * 20);
    }
}
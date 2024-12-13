using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

public class UIMenuManager : MonoBehaviour
{
    [SerializeField] private UIPopup _incorrectInfoPanel = default;
    [SerializeField] private UIPopup _serverErrorPanel = default;
    
    [SerializeField] private UIMainMenu mainMenu;
    [SerializeField] private UISignUp signUp;
    
    [SerializeField] private GameStateSO gameState;
    
    [Header("Broadcasting on")]
    [SerializeField] private VoidEventChannelSO _startGameEvent = default;
    [SerializeField] private VoidEventChannelSO _startNewGameEvent = default;
    [SerializeField] private UserLoginRequestSO _userLoginRequest = default;
    [SerializeField] private UserLoginRequestSO _userRegisterRequest = default;
    
    [Header("Listening To")]
    [SerializeField] private BoolEventChannelSO _loginCorrectEvent = default;
    [SerializeField] private BoolEventChannelSO _serverErrorEvent = default;

    private void OnEnable()
    {
        gameState.UpdateGameState(GameState.Menu);
    }

    private void Start()
    {
        mainMenu.LoginAction += LoginButtonClicked;
        mainMenu.SignUpAction += SignUpButtonClicked;

        _serverErrorEvent.OnEventRaised += CheckServerError;
        _loginCorrectEvent.OnEventRaised += CheckLoginCorrect;
    }

    private void OnDisable()
    {
        mainMenu.LoginAction -= LoginButtonClicked;
        mainMenu.SignUpAction -= SignUpButtonClicked;
        
        _loginCorrectEvent.OnEventRaised -= CheckLoginCorrect;
        _serverErrorEvent.OnEventRaised -= CheckServerError;
    }

    private void LoginButtonClicked(string login, string password)
    {
        _userLoginRequest.RaiseEvent(login, password);
    }

    private void RegisterButtonClicked(string login, string password)
    {
        _userRegisterRequest.RaiseEvent(login, password);  
    }

    private void SignInButtonClicked()
    {
        signUp.SignInAction -= SignInButtonClicked;
        signUp.SignUpAction -= RegisterButtonClicked;
        
        signUp.gameObject.SetActive(false);
        
        mainMenu.gameObject.SetActive(true);
    }

    private void SignUpButtonClicked()
    {
        mainMenu.gameObject.SetActive(false);
        
        signUp.SignInAction += SignInButtonClicked;
        signUp.SignUpAction += RegisterButtonClicked;
        
        signUp.gameObject.SetActive(true);
    }

    private void CheckLoginCorrect(bool isValid)
    {
        if (isValid)
        {
            _startGameEvent.RaiseEvent();
            gameState.UpdateGameState(GameState.Gameplay);
        }
        else
        {
            _incorrectInfoPanel.ClosePopupAction += HideIncorrectInfoPopup;
            _incorrectInfoPanel.gameObject.SetActive(true);
        }
    }

    private void CheckServerError(bool isServerError)
    {
        if (isServerError)
        {
            _serverErrorPanel.ClosePopupAction += HideServerErrorPopup;
            _serverErrorPanel.gameObject.SetActive(true);
        }
        else
        {
            _startNewGameEvent.RaiseEvent();
            gameState.UpdateGameState(GameState.AvatarCreation);
        }
    }
    
    void HideIncorrectInfoPopup(bool value)
    {
        _incorrectInfoPanel.ClosePopupAction -= HideIncorrectInfoPopup;
        _incorrectInfoPanel.gameObject.SetActive(false);

        mainMenu.ClearInputs();
    }
    
    void HideServerErrorPopup(bool value)
    {
        _serverErrorPanel.ClosePopupAction -= HideServerErrorPopup;
        _serverErrorPanel.gameObject.SetActive(false);

        mainMenu.ClearInputs();
    }
}

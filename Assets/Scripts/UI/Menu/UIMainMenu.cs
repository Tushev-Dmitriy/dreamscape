using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private InputField loginInputField;
    [SerializeField] private InputField passwordInputField;
    [SerializeField] private Button signInButton;
    [SerializeField] private Button signUpButton;
    
    public UnityAction<string, string> LoginAction;
    public UnityAction SignUpAction;

    public void Login()
    {
        LoginAction.Invoke(loginInputField.text, passwordInputField.text);
    }

    public void SignUp()
    {
        SignUpAction.Invoke();
    }

    public void ClearInputs()
    {
        loginInputField.text = "";
        passwordInputField.text = "";
    }
    
}

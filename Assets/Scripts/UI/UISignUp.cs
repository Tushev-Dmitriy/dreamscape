using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class UISignUp : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private InputField loginInputField;
        [SerializeField] private InputField passwordInputField;
        [SerializeField] private Button signInButton;
        [SerializeField] private Button signUpButton;
    
        public UnityAction<string, string> SignUpAction;
        public UnityAction SignInAction;

        public void SignUp()
        {
            SignUpAction.Invoke(loginInputField.text, passwordInputField.text);
        }

        public void SignIn()
        {
            SignInAction.Invoke();
        }
    }
}
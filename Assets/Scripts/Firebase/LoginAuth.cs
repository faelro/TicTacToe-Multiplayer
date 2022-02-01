using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using TMPro;

public class LoginAuth : MonoBehaviour
{
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    public void LoginButton()
    {
        StartCoroutine(StartLogin(emailInputField.text, passwordInputField.text));
    }

    private IEnumerator StartLogin(string email, string password)
    {
        var LoginTask = FirebaseAuthenticator.instance.auth.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception !=null)
        {
            HandleLoginErrors(LoginTask.Exception);
        }
        else
        {
            LoginUser(LoginTask);
        }
    }

    void HandleLoginErrors(System.AggregateException loginException)
    {
        Debug.LogWarning(message: $"Failed to login task with{loginException}");
        FirebaseException firebaseEx = loginException.GetBaseException() as FirebaseException;
        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

        warningLoginText.text = DefineLoginErrorMessage(errorCode);
    }

    string DefineLoginErrorMessage(AuthError errorCode)
    {
        switch (errorCode)
        {
            case AuthError.MissingEmail:
                return "Email faltando";
            case AuthError.MissingPassword:
                return "Senha faltando";
            case AuthError.InvalidEmail:
                return "Email inválido";
            case AuthError.UserNotFound:
                return "Conta não existente";
            default:
                return "Senha inválida";
        }
    }

    void LoginUser(System.Threading.Tasks.Task<Firebase.Auth.FirebaseUser> loginTask)
    {
        FirebaseAuthenticator.instance.User = loginTask.Result;
        Debug.LogFormat("User signed in succesfully: {0} ({1})", FirebaseAuthenticator.instance.User.DisplayName, FirebaseAuthenticator.instance.User.Email);
        warningLoginText.text = "";
        confirmLoginText.text = "Login bem sucedido";
        SceneManager.LoadScene("MainScene");

    }
}

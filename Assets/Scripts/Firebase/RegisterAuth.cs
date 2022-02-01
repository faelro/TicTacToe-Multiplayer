using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;

public class RegisterAuth : MonoBehaviour
{
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField verifyPasswordRegisterField;
    public TMP_Text warningRegisterText;

    public void RegisterButton()
    {
        StartCoroutine(StartRegister(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    private IEnumerator StartRegister(string email, string password, string userName)
    {
        if (!CheckRegistrationFieldsAndReturnForErrors())
        {
            var RegisterTask = FirebaseAuthenticator.instance.auth.CreateUserWithEmailAndPasswordAsync(email, password);
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                HandleRegisterErrors(RegisterTask.Exception);
            }
            else
            {
                StartCoroutine(RegisterUser(RegisterTask, userName, email, password));
            }
        }
    }

    bool CheckRegistrationFieldsAndReturnForErrors()
    {
        if (usernameRegisterField.text == "")
        {
            warningRegisterText.text = "Usuário faltando";
            return true;
        }
        else if(passwordRegisterField.text != verifyPasswordRegisterField.text)
        {
            warningRegisterText.text = "Verificação de senha inválida";
            return true;
        }
        else
        {
            return false;
        }
    }

    void HandleRegisterErrors(System.AggregateException registerException)
    {
        Debug.LogWarning(message: $"Failed to register task with {registerException}");
        FirebaseException firebaseEx = registerException.GetBaseException() as FirebaseException;
        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

        warningRegisterText.text = DefineRegisterErrorMessage(errorCode);
    }

    string DefineRegisterErrorMessage(AuthError errorCode)
    {
        switch (errorCode)
        {
            case AuthError.MissingEmail:
                return "Email faltando";
            case AuthError.MissingPassword:
                return "Senha faltando";
            case AuthError.WeakPassword:
                return "Senha muito fraca";
            case AuthError.InvalidEmail:
                return "Email inválido";
            case AuthError.EmailAlreadyInUse:
                return "Email já cadastrado";
            default:
                return "Registro falhou por motivos desconhecidos";
        }
    }

    private IEnumerator RegisterUser(System.Threading.Tasks.Task<Firebase.Auth.FirebaseUser> registerTask, string displayName, string email, string password)
    {
        FirebaseAuthenticator.instance.User = registerTask.Result;

        if (FirebaseAuthenticator.instance.User != null)
        {
            UserProfile profile = new UserProfile { DisplayName = displayName };
            var ProfileTask = FirebaseAuthenticator.instance.User.UpdateUserProfileAsync(profile);
            yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

            if(ProfileTask.Exception != null)
            {
                HandleProfileCreationErrors(ProfileTask.Exception);
            }
            else
            {
                ChangeUI.instance.ChangeBetweenLoginAndRegister();
                
                GetComponent<LoginAuth>().emailInputField.text = email;
                GetComponent<LoginAuth>().passwordInputField.text = password;
                //GetComponent<LoginAuth>().LoginButton();
            }
        }
    }

    void HandleProfileCreationErrors(System.AggregateException profileException)
    {
        Debug.LogWarning(message: $"Failed to register task with {profileException}");
        FirebaseException firebaseEx = profileException.GetBaseException() as FirebaseException;
        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
        warningRegisterText.text = "Falha ao registrar o usuário";
    }
}

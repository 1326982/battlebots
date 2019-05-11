/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
// Nom de la classe:
// Fonctionnement de la classe:
// notes:
// bugs:
// todo:
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour {
    [SerializeField] private GameObject loginBox;
    [SerializeField] private GameObject signUpBox;

    [SerializeField] private Text errorTxt;

    [SerializeField] private InputField usernameSign;
    [SerializeField] private InputField emailSign;
    [SerializeField] private InputField passwordSign;
    [SerializeField] private InputField passwordConfirmSign;

    [SerializeField] private InputField usernameLogin;
    [SerializeField] private InputField passwordLogin;

    public void toLogin() {
        signUpBox.SetActive(false);
        loginBox.SetActive(true);

    }

    public void toSignUp() {
        signUpBox.SetActive(true);
        loginBox.SetActive(false);
    }

    public void signUp(){
        string username = usernameSign.text;
        string email = emailSign.text;
        string password = passwordSign.text;
        string passwordConfirm = passwordConfirmSign.text;

        if(username.Length >0) {
            if(email.Length >0) {
                if(password.Length >0) {
                    if(password.Length >= 8) {
                        if(passwordConfirm == password) {
                            string query = "&action=signup&username="+username+"&password="+password+"&email="+email;
                            StartCoroutine(DatabaseManager.instance.Query(processSignUp,query ));
                        }else {
                            errorTxt.text = "Password confirmation and password must be the same.";
                        }
                    }else {
                        errorTxt.text = "Password must be 8 character or longer.";  
                    }
                }else {
                    errorTxt.text = "Please enter a password.";
                }
            }else {
                errorTxt.text = "Please enter your email.";        
            }
        }else {
            errorTxt.text = "Please enter a username.";            
        }
    }

    private void processSignUp(string response) {
        print(response);
        HttpGeneric token = JsonUtility.FromJson<HttpGeneric>(response);
        if(token.error == "ok"){
            usernameLogin.text = usernameSign.text;
            passwordLogin.text = passwordSign.text;
            login();
        }else {
            errorTxt.text = token.error;
        }
    }

    public void login(){
        string username = usernameLogin.text;
        string password = passwordLogin.text;
        if(username.Length >0) {
            if(password.Length >0){
                string query = "&action=login&username="+ username +"&password=" + password;
                StartCoroutine(DatabaseManager.instance.Query(processLogin, query));
            }else {
                errorTxt.text = "Please Enter your password";
            }
        }else {
            errorTxt.text = "Please enter your username.";
        }
    }

    private void processLogin(string response) {
        print(response);
        HttpLogin token = JsonUtility.FromJson<HttpLogin>(response);
        if(token.error == "ok"){
            PlayerPrefs.SetString("apiKey",token.DeviceToken);
            PlayerPrefs.SetString("usersID",token.usersID);
            PlayerPrefs.SetString("username",token.username);
            GameManager.instance.UserUsername = token.username;
            GameManager.instance.changeScene(Scenes.Auth);
        }else {
            errorTxt.text = token.error;
        }
    }

    


}

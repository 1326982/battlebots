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

public class AuthManager : MonoBehaviour {
    IEnumerator Start() {
        if(!checkAuthPrefsExists()){
            GameManager.instance.changeScene(Scenes.Login);
        }else {
            string query = "&action=keyAuth&token="+PlayerPrefs.GetString("apiKey","default")+"&username="+PlayerPrefs.GetString("username","default");
            StartCoroutine(DatabaseManager.instance.Query(apiKeyCheckResult, query));
        }
        
        yield return null;
    }

    void apiKeyCheckResult(string response){
        HttpGeneric authState = JsonUtility.FromJson<HttpGeneric>(response);
        if(authState.error == "ok"){
            GameManager.instance.UserUsername = PlayerPrefs.GetString("username");
            StartCoroutine("loadData");
        }else {
            GameManager.instance.changeScene(Scenes.Login);
        }
    }

    private bool checkAuthPrefsExists(){
        string[] keys = new string[]{"username","apiKey","usersID"};
        foreach (string key in keys) {
            if(!PlayerPrefs.HasKey(key)){
               return false; 
            } 
        }
        return true;
    }

    private IEnumerator loadData(){
        yield return StartCoroutine(DatabaseManager.instance.loadBots());
        yield return StartCoroutine(DatabaseManager.instance.loadPlayer());
        GameManager.instance.changeScene(Scenes.MainMenu);
        yield return null;
    }

}

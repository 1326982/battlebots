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

public class AuthManager : MonoBehaviour {
    [SerializeField] private Slider sliderLoad;
    [SerializeField] private UIFader content;

    IEnumerator Start() {
        sliderLoad.value = 10;
        if(!checkAuthPrefsExists()){
            StartCoroutine(changeScenefade(Scenes.Login));
        }else {
            string query = "&action=keyAuth&token="+PlayerPrefs.GetString("apiKey","default")+"&username="+PlayerPrefs.GetString("username","default");
            StartCoroutine(DatabaseManager.instance.Query(apiKeyCheckResult, query));
            sliderLoad.value = 30;
        }
        
        yield return null;
    }

    private IEnumerator changeScenefade(Scenes scene){
        sliderLoad.value = 100;
        content.Fade(FadeTransition.Out);
        yield return new WaitForSeconds(0.5f);
        GameManager.instance.changeScene(scene);
        yield return null;
    }

    void apiKeyCheckResult(string response){
        sliderLoad.value = 70;
        HttpGeneric authState = JsonUtility.FromJson<HttpGeneric>(response);
        if(authState.error == "ok"){
            GameManager.instance.UserUsername = PlayerPrefs.GetString("username");
            StartCoroutine("loadData");
        }else {
            StartCoroutine(changeScenefade(Scenes.Login));
        }
    }

    private bool checkAuthPrefsExists(){
        sliderLoad.value = 20;
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
        StartCoroutine(changeScenefade(Scenes.MainMenu));
        yield return null;
    }

}

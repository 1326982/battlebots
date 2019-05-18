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
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneManager : MonoBehaviour {

    private float _progress = 0f;
    [SerializeField] Slider sliderProgress;

    void Start() {
        if(GameManager.instance.SceneToLoad == Scenes.Battle){
            StartCoroutine(loadBattle(GameManager.instance.SceneToLoad));
        }else if(GameManager.instance.SceneToLoad == Scenes.MainMenu){
            StartCoroutine(loadMenu(GameManager.instance.SceneToLoad));
        }
    }

    IEnumerator loadBattle (Scenes scene) {
        AsyncOperation ao = SceneManager.LoadSceneAsync(scene.ToString());
        ao.allowSceneActivation = false;
        bool ready = false;
        while (!ready) {
            yield return new WaitForSeconds(0.1f);
            // [0, 0.9] > [0, 1]
            bool[] battleLoad = GameManager.instance.BattlePrep;
            float tempProgress = 0f;
            tempProgress += ao.progress / 0.9f *25f;
            foreach(bool item in battleLoad) {if(item){tempProgress+=25f;}}
            _progress = tempProgress;
            sliderProgress.value = _progress;
            // Loading completed
            if (_progress>90f) {
                ao.allowSceneActivation = true;
            }
            
            yield return null;
        }
    }
    IEnumerator loadMenu (Scenes scene) {
        AsyncOperation ao = SceneManager.LoadSceneAsync(scene.ToString());
        ao.allowSceneActivation = false;
        bool ready = false;
        while (!ready) {
            yield return new WaitForSeconds(0.1f);
            // [0, 0.9] > [0, 1]
            float tempProgress = 0f;
            tempProgress += ao.progress / 0.9f *25f;
            _progress = tempProgress;
            sliderProgress.value = _progress;
            // Loading completed
            if (GameManager.instance.MenuReady) {
                ao.allowSceneActivation = true;
            }
            
            yield return null;
        }
    }

}

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
        StartCoroutine(AsynchronousLoad(GameManager.instance.SceneToLoad));
    }

    IEnumerator AsynchronousLoad (Scenes scene) {
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
            print(battleLoad[0]+" "+battleLoad[1]+" "+battleLoad[2]+" "+(ao.progress/0.9f*25f));
            Debug.Log("Loading progress: " + (_progress) + "%");
            // Loading completed
            if (_progress>90f) {
                ao.allowSceneActivation = true;
            }
            
            yield return null;
        }
    }

}

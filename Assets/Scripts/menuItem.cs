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
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class menuItem : MonoBehaviour
{
    [SerializeField] public Text texteItem;
    [SerializeField] public RawImage imageItem;
    private RectTransform _rect;
    public string callback;
    public string nomPiece;

    private void Start() {
        _rect = GetComponent<RectTransform>();
    }

    public void btnAction() {
        if(nomPiece != "") {MenuManager.instance.MenuParameters = nomPiece;}
        StartCoroutine("animationClique");
        MenuManager.instance.Invoke( "call"+callback, 0f);
    }
    private void Update() {
        if(gameObject == MenuManager.instance.events.currentSelectedGameObject ){
             EventSystem.current.SetSelectedGameObject(null);
             
             btnAction();
        }
    }

    private IEnumerator animationClique(){
        _rect.localScale = new Vector3(1,1,1);
        //phase GRRRROOOOOOWWWW
        while(_rect.localScale.x <= 1.4f){
            float x = _rect.localScale.x + 0.1f;
            float y = _rect.localScale.y + 0.1f;
            float z = _rect.localScale.z + 0.1f;
            _rect.localScale = new Vector3(x,y,z);
            yield return new WaitForSeconds(0.01f);
        }
        while(_rect.localScale.x >= 1){
            float x = _rect.localScale.x - 0.05f;
            float y = _rect.localScale.y - 0.05f;
            float z = _rect.localScale.z - 0.05f;
            _rect.localScale = new Vector3(x,y,z);
            yield return new WaitForSeconds(0.01f);
        }
        yield return null;
    }
}   

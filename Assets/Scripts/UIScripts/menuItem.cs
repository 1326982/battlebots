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
    [SerializeField] public GameObject lockFilter;
    [SerializeField] public Text lockText;
    private RectTransform _rect;
    public string callback;
    public string nomPiece;
    private float initialSize;
    private float bigSize;


    private void Start() {
        _rect = GetComponent<RectTransform>();
        initialSize = _rect.localScale.x;
        bigSize = initialSize+((initialSize/100)*40);

    }

    public void btnAction() {
        if(nomPiece != "") {
            MenuManager.instance.MenuParameters = nomPiece;
            MenuManager.instance.Invoke( "call"+callback, 0f);}
        else {
            StartCoroutine("animationClique");
        }
        
    }
    private void Update() {
        if(gameObject == MenuManager.instance.events.currentSelectedGameObject ){
             EventSystem.current.SetSelectedGameObject(null);
             
             btnAction();
        }
    }

    private IEnumerator animationClique(){
        //_rect.localScale = new Vector3(1,1,1);
        //phase GRRRROOOOOOWWWW
        while(_rect.localScale.x <= bigSize){
            float x = _rect.localScale.x + 0.5f;
            float y = _rect.localScale.y + 0.5f;
            float z = _rect.localScale.z + 0.5f;
            _rect.localScale = new Vector3(x,y,z);
            yield return new WaitForSeconds(0.01f);
        }
        while(_rect.localScale.x >= initialSize){
            float x = _rect.localScale.x - 0.25f;
            float y = _rect.localScale.y - 0.25f;
            float z = _rect.localScale.z - 0.25f;
            _rect.localScale = new Vector3(x,y,z);
            yield return new WaitForSeconds(0.01f);
        }
        MenuManager.instance.Invoke( "call"+callback, 0f);
        yield return null;
    }
}   

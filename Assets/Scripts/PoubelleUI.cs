using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class PoubelleUI : MonoBehaviour
{
    public bool isSelecting = false;
    public int fingerid = 0;
    private bool inAnimation=false;
    private RectTransform _rect;
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    

    private void Start() {
        _rect = GetComponent<RectTransform>();
        m_Raycaster = GetComponent<GraphicRaycaster>();
        StartCoroutine(animationDelete(false));
    }
    private void OnEnable() {
        if(_rect){
            StartCoroutine(animationDelete(false));
        }
        
    }

    // Update is called once per frame
    void Update() {
        if(isSelecting){
            Debug.Log("debug is selecting sur le finger id: "+fingerid);
            if(EventSystem.current.IsPointerOverGameObject(fingerid)){
                Debug.Log("le pointer est sur le canvas");
                m_PointerEventData = new PointerEventData(EventSystem.current);
                
                m_PointerEventData.position = (Vector2)findTouchPosition();
                List<RaycastResult> results = new List<RaycastResult>();
                m_Raycaster.Raycast(m_PointerEventData, results);
                Debug.Log(results);
                foreach (RaycastResult result in results) {
                    Debug.Log("Hit " + result.gameObject.name);
                    if(result.gameObject == gameObject){
                        MenuManager.instance.trashCanShortcut();
                        isSelecting=false;
                        if(!inAnimation){StartCoroutine(animationDelete(true));}
                    }
                }



                
            }
        }
        
    }
    private Vector2? findTouchPosition(){
        for(int i = 0;i<Input.touchCount;i++){
            if(Input.GetTouch(i).fingerId == fingerid) {
                Debug.Log(Input.GetTouch(i).position);
                return Input.GetTouch(i).position;
            }
        }
        Debug.Log("pointernull");
        return null;
    }

    private IEnumerator animationDelete(bool willDisable){
        inAnimation = true;
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
        if(willDisable){
            this.gameObject.SetActive(false);
        }else {
            inAnimation = false;
        }
        yield return null;
    }


}

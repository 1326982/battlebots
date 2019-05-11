using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Modal : MonoBehaviour {
    private bool choice;
    private bool interacted = false;
    [SerializeField] private Text texte;
    [SerializeField] private Text txtOptionNon;
    [SerializeField] private Text txtOptionOui;
    
    public void accept() {
        choice = true;
        interacted = true;
    }

    public void cancel(){
        choice = false;
        interacted = true;
    }

    public void init(string m, Action sucessFunction, string optionOui = "Yes", string optionNon = "No", Action cancelFunction = null ){
        txtOptionNon.text = optionNon;
        txtOptionOui.text = optionOui;
        texte.text = m;
        StartCoroutine(execModal(sucessFunction,cancelFunction));
    }


    public IEnumerator execModal(Action sucessFunction,Action cancelFunction) {
        while(interacted == false) {
            yield return new WaitForSeconds(0.2f);
        }
        if(choice == true){
            sucessFunction();
        }else{
            if(cancelFunction!=null){cancelFunction();}
        }
        
        Destroy(gameObject);
        yield return null;

    }
}

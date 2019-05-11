using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour {
    private bool coroutineWorking = false;
    [SerializeField] GameObject hammerHead;
    private int nbframesDmg = 10;

    private IEnumerator mouvement(){
        while(transform.localRotation.x <0.4f) {
            transform.Rotate(6f,0,0);
            if(transform.localRotation.x <0.3f){
                hammerHead.tag = "Untagged";
            }else {
                hammerHead.tag = "weaponCollider";
            }
            yield return 0;
        }
        StartCoroutine("activateDamage");
        while(transform.localRotation.x > 0f) {
            transform.Rotate(-0.8f,0,0);
            yield return 0;

        }
        yield return new WaitForSeconds(1);
        coroutineWorking = false;
        
    }

    private IEnumerator activateDamage(){
        hammerHead.tag = "weaponCollider";
        for(int i =0;i<nbframesDmg;i++){
            yield return 0;
        }
        hammerHead.tag = "Untagged";
        yield return null;
    }
    
    private void OnTriggerStay(Collider other) {
        if(other.gameObject.name != "Plancher" ) {
            if(!coroutineWorking) {
                coroutineWorking = true;
                StartCoroutine("mouvement");
            }
        }
    }
}

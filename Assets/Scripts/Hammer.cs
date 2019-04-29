using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour {
    private bool coroutineWorking = false;

    private IEnumerator mouvement(){
        while(transform.localRotation.x <0.4f) {
            transform.Rotate(5f,0,0);
            print(transform.rotation.x);
            yield return new WaitForSeconds(0.01f);
        }
        while(transform.localRotation.x > 0f) {
            transform.Rotate(-1f,0,0);
            yield return new WaitForSeconds(0.01f);

        }
        coroutineWorking = false;
        
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

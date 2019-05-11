using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flipper : MonoBehaviour
{
    private bool isFliping = false;
    private void Start() {
        transform.localRotation= Quaternion.Euler(Vector3.zero);
    }


    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.name != "Plancher" ) {
            if(!isFliping){
                StartCoroutine("Flip");
            }
        }
    }

    private IEnumerator Flip(){
        isFliping = true;
        gameObject.tag = "weaponCollider";
        while(transform.localRotation.x > -0.5f ){
            transform.Rotate(-5f,0,0);
            yield return 0;
        }
        while(transform.localRotation.x<0){
            transform.Rotate(5f,0,0);
            yield return 0;
        }
        transform.localRotation= Quaternion.Euler(Vector3.zero);
        isFliping=false;
        gameObject.tag = "Untagged";
        yield return new WaitForSeconds(3f);
        yield return null;
    }
    
}

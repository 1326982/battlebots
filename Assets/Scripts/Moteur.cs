using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moteur : MonoBehaviour {
    private bool  activated = false;
    private float vitesse;
    private Rigidbody _rbodyParent;
    private float velocite = 0;
    private float acceleration = 0.1f;

    private void Start() {
        _rbodyParent = transform.parent.GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update() {
        if(activated) {
            transform.Rotate(0,_rbodyParent.velocity.magnitude,0);
        if(velocite<vitesse){
            velocite+=acceleration;
        } else {
            velocite-= acceleration;
            if(velocite <0){velocite=0;};
        }
        if(transform.position.y < 1.5f) {
            _rbodyParent.AddForce(new Vector3(transform.parent.transform.forward.x,0.2f,transform.parent.transform.forward.z)*velocite);
        }
    }
        
        
    }
    public float SetVitesse {
        set{vitesse = value;}
    }
    public void activate(){
        activated = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moteur : MonoBehaviour {
    private bool  activated = false;
    private float vitesse;
    private Rigidbody _rbodyParent;
    private float velocite = 0;
    private float acceleration = 0.1f;
    private float minDist;
    private float toleranceDist = 0.02f;
    [SerializeField] LayerMask layer;

    private void Start() {
        _rbodyParent = transform.parent.GetComponent<Rigidbody>();
        Transform memoireParent = transform.parent;
        transform.parent = null;
        float worldScaleZ = transform.localScale.z;
        transform.parent=memoireParent;
        minDist = (GetComponent<MeshFilter>().mesh.bounds.size.z/2*worldScaleZ)+toleranceDist;
    }
    // Update is called once per frame
    void Update() {
         Debug.DrawLine(transform.position,new Vector3(transform.position.x,transform.position.y-minDist,transform.position.z),Color.green,0.1f);
         RaycastHit hit;
        bool testdist = Physics.Linecast(transform.position, new Vector3(transform.position.x,transform.position.y-minDist,transform.position.z),out hit,layer);

        if(testdist){Debug.Log(hit.transform.gameObject.name);};
        if(activated) {
            transform.Rotate(0,_rbodyParent.velocity.magnitude,0);
            if(velocite<vitesse && testdist){
                velocite+=acceleration;
            } else {
                velocite-= acceleration*2;
                if(velocite <0){velocite=0;};
            }
            if(testdist) {
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

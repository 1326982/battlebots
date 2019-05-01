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

public class Destroyable : MonoBehaviour    {
    // Start is called before the first frame update
    
    [SerializeField] private float health;
    [SerializeField]Component[] componentAdetruire;
    [SerializeField]Destroyable[] bringDownWithIt;
    Collider _collider;
    private bool isCurrentlyDamaged = false;

    
    public void endommager(float valeur){
        if(!isCurrentlyDamaged){
            isCurrentlyDamaged = true;
            health= health-(valeur);
        // Debug.Log((valeur/60) +" damage on " + gameObject.name + " by " + giver);
        //Debug.Log("je recois "+valeur+" dommages");
            if(health <= 0) {
                foreach(Component item in componentAdetruire) {
                    Destroy(item);
                }
                gameObject.tag = "Destroyed";
                gameObject.transform.parent = null;
                if(gameObject.GetComponent<Rigidbody>() == null) {
                    gameObject.AddComponent<Rigidbody>();
                }
                gameObject.GetComponent<Rigidbody>().mass = 0.0001f;
                //GetComponent<Rigidbody>().AddForce(Vector3.up / 0.2f, ForceMode.Impulse);
                foreach(Destroyable item in bringDownWithIt){
                    if(item != null) {
                        item.endommager(10000);
                    }
                    
                }
                Destroy(this);

            }
            isCurrentlyDamaged = false;
        }
            
    }

    public float HP {
        get{return health;}
    }

}

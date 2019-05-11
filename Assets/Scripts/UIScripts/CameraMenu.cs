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

public class CameraMenu : MonoBehaviour {

[SerializeField] private GameObject _cible;
private Camera cam;
Vector2 touchpos;
public bool blockCam = false;
public int fingeridTouchMove = 20;




void Start() {
    cam = GetComponent<Camera>();
}

void Update() {
    if(Input.GetAxis("Horizontal") >0 || Input.GetAxis("Horizontal") <0){
        transform.RotateAround(_cible.transform.position,new Vector3(0,1,0),Input.GetAxis("Horizontal") );
    }
    if(Input.GetAxis("Vertical") >0 || Input.GetAxis("Vertical") <0){
        if(Input.GetKey(KeyCode.Space)) {
            if(Vector3.Distance(transform.position,_cible.transform.position) > 0.5) {
                transform.position = Vector3.MoveTowards(transform.position,_cible.transform.position,Input.GetAxis("Vertical")/20);
            }
            if(Vector3.Distance(transform.position,_cible.transform.position) < 0.5 && Input.GetAxis("Vertical") <0) {
                transform.position = Vector3.MoveTowards(transform.position,_cible.transform.position,Input.GetAxis("Vertical")/20);
            }
        } else {
            Vector3 localX = transform.TransformDirection(Vector3.right);
            transform.RotateAround(_cible.transform.position,localX,Input.GetAxis("Vertical") );
        }
    }
    if(!blockCam) {
        if(Input.touchCount ==1){
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) {
                touchpos = touch.position;
                fingeridTouchMove = touch.fingerId;
                MenuManager.instance.SetClickPieces = true;
            }

            if (touch.phase == TouchPhase.Moved && touch.fingerId == fingeridTouchMove) {
                Vector2 pos = touch.position;
                if(pos.x != touchpos.x){
                    transform.RotateAround(_cible.transform.position,new Vector3(0,1,0),-(touchpos.x-pos.x)/22 );
                }
                if(pos.y != touchpos.y){
                    Vector3 localX = transform.TransformDirection(Vector3.right);
                    transform.RotateAround(_cible.transform.position,localX,(touchpos.y-pos.y)/22);
                }
                touchpos = pos; 
            }
            if(touch.phase == TouchPhase.Ended && touch.fingerId == fingeridTouchMove) {
                fingeridTouchMove = 20;
                MenuManager.instance.SetClickPieces = false;
            }
            
        } else if (Input.touchCount == 2) {
            Camera camera = GetComponent<Camera>();
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
                // Otherwise change the field of view based on the change in distance between the touches.
                camera.fieldOfView += deltaMagnitudeDiff * 0.1f;

                // Clamp the field of view to make sure it's between 0 and 180.
                camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 20, 100);
        }
    }else {
        fingeridTouchMove = 20;
    }
}



}

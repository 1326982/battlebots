using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellNotifInfo : MonoBehaviour {

    [SerializeField] private Text txtMessage;
    [SerializeField] private GameObject friendsBtnGroup;
    [SerializeField] private HttpNotification infos;

    public void init(HttpNotification infoQuery){
        infos = infoQuery;
        Debug.Log(infos.id);
        txtMessage.text = infos.message;
        chooseBtn();

    }
    public void chooseBtn(){
        switch(infos.type) {
            case "friendRequest":
            friendsBtnGroup.SetActive(true);
            break;
        }
    }
    public void acceptRequest() {
        StartCoroutine(DatabaseManager.instance.friendRequest(infos.otherID,infos.id));

    }
    public void declineRequest(){
        StartCoroutine(DatabaseManager.instance.deleteFriend(int.Parse(infos.param),infos.id));
    }
}

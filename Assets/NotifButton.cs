using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotifButton : MonoBehaviour
{
    [SerializeField] private Text notificationCountText;
    [SerializeField] private GameObject notificationPastille;

    // Start is called before the first frame update
    void Start()
    {
        getNotificationCount();
    }

    public void getNotificationCount(){
        string query= "&action=getNotificationsCount&usersID="+PlayerPrefs.GetString("usersID");
        StartCoroutine(DatabaseManager.instance.Query(showNotificationCount,query));
    }
    public void showNotificationCount(string count){
        if(count == "0"){
            notificationPastille.SetActive(false);
        }else{
            notificationCountText.text = count;
            notificationPastille.SetActive(true);
        }
        
    }
}

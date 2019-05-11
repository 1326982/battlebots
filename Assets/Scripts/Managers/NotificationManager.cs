using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager instance;
    [SerializeField] private GameObject cellNotif;
    [SerializeField] private GameObject displayNotif; 
    // Start is called before the first frame update
    private void OnEnable() {
        refreshDisplay();
    }

    void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(this.gameObject);
        }
    }
    private void refreshDisplay() {
        emptyContent();
        print("getNotifications");
        string query = "&action=getNotifications&usersID="+ PlayerPrefs.GetString("usersID");
        StartCoroutine(DatabaseManager.instance.Query(showResults,query));
    }
    private void showResults(string response){
        HttpNotificationWrapper notifs = JsonUtility.FromJson<HttpNotificationWrapper>(response);
        foreach(HttpNotification notif in notifs.wrap){
            GameObject cell = Instantiate(cellNotif) as GameObject;
            cell.GetComponent<RectTransform>().SetParent(displayNotif.transform,false);
            cell.GetComponent<CellNotifInfo>().init(notif);
        }
        
    }
    private void emptyContent(){
        var children = new List<GameObject>();
        foreach (Transform child in displayNotif.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
    }
    public void asyncRefresh(string x){
        refreshDisplay();
    }


}

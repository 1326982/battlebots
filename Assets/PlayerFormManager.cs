using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI ;
using System.Text.RegularExpressions;

public class PlayerFormManager : MonoBehaviour
{
    public static PlayerFormManager instance;
    [SerializeField] private GameObject displayContent;
    [SerializeField] private GameObject perfabListCell;
    [SerializeField] private InputField champRecherche;

    private DisplaySection section = DisplaySection.Friends;
    private int browsePage=1;


    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable() {
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
        print("getting Friends");
        string query = "";
        switch(section){
            case DisplaySection.Friends:
                query = "&action=getFriends&usersID="+ PlayerPrefs.GetString("usersID");
            break;
            case DisplaySection.Browse:
                query = "&action=getAllUsers&usersID="+ PlayerPrefs.GetString("usersID");
            break;
            case DisplaySection.Search:
                query = "&action=searchUser&usersID="+ PlayerPrefs.GetString("usersID")+"&search="+MySqlEscape("%"+champRecherche.text.Replace(" ","%")+"%");
            break;
        }
        StartCoroutine(DatabaseManager.instance.Query(showResults,query));
    }
    private void showResults(string response){
        HttpOpponentWrapper users = JsonUtility.FromJson<HttpOpponentWrapper>(response);
        foreach(HttpOpponent user in users.wrap){
            GameObject cell = Instantiate(perfabListCell) as GameObject;
            cell.GetComponent<RectTransform>().SetParent(displayContent.transform,false);
            cell.GetComponent<CellInfo>().init(user.level,user.rank,user.username,user.id,user.mutualFriend,user.friendshipId,user);
        }
        
    }
    private string MySqlEscape(string usString) {
    if (usString == null)
    {
        return null;
    }
    return Regex.Replace(usString, @"[\r\n\x00\x1a\\'""]", @"\$0");
}
    public void recherche(){
        if(champRecherche.text != ""){
            section = DisplaySection.Search;
            refreshDisplay();
        }
        
    }
    public void browseAll(){
        section = DisplaySection.Browse;
        refreshDisplay();
    }
    public void showFriends(){
        section = DisplaySection.Friends;
        refreshDisplay();
    }

    private void emptyContent(){
        var children = new List<GameObject>();
        foreach (Transform child in displayContent.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
    }
}
public enum DisplaySection {
    Friends, Browse, Search
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellInfo : MonoBehaviour
{
    [SerializeField] Text txtLevel;
    [SerializeField] Text txtRanking;
    [SerializeField] Text txtUsername;
    [SerializeField] GameObject btnBattle;
    [SerializeField] GameObject btnAddFriend;
    [SerializeField] GameObject btnRemoveFriend;
    private bool mutualFriend;
    private int friendshipID = 0;
    private int userID; 

    public void init(int level, int ranking, string username, int userid, bool mutual, int friendshipid ){
        txtLevel.text = level.ToString();
        txtRanking.text = "Rank "+ranking;
        txtUsername.text = username;
        userID = userid;
        mutualFriend = mutual;
        friendshipID = friendshipid;
        buttonManaging();
    }
    public void buttonManaging(){
        if(mutualFriend){
            btnBattle.SetActive(true);
        }
        if(friendshipID == 0){
            btnAddFriend.SetActive(true);
        }else {
            btnRemoveFriend.SetActive(true);
        }
    }
    
    public void launchBattle() {
        
    }
    public void askFriendship() {

    }
    public void deleteFriendship(){

    }
}

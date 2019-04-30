/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
// Nom de la classe: DatabaseManager
// Fonctionnement de la classe: Gère les query a la base de données.
// notes:
// bugs:
// todo:
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class DatabaseManager : MonoBehaviour {
    public static DatabaseManager instance;

    private string url = "https://battlebots.antoinebernier.com/battlebots.php?key=uhtuo62d3nei8037mwe8vyko8r77ek161jfs7w58cdb3sdp07peoekjyce5co3zv9avl2ypdxfmzm25hqbkus0u5elbesh41afsh30ckgvf2ye867qj6cusz9yrtppdr";

    void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(instance);
        } else {
            Destroy(this.gameObject);
        }
    }

    public IEnumerator Query(Action<string> sucessFunction ,string query) {
        WWW www = new WWW(url+query);
        yield return www;
        print(www.text);
        sucessFunction(www.text);
        yield return null;

    }

    public IEnumerator loadPlayer(){
        print("playerLoading");
        string query = "&action=getUserInfo&usersID="+ PlayerPrefs.GetString("usersID");
        yield return StartCoroutine(DatabaseManager.instance.Query(readPlayer,query));
        yield return null;
    }
    private void readPlayer(string response) {
        HttpUserInfo userInfo = JsonUtility.FromJson<HttpUserInfo>(response);
        GameManager.instance.SetUserInfo(userInfo);
        
    }

    public IEnumerator loadBots(){
        string query = "&action=loadBots&usersID="+ PlayerPrefs.GetString("usersID");
        yield return StartCoroutine(DatabaseManager.instance.Query(readbots,query));
        yield return null;
    }
    public IEnumerator loadBotsEdit(){
        string query = "&action=loadBots&usersID="+ PlayerPrefs.GetString("usersID");
        yield return StartCoroutine(DatabaseManager.instance.Query(readbotsEdit,query));
        yield return null;
    }


    private void readbots(string response){
        HttpBotsWrapper bots = JsonUtility.FromJson<HttpBotsWrapper>(response);
        GameManager.instance.LoadedBots = buildBotArray(true, bots);
    }
    private void readbotsEdit(string response){
        HttpBotsWrapper bots = JsonUtility.FromJson<HttpBotsWrapper>(response);
        GameManager.instance.LoadedBots = buildBotArray(false, bots);
        MenuManager.instance.spawnbotEdit();
    }

    private BotBuilder[] buildBotArray(bool replaceSelected, HttpBotsWrapper bots){
        BotBuilder[] botArrayTmp = new BotBuilder[3] {new BotBuilder(),new BotBuilder(),new BotBuilder()};

        foreach(HttpBots botInfo in bots.wrap) {
            int id = int.Parse(botInfo.botsSlot);
            BotBuilder botAct = botArrayTmp[id];
            botAct.platform = botInfo.botsPlatform;
            botAct.botID = botInfo.botsId;
            botAct.rotationSpeed = float.Parse(botInfo.botsRotationSpeed);
            botAct.speed = float.Parse(botInfo.botsSpeed);
            botAct.weight = float.Parse(botInfo.botsWeight);
            string jsonAnchorPrepared = "{\"botsAnchorInfo\":" + botInfo.botsAnchorInfo + "}" ;
            JsonAnchorWrapper listPartsJson = JsonUtility.FromJson<JsonAnchorWrapper>(jsonAnchorPrepared);
            botAct.listParts = new AnchorInfo[listPartsJson.botsAnchorInfo.Length];
            int i =0;
            foreach(JsonAnchor jsonAnchor in listPartsJson.botsAnchorInfo) {
                botAct.listParts[i] = new AnchorInfo();
                AnchorInfo AnchorAct = botAct.listParts[i];
                AnchorAct.anchorPos = jsonAnchor.anchorPos;
                AnchorAct.itemName = jsonAnchor.itemName;
                i++;
            }
            if(replaceSelected) {
                if(botInfo.botsPrefered == "true"){
                    PlayerPrefs.SetInt("botsPrefered",id);
                    GameManager.instance.SelectedBotIndex = id;
                }
            }
        }
        return botArrayTmp;
    }


}

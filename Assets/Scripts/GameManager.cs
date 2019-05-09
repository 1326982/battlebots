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
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    
    public static GameManager instance;


    /*Liste des sceneries */
    [Header("Sceneries")]
    [SerializeField] private string[] keysSceneries;
    [SerializeField] private GameObject[] valuesSceneries;
    private Dictionary<string, GameObject> sceneryList;
    [Header("-----------------")]
    /*listes des sceneneries end */

    /*Info actual player */
    private UserInfo user = new UserInfo();
    /**/
    /*Bots loaded at Auth */
    private BotBuilder[] myBots = new BotBuilder[3] {new BotBuilder(),new BotBuilder(),new BotBuilder()};
    private int selectedBot = 0;
    /**/

    
    private BotBuilder opponent;
    private int opponentBotID;
    private HttpOpponent nextOpponent;
    private bool localLoaded = false;
    private bool opponentLoaded = false;
    private bool battleReady = false;
    private BattleSettings battleSettings;
    private Scenes asyncSceneLoad;


    // Start is called before the first frame update
    void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(instance);
            sceneryList = loadDictionnary(keysSceneries,valuesSceneries);
            Application.targetFrameRate = 60;
        } else {
            Destroy(this.gameObject);
        }
    }

    public void startBattle(string battleOnOff) {
        if(battleOnOff ==  "online") {
            StartCoroutine(loadBattleRoutine(BattleType.OnlineQuick, opponentBotID)); 
        }else if(battleOnOff == "offline") {
            StartCoroutine(loadBattleRoutine(BattleType.Offline)); 
        }else if(battleOnOff == "onlineFriend") {
            StartCoroutine(loadBattleRoutine(BattleType.OnlineSetup)); 
        }
    }

    private Dictionary<string, GameObject> loadDictionnary( string[] keys, GameObject[] values) {
        Dictionary<string, GameObject> tempDictionnary = new Dictionary<string, GameObject>{};
        for(int i =0;i<keys.Length;i++) {
            tempDictionnary.Add(keys[i],values[i]);
        }
        return tempDictionnary;
    }

    public void changeScene(Scenes sceneName) {
          SceneManager.LoadScene(sceneName.ToString());
    }

    private IEnumerator loadBattleRoutine(BattleType type, int opponentId = 0) {
        asyncSceneLoad = Scenes.Battle;
        SceneManager.LoadScene(Scenes.sceneLoader.ToString());
        yield return StartCoroutine(presetBattle(type,opponentId));
        battleReady = true;
        yield return null;
    }

    private IEnumerator presetBattle(BattleType type , int idBotOpponent = 0) {
        battleSettings = new BattleSettings();
        if(type == BattleType.Offline){
            yield return StartCoroutine(generateOpponent("singleplayer"));
            while(!opponentLoaded) {
                yield return new WaitForSeconds(0.5f);
            }
            battleSettings.visitorbot = opponent;
        }else if(type == BattleType.OnlineQuick) {
            yield return StartCoroutine(generateOpponent("multiplayer"));
            while(!opponentLoaded) {
                yield return new WaitForSeconds(0.5f);
            }
            battleSettings.visitorbot = opponent;
        }else if (type == BattleType.OnlineSetup){
            yield return StartCoroutine(generateOpponent("friendly"));
            while(!opponentLoaded) {
                yield return new WaitForSeconds(0.5f);
            }
            battleSettings.visitorbot = opponent;
        }
        yield return battleSettings.localbot = GetActiveBot;
        localLoaded = true;
        yield return battleSettings.Scenery = sceneryList["Plain"];
        yield return new WaitForSeconds(0.5f);
        yield return null;
    }

    private IEnumerator generateOpponent(string typeOpponent) {
        string query = "";
        switch(typeOpponent){
            case "singleplayer":
                query = "&action=loadBotComp";
            break;
            case "multiplayer":
                query = "&action=loadRandomBot&username="+user.username+"&userRank="+user.userClassement;
            break;
            case "friendly":
                query = "&action=loadBotWithUser&opponentID="+nextOpponent.id;
            break;
        }
        yield return StartCoroutine(DatabaseManager.instance.Query(loadOpponent,query));
        yield return null;
    }
    private IEnumerator fetchOpponent(int botID) {
        opponent = myBots[2];
        return null;
    }
    public void loadOpponent(string response) {
        HttpBots botloaded = JsonUtility.FromJson<HttpBots>(response);
        opponent = fillBotbuilder(botloaded);
        opponentLoaded = true;
    }
    private BotBuilder fillBotbuilder(HttpBots botInfo) {
        BotBuilder botAct = new BotBuilder();
        botAct.platform = botInfo.botsPlatform;
        botAct.botID = botInfo.botsId;
        botAct.botsName = botInfo.botsName;
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
        return botAct;
    }

    /*Interragir avec le gameManager*/
    public void SetUserInfo(HttpUserInfo userinfo) {
        user.cash = userinfo.cash;
        user.userLvl = userinfo.userLvl;
        user.userNextLvlXp = userinfo.userNextLvlXp;
        user.userXp = userinfo.userXp;
        user.userClassement = userinfo.userClassement;
    }

    public UserInfo UserInfoAct {
        get{return user;}
    }
    public void SetOpponent(HttpOpponent infos) {
        nextOpponent = infos;
    }

    public BotBuilder OpponentBot {
        get{ return opponent;}
        set{ opponent = value;}
    }

    public BotBuilder GetActiveBot {
        get {return myBots[selectedBot];}
    }

    public int SelectedBotIndex {
        get {return selectedBot;}
        set {selectedBot = value;}
    }

    public BotBuilder[] LoadedBots {
        get {return myBots;}
        set {myBots = value;}
    }

    public string UserUsername {
        get{return user.username;}
        set{user.username = value;}
    }

    public Scenes SceneToLoad {
        get{return asyncSceneLoad;}
        set{asyncSceneLoad = value;}
    }
    public BattleSettings CurrentBattleSettings {
        get{return battleSettings;}
        set{battleSettings = value;}
    }
    public bool[] BattlePrep {
        get { return new bool[3]{battleReady,opponentLoaded,localLoaded};}
    }
    public void resetBattlePrep() {
        battleReady = opponentLoaded = localLoaded = false;
    }



}


public enum Scenes {
    MainMenu, Battle, sceneLoader, Login, Auth, botMaker
}
public enum BattleType {
    OnlineQuick , Offline, OnlineSetup
}

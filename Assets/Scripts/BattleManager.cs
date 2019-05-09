/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
// Nom de la classe: BattleManager
// Fonctionnement de la classe: s'occupe du déroulement d'un combat dnas la scene "Battle"
// notes:
// bugs:
// todo:
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class BattleManager : MonoBehaviour {

    public static BattleManager instance;
    private BattleSettings battleSettings;
    private GameObject areneGo;
    private Arene areneComp;
    private Bot localBot;
    private string localBotName;
    private Bot visitorBot;
    private string visitorBotName;
    private BotState localBotState;
    private BotState visitorBotState;
    private float battleClock = 90 ;
    private string winner = "";
    private bool quickEnd = false;


    // CINEMACHINE
    [SerializeField] CinemachineVirtualCamera vcam1;
    [SerializeField] CinemachineVirtualCamera vcam2;// pour les close up ...
    [SerializeField] CinemachineVirtualCamera vcam3Visitor;// pour la pres ...
    [SerializeField] CinemachineVirtualCamera vcam3local;// pour la pres ...
    [SerializeField] CinemachineDollyCart vcam3Cartvisitor;
    [SerializeField] CinemachineDollyCart vcam3Cartlocal;
    [SerializeField] CinemachineVirtualCamera vcamCoreLocal;
    [SerializeField] CinemachineVirtualCamera vcamCoreVisitor;

    [SerializeField] private Text visitorHealthText;
    [SerializeField] private Text visitorBotNameText;
    [SerializeField] private Text localHealthText;
    [SerializeField] private Text localBotNameText;
    [SerializeField] private Slider localHealthSlider;
    [SerializeField] private Slider visitorHealthSlider;

    [SerializeField] private GameObject _panneauUIBattle;
    [SerializeField] private Text clock;

    [SerializeField] private PanneauFin panneauFin;

    [SerializeField] private Text countdownText;
    [SerializeField] private GameObject presentationPanel;
    [SerializeField] private Text presentationPanelText;

    //phrase de fin 
    private Dictionary<string,Dictionary<causeOfDeath,string>> phrasesFin = new Dictionary<string, Dictionary<causeOfDeath, string>>();

    void Awake(){
        if(instance == null) {
            instance = this;
            GameManager.instance.resetBattlePrep();
            battleSettings = GameManager.instance.CurrentBattleSettings;
            areneGo = Instantiate(battleSettings.Scenery);
            areneComp = areneGo.GetComponent<Arene>();
            fillPhraseFin();
            StartCoroutine("addbots");
        } else {
            Destroy(this.gameObject);
        }
        
        
    }

    private void fillPhraseFin(){
        phrasesFin.Add("visitor",new Dictionary<causeOfDeath, string>());
        phrasesFin["visitor"].Add(causeOfDeath.CoreDestroyed,"The opponent core has been destroyed");
        phrasesFin["visitor"].Add(causeOfDeath.Totaled,"The opponent bot has been totaled");
        phrasesFin["visitor"].Add(causeOfDeath.UpsideDown,"The opponent bot has been flipped");
        phrasesFin["visitor"].Add(causeOfDeath.Damage,"Your bot is the least damaged after timeout");
        phrasesFin.Add("local",new Dictionary<causeOfDeath, string>());
        phrasesFin["local"].Add(causeOfDeath.CoreDestroyed,"Your bot's core has been destroyed");
        phrasesFin["local"].Add(causeOfDeath.Totaled,"Your bot has been totaled");
        phrasesFin["local"].Add(causeOfDeath.UpsideDown,"Your bot has been flipped");
        phrasesFin["local"].Add(causeOfDeath.Damage,"Your bot is more damaged after timeout");
    }

    private void assignCams(){
        vcam1.Follow =localBot.gameObject.transform;
        vcam1.LookAt = localBot.gameObject.transform;
        vcam2.Follow =localBot.gameObject.transform;
        vcam2.LookAt = localBot.gameObject.transform;
        vcam3Visitor.LookAt = visitorBot.gameObject.transform;
        vcam3local.LookAt = localBot.gameObject.transform;
        vcamCoreLocal.LookAt = localBot.CoreGO.gameObject.transform;
        vcamCoreLocal.Follow = localBot.CoreGO.gameObject.transform;
        vcamCoreVisitor.LookAt = visitorBot.CoreGO.gameObject.transform;
        vcamCoreVisitor.Follow = visitorBot.CoreGO.gameObject.transform;
    }

    private IEnumerator addbots(){
        GameObject[] spawners = areneComp.getSpawners();
        BotEntityBuilder bot1 = spawners[0].AddComponent<BotEntityBuilder>();
        BotEntityBuilder bot2 = spawners[1].AddComponent<BotEntityBuilder>();
        bot1.SetBotBuilder = battleSettings.localbot;
        bot1.NameSetterValue = "local";
        bot2.SetBotBuilder = battleSettings.visitorbot;
        bot2.NameSetterValue = "visitor";
        localBotName = battleSettings.localbot.botsName;
        visitorBotName = battleSettings.visitorbot.botsName;

        while(localBot==null && visitorBot==null){
            Debug.Log("not Ready yet");
            yield return new WaitForSeconds(0.1f);
        }
        while(visitorBot.State == BotState.Building || localBot.State == BotState.Building){
            Debug.Log("building bots...");
            Debug.Log("vistitor is "+visitorBot.State+" local is "+localBot.State);
            yield return new WaitForSeconds(0.1f);
        }
        assignCams();
        switchCam(CamSelect.VisitorPresentation);
        yield return StartCoroutine("presentation");

        countdownText.GetComponent<UIFader>().fade(FadeTransition.In,0.3f);
        yield return new WaitForSeconds(1f);
        countdownText.text = "2";
        yield return new WaitForSeconds(1f);
        countdownText.text = "1";
        yield return new WaitForSeconds(1f);
        countdownText.text = "Battle!";
        countdownText.GetComponent<UIFader>().fade(FadeTransition.Out,0.6f);
        switchCam(CamSelect.LocalFollowNear);
        localBotNameText.text = localBotName;
        visitorBotNameText.text = visitorBotName;
        _panneauUIBattle.GetComponent<UIFader>().fade(FadeTransition.In, 0.5f);
        StartCoroutine("beginBattle");
        yield return null;
    }

    private IEnumerator presentation() {
        bool[] presentActions = new bool[4]{false,false,false,false};// visitor in, visitor out, local in, local out
        vcam3Cartlocal.m_Speed = 0.8f;
        vcam3Cartvisitor.m_Speed = 0.8f;
        Debug.Log("presentation");
        while(vcam3Cartlocal.m_Position<8f){
            float pos = vcam3Cartlocal.m_Position;
            if(pos>=0 && !presentActions[0]){
                presentationPanelText.text = visitorBotName;
                presentationPanel.GetComponent<UIFader>().fade(FadeTransition.In,0.5f);
                presentActions[0]=true;
            } else if(pos >= 3 && !presentActions[1]){
                presentationPanel.GetComponent<UIFader>().fade(FadeTransition.Out,0.5f);
                presentActions[1]=true;
                vcam3Cartvisitor.m_Speed = 1.5f;
                vcam3Cartlocal.m_Speed = 1.5f;
                switchCam(CamSelect.LocalPresentation);
            }else if(pos >= 6 && !presentActions[2]){
                vcam3Cartvisitor.m_Speed = 0.8f;
                vcam3Cartlocal.m_Speed = 0.8f;
                presentationPanelText.text = localBotName;
                presentationPanel.GetComponent<UIFader>().fade(FadeTransition.In,0.5f);
                presentActions[2]=true;
            }else if(pos >= 7.5f && !presentActions[3]){
                presentationPanel.GetComponent<UIFader>().fade(FadeTransition.Out,0.5f);
                presentActions[3]=true;
            }
            yield return new WaitForSeconds(0.1f);
        }
        switchCam(CamSelect.LocalFollowFar);
        yield return null;
    }


    private IEnumerator beginBattle(){
        localBot.activate();
        visitorBot.activate();
        float duration = battleClock;
        float startTime = Time.time;
        while(startTime+duration > Time.time && !quickEnd ){
            battleClock = (startTime+duration)-Time.time;
            clock.text = float2minute(battleClock);
            showHealth();
            yield return 0;
            
        }
        _panneauUIBattle.GetComponent<UIFader>().fade(FadeTransition.Out, 0.5f);
        localBot.deactivate();
        visitorBot.deactivate();
        if(winner == ""){
            yield return StartCoroutine("getWinner");
        }

        yield return new WaitForSeconds(3f);
        Bot loser = (winner == "local")?visitorBot:localBot;
        string loserName =  (winner == "local")?"visitor":"local";
        panneauFin.titre.text = phrasesFin[loserName][loser.DeathCause];
        panneauFin.GetComponent<UIFader>().fade(FadeTransition.In,0.5f);
        panneauFin.GetComponent<CanvasGroup>().interactable = true;

        yield return null;
    }
    private string float2minute(float temps){
        string minute = (Mathf.Floor(temps/60) == 0)? "":Mathf.Floor(temps/60).ToString();
        string seconde = (minute == "")?"":":";
        seconde +=(temps%60 == 0)?seconde = "00": ((temps%60<10)?"0":"")+Mathf.Floor((temps%60)).ToString();
        return minute+seconde;
    }

    
    private void showHealth(){
        visitorHealthText.text = Mathf.RoundToInt(visitorBot.percentHP()).ToString()+"%";
        localHealthText.text = Mathf.RoundToInt(localBot.percentHP()).ToString()+"%";
        localHealthSlider.value = localBot.percentHP();
        visitorHealthSlider.value = visitorBot.percentHP();
    }

    private IEnumerator getWinner() {
        float localhp = localBot.getHp();
        float visitorHp = visitorBot.getHp();
        if(localhp == visitorHp) {
            winner = "draw";
        }else if(localhp > visitorHp) {
            winner = "local";
            visitorBot.DeathCause = causeOfDeath.Damage;
        }else {
            winner = "visitor";
            localBot.DeathCause = causeOfDeath.Damage;
        }
        yield return null;
    }

    public void SetBotState(Bot bot, BotState state){
        if(bot.gameObject.name == "visitor"){
            visitorBotState = state;
        }else if(bot.gameObject.name == "local"){
            localBotState = state;
        }
    }

    public Bot SetLocalBot {
        set{localBot = value;}
    }
    public Bot SetVisitorBot {
        set {visitorBot = value;}
    }
    public void backtoMenu() {
        GameManager.instance.changeScene(Scenes.MainMenu);
    }
    public void DeclareDeath(string deadName){
        if(deadName == "visitor") {
            winner = "local";
        } else { 
            winner = "visitor";
        }
        quickEnd = true;
    }

    public void switchCam(CamSelect nomCam){
        Dictionary<CamSelect,CinemachineVirtualCamera> listcam = new Dictionary<CamSelect, CinemachineVirtualCamera>();
        listcam.Add(CamSelect.LocalFollowFar,vcam1);
        listcam.Add(CamSelect.LocalFollowNear,vcam2);
        listcam.Add(CamSelect.LocalPresentation,vcam3local);
        listcam.Add(CamSelect.VisitorPresentation,vcam3Visitor);
        listcam.Add(CamSelect.CoreLocal,vcamCoreLocal);
        listcam.Add(CamSelect.CoreVisitor,vcamCoreVisitor);

        foreach(KeyValuePair<CamSelect,CinemachineVirtualCamera> pair in listcam) {
            CinemachineVirtualCamera cameraAct = pair.Value;
            if(pair.Key == nomCam){
                pair.Value.m_Priority = 50;
            }else {
                pair.Value.m_Priority = 1;
            }   
        }

    }


}

public enum BotState {
    Created,Building, Ready
}
public enum CamSelect {
    LocalFollowFar,LocalFollowNear,LocalPresentation,VisitorPresentation,CoreLocal,CoreVisitor
}

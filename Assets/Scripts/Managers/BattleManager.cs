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
    private bool abandoned = false;


    // CINEMACHINE
    [SerializeField] CinemachineVirtualCamera vcam1;
    [SerializeField] CinemachineVirtualCamera vcam2;// pour les close up ...
    [SerializeField] CinemachineVirtualCamera vcam3Visitor;// pour la pres ...
    [SerializeField] CinemachineVirtualCamera vcam3local;// pour la pres ...
    [SerializeField] CinemachineVirtualCamera vcamCoreLocal;
    [SerializeField] CinemachineVirtualCamera vcamCoreVisitor;

    [SerializeField] private Text _visitorHealthText;
    [SerializeField] private Text _visitorBotNameText;
    [SerializeField] private Text _localHealthText;
    [SerializeField] private Text _localBotNameText;
    [SerializeField] private Slider _localHealthSlider;
    [SerializeField] private Slider _visitorHealthSlider;

    [SerializeField] private GameObject _panneauUIBattle;
    [SerializeField] private GameObject _panneauFlipCountdown;
    [SerializeField] private Text _clock;
    [SerializeField] private Text _textCountown;
    [SerializeField] private Text _textFlip;
    private bool _botIsFlipped = false;

    [SerializeField] private PanneauFin panneauFin;

    [SerializeField] private Text countdownText;
    [SerializeField] private GameObject presentationPanel;
    [SerializeField] private Text presentationPanelText;
    [SerializeField] private Text presentationPanelTextUsername;

    //phrase de fin 
    private Dictionary<string,Dictionary<causeOfDeath,string>> phrasesFin = new Dictionary<string, Dictionary<causeOfDeath, string>>();

    [SerializeField] private GameObject _xpPanel;
    [SerializeField] private GameObject _rankPanel;
    [SerializeField] private Text _txtRanking;
    [SerializeField] private Slider _sliderXp;
    [SerializeField] private GameObject _lvlupFader;
    [SerializeField] private Text _txtSliderXp;

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
        vcam3Visitor.Follow = visitorBot.gameObject.transform;
        vcam3local.Follow = localBot.gameObject.transform;
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
            yield return new WaitForSeconds(0.1f);
        }
        assignCams();
        yield return StartCoroutine("presentation");

        countdownText.GetComponent<UIFader>().Fade(FadeTransition.In,0.3f);
        yield return new WaitForSeconds(1f);
        countdownText.text = "2";
        yield return new WaitForSeconds(1f);
        countdownText.text = "1";
        yield return new WaitForSeconds(1f);
        countdownText.text = "Battle!";
        countdownText.GetComponent<UIFader>().Fade(FadeTransition.Out,0.6f);
        switchCam(CamSelect.LocalFollowNear);
        _localBotNameText.text = localBotName;
        _visitorBotNameText.text = visitorBotName;
        _panneauUIBattle.GetComponent<UIFader>().Fade(FadeTransition.In, 0.5f);
        StartCoroutine("beginBattle");
        yield return null;
    }

    private IEnumerator presentation() {
        int step = 0;
        int stepAct =0;
        int autoSkipFrames =0;
        Debug.Log("presentation");
        while(step <5){
            autoSkipFrames++;
            if(Input.touchCount >0  ){
                if(Input.GetTouch(0).phase == TouchPhase.Began){
                    step++;
                    autoSkipFrames = 0;
                }  
            }
            if(autoSkipFrames >500){
                step++;
                autoSkipFrames = 0;
            }
            if(Input.GetMouseButtonDown(0) && Application.platform != RuntimePlatform.Android){
                step++;
                autoSkipFrames = 0;
            }
            if(step != stepAct){
                switch(step){
                    case 1:
                        stepAct = step;
                        switchCam(CamSelect.VisitorPresentation);
                        presentationPanelTextUsername.text = battleSettings.opponentUsername + "'s ";
                        presentationPanelText.text = visitorBotName;
                        presentationPanel.GetComponent<UIFader>().Fade(FadeTransition.In,0.5f);
                    break;
                    case 2:
                        stepAct = step;
                        presentationPanel.GetComponent<UIFader>().Fade(FadeTransition.Out,0.5f);
                        yield return new WaitForSeconds(0.6f);
                        step++;
                    break;
                    case 3:
                        stepAct = step;
                        switchCam(CamSelect.LocalPresentation);
                        presentationPanelTextUsername.text = GameManager.instance.UserInfoAct.username + "'s ";
                        presentationPanelText.text = localBotName;
                        presentationPanel.GetComponent<UIFader>().Fade(FadeTransition.In,0.5f);
                    break;
                    case 4:
                        stepAct = step;
                        switchCam(CamSelect.LocalFollowFar);
                        presentationPanel.GetComponent<UIFader>().Fade(FadeTransition.Out,0.5f);
                        yield return new WaitForSeconds(0.6f);       
                        step++;             
                        break;
                }
            }
            yield return 0;

        }
        
        yield return null;
    }


    private IEnumerator beginBattle(){
        localBot.activate();
        visitorBot.activate();
        float duration = battleClock;
        float startTime = Time.time;
        while(startTime+duration > Time.time && !quickEnd ){
            battleClock = (startTime+duration)-Time.time;
            _clock.text = float2minute(battleClock);
            showHealth();
            yield return 0;
            
        }
        _panneauUIBattle.GetComponent<UIFader>().Fade(FadeTransition.Out, 0.5f);
        localBot.deactivate();
        visitorBot.deactivate();

        if(winner == ""){
            yield return StartCoroutine("getWinner");
        }

        yield return new WaitForSeconds(3f);

        Bot loser = (winner == "local")?visitorBot:localBot;
        string loserName =  (winner == "local")?"visitor":"local";

        GameManager.instance.SetXpPack = makeXpPack();


        panneauFin.titre.text = (abandoned)?"You abandoned the battle":phrasesFin[loserName][loser.DeathCause];
        panneauFin.GetComponent<UIFader>().Fade(FadeTransition.In,0.5f);
        panneauFin.GetComponent<CanvasGroup>().interactable = true;

        yield return StartCoroutine("xpAnimation");

        if(battleSettings.battletype == BattleType.OnlineQuick){
            yield return StartCoroutine("rankAnimation");
        }

        yield return null;
    }


    private XpPack makeXpPack(){
        XpPack tmpPack = new XpPack();
        int currentXp = GameManager.instance.UserInfoAct.userXp;
        int nextXp = GameManager.instance.UserInfoAct.userNextLvlXp;
        int xpWon = calculateXp();
        if(currentXp+xpWon<nextXp){
            tmpPack.lvl = GameManager.instance.UserInfoAct.userLvl;
            tmpPack.xp = currentXp+xpWon;
            tmpPack.xpNext = nextXp;
        }else {
            tmpPack.xp = (currentXp+xpWon)-nextXp;
            tmpPack.lvl = GameManager.instance.UserInfoAct.userLvl+1;
            tmpPack.xpNext = GameManager.instance.calculateNextXp(tmpPack.lvl); 
        }


        return tmpPack;
    }

    public void abandon(){
        abandoned = true;
        DeclareDeath("local");
    }

    private IEnumerator xpAnimation(){
        int currentXp = GameManager.instance.UserInfoAct.userXp;
        int nextXp = GameManager.instance.UserInfoAct.userNextLvlXp;
        int xpWon = calculateXp();
        
        _sliderXp.maxValue = nextXp;
        _sliderXp.minValue = 0;
        _sliderXp.value = currentXp;
        _txtSliderXp.text = currentXp.ToString()+"/"+nextXp.ToString();
        yield return new WaitForSeconds(2f);
        _xpPanel.GetComponent<UIFader>().Fade(FadeTransition.In);
        while(xpWon !=0){
            xpWon--;
            _txtSliderXp.text = (++currentXp).ToString()+"/"+nextXp.ToString();
            _sliderXp.value = currentXp;
            if(currentXp == nextXp){
                StartCoroutine("lvlUp");
                GameManager.instance.UserInfoAct.userNextLvlXp = GameManager.instance.calculateNextXp(++GameManager.instance.UserInfoAct.userLvl);
                GameManager.instance.UserInfoAct.userXp = 0;
                currentXp=0;
                nextXp = GameManager.instance.UserInfoAct.userNextLvlXp;
            }
            yield return new WaitForSeconds(0.1f);
        }
        GameManager.instance.UserInfoAct.userXp = currentXp;
        yield return null;
    }

    private IEnumerator lvlUp(){
        _lvlupFader.GetComponent<UIFader>().Fade(FadeTransition.In);
        yield return new WaitForSeconds(2f);
        _lvlupFader.GetComponent<UIFader>().Fade(FadeTransition.Out);
        yield return null;
    }

    private int calculateXp(){
        int temp = (winner == "local")?15:5;
        if(battleSettings.battletype == BattleType.OnlineSetup){
            temp = temp/3;
        }
        if(battleSettings.battletype == BattleType.Offline && GameManager.instance.UserInfoAct.userLvl>3){
            temp = 0;
        }
        if(abandoned){
            temp = 0;
        }
        return temp;
    }

    private IEnumerator rankAnimation(){
        int modifierRank = (winner=="local")?5:-3;
        int currentRank = GameManager.instance.UserInfoAct.userClassement;
        int targetRank = currentRank+modifierRank;
        _rankPanel.GetComponent<UIFader>().Fade(FadeTransition.In);
        _txtRanking.text = currentRank.ToString();
        yield return new WaitForSeconds(1f);
        while(currentRank != targetRank ){
            if(currentRank<targetRank){
                currentRank++;
            }else{
                currentRank--;
            }
            _txtRanking.text = currentRank.ToString();
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(2f);
        yield return null; 

    }

    private string float2minute(float temps){
        string minute = (Mathf.Floor(temps/60) == 0)? "":Mathf.Floor(temps/60).ToString();
        string seconde = (minute == "")?"":":";
        seconde +=(temps%60 == 0)?seconde = "00": ((temps%60<10)?"0":"")+Mathf.Floor((temps%60)).ToString();
        return minute+seconde;
    }

    
    private void showHealth(){
        _visitorHealthText.text = Mathf.RoundToInt(visitorBot.percentHP()).ToString()+"%";
        _localHealthText.text = Mathf.RoundToInt(localBot.percentHP()).ToString()+"%";
        _localHealthSlider.value = localBot.percentHP();
        _visitorHealthSlider.value = visitorBot.percentHP();
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
    public string CountdownValue{
        get{return _textCountown.text;}
        set{_textCountown.text = value;}
    }
    public string CountdownName {
        set {_textFlip.text = value;}
    }
    public GameObject PanelFlip {
        get{return _panneauFlipCountdown;}
    }
    public bool BotIsFlipped {
        get{return _botIsFlipped;}
        set{_botIsFlipped = value;}
    }
    public void backtoMenu() {
        //GameManager.instance.changeScene(Scenes.MainMenu);
        if(battleSettings.battletype != BattleType.OnlineQuick){
            StartCoroutine(GameManager.instance.returnToMenu());
        }else {
            StartCoroutine(GameManager.instance.returnToMenu(true,battleSettings.visitorbot.ownerID,winner));
        }
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

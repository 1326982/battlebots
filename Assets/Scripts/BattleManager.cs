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

public class BattleManager : MonoBehaviour {

    public static BattleManager instance;
    private BattleSettings battleSettings;
    private GameObject areneGo;
    private Arene areneComp;
    private Bot localBot;
    private Bot visitorBot;
    private float battleClock = 90 ;
    private string winner = "";
    private bool quickEnd = false;

    [SerializeField] private Text clock;
    [SerializeField] private Slider clockSlider;

    [SerializeField] private PanneauFin panneauFin;

    void Awake(){
        if(instance == null) {
            instance = this;
        } else {
            Destroy(this.gameObject);
        }
        GameManager.instance.resetBattlePrep();
        battleSettings = GameManager.instance.CurrentBattleSettings;
        areneGo = Instantiate(battleSettings.Scenery);
        areneComp = areneGo.GetComponent<Arene>();
        StartCoroutine("addbots");
        
    }
    private IEnumerator addbots(){
        GameObject[] spawners = areneComp.getSpawners();
        BotEntityBuilder bot1 = spawners[0].AddComponent<BotEntityBuilder>();
        BotEntityBuilder bot2 = spawners[1].AddComponent<BotEntityBuilder>();
        bot1.SetBotBuilder = battleSettings.localbot;
        bot1.NameSetterValue = "local";
        bot2.SetBotBuilder = battleSettings.visitorbot;
        bot2.NameSetterValue = "visitor";
        print("3");
        yield return new WaitForSeconds(1f);
        print("2");
        yield return new WaitForSeconds(1f);
        print("1");
        yield return new WaitForSeconds(1f);
        StartCoroutine("beginBattle");
        yield return null;
    }

    private IEnumerator beginBattle(){
        localBot.activate();
        visitorBot.activate();
        float duration = battleClock;
        float startTime = Time.time;
        while(startTime+duration > Time.time && !quickEnd ){
            battleClock = (startTime+duration)-Time.time;
            clockSlider.value = battleClock;
            clock.text = float2minute(battleClock);
            yield return new WaitForSeconds(0.2f);
            
        }
        localBot.deactivate();
        visitorBot.deactivate();
        if(winner == ""){
            yield return StartCoroutine("getWinner");
        }
        panneauFin.titre.text = (winner == "local")?"You won!":(winner == "visitor")?"You lost":"Draw!";
        panneauFin.gameObject.SetActive(true);

        yield return null;
    }
    private string float2minute(float temps){
        string minute = (Mathf.Floor(temps/60) == 0)? "":Mathf.Floor(temps/60).ToString();
        string seconde = (minute == "")?"":":";
        seconde +=(temps%60 == 0)?seconde = "00": ((temps%60<10)?"0":"")+Mathf.Floor((temps%60)).ToString();
        return minute+seconde;
    }

    

    private IEnumerator getWinner() {
        float localhp = localBot.getHp();
        float visitorHp = visitorBot.getHp();
        if(localhp == visitorHp) {
            winner = "draw";
        }else if(localhp > visitorHp) {
            winner = "local";
        }else {
            winner = "visitor";
        }
        yield return null;
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


}

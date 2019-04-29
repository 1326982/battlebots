/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
// Nom de la classe: BotEntityBuilder
// Fonctionnement de la classe: component ajouté a un spawner permettant d'initialiser un bot a partir de sa base;
// notes:
// bugs:
// todo:
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotEntityBuilder : MonoBehaviour {
    private BotBuilder instructions;
    private bool isEdited = false;
    private string botName;
    // Start is called before the first frame update
    void Start() {
        GameObject plateforme = Resources.Load(instructions.platform) as GameObject;
        GameObject bot = Instantiate(plateforme,transform.position, transform.rotation);
        Bot botComponent = bot.AddComponent<Bot>();
        botComponent.isEdited = isEdited;
        botComponent.Build(instructions);
        bot.name = botName;
        if(botName == "local"){
            BattleManager.instance.SetLocalBot = botComponent;
        } else if(botName == "visitor"){
            BattleManager.instance.SetVisitorBot = botComponent;
        }
        
        Destroy(gameObject);
    }

    public BotBuilder SetBotBuilder {
        set {instructions = value;}
    }
    public bool Edited {
        set{isEdited = value;}
        get{return isEdited;}
    }
    public string NameSetterValue {
        get {return botName;}
        set {botName = value;}
    }


}

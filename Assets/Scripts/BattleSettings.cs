﻿/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
// Nom de la classe: BattleSettings
// Fonctionnement de la classe: Contient les informations pour démarrer un combat, sera envoyé en objet a battle Manager
// notes:
// bugs:
// todo:
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSettings {   
    public GameObject Scenery;
    public BotBuilder localbot;
    public BotBuilder visitorbot;
    public string opponentBotName;
    public string opponentUsername;
    public string opponentID;
    public BattleType battletype;
}

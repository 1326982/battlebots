﻿/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
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
using UnityEngine;
using UnityEditor;

public class ItemInfo  {
    public string name;
    public string callback;
    public string nomPiece;
    public bool isMiniThumb;


    public ItemInfo(string nom, string pathimg, string call, string piece ,bool isMiniThumbConst){
        name = nom;
        callback = call;
        nomPiece = piece;
        isMiniThumb = isMiniThumbConst;
    }

    



}

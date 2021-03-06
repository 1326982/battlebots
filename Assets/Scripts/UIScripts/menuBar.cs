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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class menuBar : MonoBehaviour   {
    private RectTransform rTransform;
    [SerializeField] private GameObject EmptyItem;
    [SerializeField] private GameObject notificationBt;
    private List<GameObject> vieuxBouton = new List<GameObject>(){};
    /*------------------------------------------------------------------------------------------------------------------------------------------- */
    private ItemInfo[] menuPrincipal = new ItemInfo[5]{ new ItemInfo("Training","training","battle","offline",false),
                                                        new ItemInfo("Battle","battle","multiplayer","online",false),
                                                        new ItemInfo("Customize Robot","custom","customize","",false),
                                                        new ItemInfo("Notifications","notif","showNotifications","Notifications",false),
                                                        new ItemInfo("Settings","option","settings","",false)  
                                                        };
    /*------------------------------------------------------------------------------------------------------------------------------------------- */
    private ItemInfo[] menuCustomize = new ItemInfo[5] {new ItemInfo("Back","backbt","principal","",false),
                                                        new ItemInfo("Robot bases","bases","subCustomize","bases",false),
                                                        new ItemInfo("Locomotion","wheel","subCustomize","roues",false),
                                                        new ItemInfo("Weapons","battle","subCustomize","armesMelee",false),
                                                        new ItemInfo("Defense","defense","subCustomize","defense",false) 
                                                        };
    private ItemInfo[] menuSettings = new ItemInfo[2] {new ItemInfo("Back","backbt","principal","",false),
                                                        new ItemInfo("Disconnect","disconnect","disconnect","",false),
                                                        };
    /*------------------------------------------------------------------------------------------------------------------------------------------- */
    private ItemInfo[] subMenuRoues = new ItemInfo[3] { new ItemInfo("Back","backbt","customize","",false),
                                                        new ItemInfo("Simple wheel","icon_place","placerItem","roueSimple",true),
                                                        new ItemInfo("Truck wheel","icon_place","placerItem","roueTruck",true)
                                                        };
    /*------------------------------------------------------------------------------------------------------------------------------------------- */
    private ItemInfo[] subMenuArmesM = new ItemInfo[5] {new ItemInfo("Back","backbt","customize","",false),
                                                        new ItemInfo("Horizontal Roller","icon_place","placerItem","weaponRoller",true),
                                                        new ItemInfo("Hammer","icon_place","placerItem","weaponHammer",true,1),
                                                        new ItemInfo("Kinetic Flipper","icon_place","placerItem","weaponFlipper",true,2),
                                                        new ItemInfo("Circular Saw","icon_place","placerItem","weaponSaw",true,3)
                                                        };
    /*------------------------------------------------------------------------------------------------------------------------------------------- */
    private ItemInfo[] subMenuBases = new ItemInfo[3] { new ItemInfo("Back","backbt","customize","",false),
                                                        new ItemInfo("The Wedge","icon_place","changeBase","baseWedge",true),
                                                        new ItemInfo("Roombanator","icon_place","changeBase","baseRoomba",true)
                                                        };
    /*------------------------------------------------------------------------------------------------------------------------------------------- */    
    private ItemInfo[] subMenuDefense = new ItemInfo[2]{new ItemInfo("Back","backbt","customize","",false),
                                                        new ItemInfo("Square plate","icon_place","placerItem","defensePlate",true)
                                                        };
    /*------------------------------------------------------------------------------------------------------------------------------------------- */
    void Start() {
        rTransform = GetComponent<RectTransform>();
        montrerPrincipal();
    }

  
    void changerMenu(ItemInfo[] menu) {
        nettoyerVieuxBt();
        float division = (rTransform.rect.width)/menu.Length;
        float menuFirstPos = (-rTransform.rect.width/2)+(division/2);

        for(int i = 0; i<menu.Length;i++){
            if(menu[i].nomPiece != "Notifications"){
                GameObject newItem = Instantiate(EmptyItem,new Vector2(0,0),rTransform.rotation);
                vieuxBouton.Add(newItem);
                newItem.GetComponent<menuItem>().callback = menu[i].callback;
                newItem.GetComponent<menuItem>().texteItem.text = menu[i].name;
                newItem.GetComponent<menuItem>().nomPiece = menu[i].nomPiece;
                newItem.transform.SetParent(gameObject.transform,false);
                RectTransform newItemRT =newItem.GetComponent<RectTransform>();
                newItemRT.sizeDelta = new Vector2(division , rTransform.sizeDelta.y);
                newItemRT.localPosition = new Vector3((menuFirstPos+(division*i)),0,0);
                newItem.GetComponent<menuItem>().imageItem.texture = Resources.Load<Texture2D>(menu[i].pathimg);
                if(menu[i].isMiniThumb) {
                    StartCoroutine(loadThumb(newItem.GetComponent<menuItem>().imageItem,menu[i].nomPiece));
                }
                if(menu[i].lvlLock>GameManager.instance.UserInfoAct.userLvl){
                    newItem.GetComponent<Button>().interactable = false;
                    newItem.GetComponent<menuItem>().lockFilter.SetActive(true);
                    newItem.GetComponent<menuItem>().lockText.text = "Unlock at lvl "+menu[i].lvlLock;
                }
            }else {
                GameObject newItem = Instantiate(notificationBt,new Vector2(0,0),rTransform.rotation);
                vieuxBouton.Add(newItem);
                newItem.GetComponent<menuItem>().callback = menu[i].callback;
                newItem.GetComponent<menuItem>().texteItem.text = menu[i].name;
                newItem.GetComponent<menuItem>().nomPiece = menu[i].nomPiece;
                
                newItem.transform.SetParent(gameObject.transform,false);
                RectTransform newItemRT =newItem.GetComponent<RectTransform>();
                newItemRT.sizeDelta = new Vector2(division , rTransform.sizeDelta.y);
                newItemRT.localPosition = new Vector3((menuFirstPos+(division*i)),0,0);
                newItem.GetComponent<menuItem>().imageItem.texture = Resources.Load<Texture2D>(menu[i].pathimg);
                if(menu[i].isMiniThumb) {
                    StartCoroutine(loadThumb(newItem.GetComponent<menuItem>().imageItem,menu[i].nomPiece));
                }
            }
        }
    }

    IEnumerator loadThumb(RawImage imageItem , string nomItem) {
        Texture2D imgIcon = Resources.Load<Texture2D>("thumbnails/thumb"+nomItem);
        yield return imgIcon;
        imageItem.texture = imgIcon;
    }

    public void nettoyerVieuxBt() {
        foreach(GameObject item in vieuxBouton){
            Destroy(item);
        }  
        vieuxBouton = new List<GameObject>(){};
    }
    public void montrerCustomize(){
        changerMenu(menuCustomize);
    }
    public void montrerPrincipal(){
        changerMenu(menuPrincipal);
    }
    public void montrerSettings(){
        changerMenu(menuSettings);
    }
    public void montrerSubMenuCustom(string menu){
        switch(menu) {
            case "roues":
                MenuManager.instance.menuCustomeAct = menu;
                MenuManager.instance.showLimit(PieceType.Locomotion);
                changerMenu(subMenuRoues);
            break; 
            case "bases": 
                changerMenu(subMenuBases);
            break;
            case "armesMelee":
                MenuManager.instance.menuCustomeAct = menu;
                MenuManager.instance.showLimit(PieceType.Attack);
                changerMenu(subMenuArmesM);
            break;
            case "defense":
                MenuManager.instance.menuCustomeAct = menu;
                MenuManager.instance.showLimit(PieceType.Defense);
                changerMenu(subMenuDefense);
            break;   
        }
    }
}

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
using UnityEditor;
using UnityEngine.UI;

public class menuBar : MonoBehaviour   {
    private RectTransform rTransform;
    [SerializeField] private GameObject EmptyItem;
    private List<GameObject> vieuxBouton = new List<GameObject>(){};
    /*------------------------------------------------------------------------------------------------------------------------------------------- */
    private ItemInfo[] menuPrincipal = new ItemInfo[4]{ new ItemInfo("Offline Battle","icon_place","battle","offline",false),
                                                        new ItemInfo("Online Battle","icon_place","multiplayer","online",false),
                                                        new ItemInfo("Customize Robot","icon_place","customize","",false),
                                                        new ItemInfo("Settings","icon_place","settings","",false)  
                                                        };
    /*------------------------------------------------------------------------------------------------------------------------------------------- */
    private ItemInfo[] menuCustomize = new ItemInfo[5] {new ItemInfo("Back","icon_place","principal","",false),
                                                        new ItemInfo("Robot bases","icon_place","subCustomize","bases",false),
                                                        new ItemInfo("Locomotion","icon_place","subCustomize","roues",false),
                                                        new ItemInfo("Weapons","icon_place","subCustomize","armesMelee",false),
                                                        new ItemInfo("Abilities","icon_place","back","",false) 
                                                        };
    private ItemInfo[] menuSettings = new ItemInfo[2] {new ItemInfo("Back","icon_place","principal","",false),
                                                        new ItemInfo("Disconnect","icon_place","disconnect","",false),
                                                        }; 
    /*------------------------------------------------------------------------------------------------------------------------------------------- */
    private ItemInfo[] subMenuRoues = new ItemInfo[3] { new ItemInfo("Back","icon_place","customize","",false),
                                                        new ItemInfo("Simple wheel","icon_place","placerItem","roueSimple",true),
                                                        new ItemInfo("Truck wheel","icon_place","placerItem","roueTruck",true)
                                                        };
    /*------------------------------------------------------------------------------------------------------------------------------------------- */
    private ItemInfo[] subMenuArmesM = new ItemInfo[3] {new ItemInfo("Back","icon_place","customize","",false),
                                                        new ItemInfo("Horizontal Roller","icon_place","placerItem","weaponRoller",true),
                                                        new ItemInfo("Hammer","icon_place","placerItem","weaponHammer",true)
                                                        };
    /*------------------------------------------------------------------------------------------------------------------------------------------- */
    private ItemInfo[] subMenuBases = new ItemInfo[2] { new ItemInfo("Back","icon_place","customize","",false),
                                                        new ItemInfo("The Wedge","icon_place","changeBase","baseWedge",true)
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
            GameObject newItem = Instantiate(EmptyItem,new Vector2(0,0),rTransform.rotation);
            vieuxBouton.Add(newItem);
            newItem.GetComponent<menuItem>().callback = menu[i].callback;
            newItem.GetComponent<menuItem>().texteItem.text = menu[i].name;
            newItem.GetComponent<menuItem>().nomPiece = menu[i].nomPiece;
            newItem.transform.SetParent(gameObject.transform,false);
            RectTransform newItemRT =newItem.GetComponent<RectTransform>();
            newItemRT.sizeDelta = new Vector2(division , rTransform.sizeDelta.y);
            newItemRT.localPosition = new Vector3((menuFirstPos+(division*i)),0,0);
            if(menu[i].isMiniThumb) {
                StartCoroutine(loadThumb(newItem.GetComponent<menuItem>().imageItem,menu[i].nomPiece));
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
                changerMenu(subMenuRoues);
            break; 
            case "bases": 
                changerMenu(subMenuBases);
            break;
            case "armesMelee":
                changerMenu(subMenuArmesM);
            break;    
        }
    }
}

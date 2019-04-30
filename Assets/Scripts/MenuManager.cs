﻿/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
// Nom de la classe: MenuManager 
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
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class MenuManager : MonoBehaviour {
    public static MenuManager instance;

    [SerializeField] private Transform botSpawnPos;
    [SerializeField] private menuBar barreMenu;
    [SerializeField] private ProfileInfoPanel panelinfo;
    [SerializeField] private CameraMenu cameraMenu;
    [SerializeField] private GameObject modalBox;
    [SerializeField] public EventSystem events;
    [SerializeField] private PoubelleUI trashUI;
    [SerializeField] private GameObject PanneauMulti;

    private Bot showedBot;

    private bool hasChangedSinceLastSave = false;

    private bool isEditing = false;
    private bool blockItemClick = false;

    public GameObject botContainer;
    private GameObject selectedPart = null;
    private bool selectedIsOnFloor = false;
    private GameObject slideSelectedPartInDMs = null;
    private int SelectedPartFingerID = 0;
    private string modalParamBase;
    
    private string menuParam;

    private GameObject pieceEnTransit;
    [SerializeField]private GameObject transitAnchor;


    void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(this.gameObject);
        }
    }
    private void Start() {
        UserInfo info = GameManager.instance.UserInfoAct;
        panelinfo.txtranking.text = "Ranking "+ info.userClassement;
        panelinfo.txtnom.text = info.username;
        panelinfo.txtlvl.text = info.userLvl.ToString();
        panelinfo.textXp.text = info.userXp + "/" + info.userNextLvlXp;
        float xp = (float)info.userXp/(float)info.userNextLvlXp;
        panelinfo.sliderXp.value = xp;
        botContainer = new GameObject();
        spawnEditBot();
    }



    void Update() {
        if(isEditing) {
            if(!blockItemClick) {
                editingHandler();
            }
        }
    }

    private void editingHandler() {
        if (Application.platform != RuntimePlatform.Android) { // souris Dev
            if(selectedPart != null) {// souris Dev
                Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );// souris Dev
                RaycastHit hit;// souris Dev
                if( Physics.Raycast( ray, out hit, 100 ) ) {// souris Dev
                    
                    if(hit.transform.gameObject.tag == "anchorBuild") {// souris Dev
                        if(selectedPart != null) {// souris Dev
                            if(hit.transform.gameObject.GetComponent<AnchorPlace>().isOccupied == false) {// souris Dev
                                swapAnchor(hit);// souris Dev
                            } else {// souris Dev
                                print("position is already taken!");// souris Dev
                            }// souris Dev
                        }// souris Dev
                        selectedIsOnFloor = false;// souris Dev
                    }else if(hit.transform.gameObject.tag == "PlancherEditor"){// souris Dev
                        // if(selectedPart != null ){// souris Dev
                        //     trashItem(hit);// souris Dev
                        // }// souris Dev
                        selectedIsOnFloor = true;// souris Dev
                    } else {// souris Dev
                        selectedPart.transform.position = hit.point;// souris Dev
                        selectedIsOnFloor = false;// souris Dev
                        
                    }// souris Dev
                }// souris Dev
            }// souris Dev
            if( Input.GetMouseButtonUp(0) ) {// souris Dev
                if(selectedPart != null) {// souris Dev
                    if(selectedIsOnFloor){// souris Dev
                        if(selectedPart != null ){// souris Dev
                            trashItem();// souris Dev
                        }
                    }else{// souris Dev
                        selectedPart.transform.position = selectedPart.GetComponent<EditorInfo>().linkedAnchor.transform.position;// souris Dev
                        selectedPart.GetComponent<Collider>().enabled = true;// souris Dev
                        selectedPart = null;// souris Dev
                        cameraMenu.blockCam = false;// souris Dev
                    }// souris Dev

                }// souris Dev
                
            }// souris Dev
            if( Input.GetMouseButtonDown(0) ) {
                Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
                RaycastHit hit;
                if(slideSelectedPartInDMs!=null){
                    selectedPart = slideSelectedPartInDMs;
                    slideSelectedPartInDMs = null;
                    selectedPart.GetComponent<Collider>().enabled = false;
                    cameraMenu.blockCam = true;
                }else{
                    if( Physics.Raycast( ray, out hit, 100 ) ) {
                        if(hit.transform.gameObject.tag == "mWeapon" || hit.transform.gameObject.tag == "wheel" ) {
                            selectedPart = hit.transform.gameObject;
                            selectedPart.GetComponent<Collider>().enabled = false;
                            cameraMenu.blockCam = true;
                        }
                    }
                }
            }
        }
        if(Input.touchCount>0){
            Debug.Log("ya "+Input.touchCount+" touchs");
            for(int i = 0;i<Input.touchCount;i++){
                Touch toucheAct = Input.GetTouch(i);
                
                if(toucheAct.phase == TouchPhase.Moved){
                    if(selectedPart != null && toucheAct.fingerId == SelectedPartFingerID ) {
                        Ray ray = Camera.main.ScreenPointToRay(toucheAct.position );
                        RaycastHit hit;
                        if( Physics.Raycast( ray, out hit, 100 ) ) {
                            
                            if(hit.transform.gameObject.tag == "anchorBuild") {
                                selectedIsOnFloor = false;
                                if(selectedPart != null) {
                                    if(hit.transform.gameObject.GetComponent<AnchorPlace>().isOccupied == false) {
                                        swapAnchor(hit);
                                    } else {
                                        print("position is already taken!");
                                    }
                                }
                            }else if(hit.transform.gameObject.tag == "PlancherEditor"){
                                selectedIsOnFloor = true;
                                selectedPart.transform.position = hit.point;
                                // if(selectedPart != null ){
                                //     trashItem();
                                // }
                            } else {
                                selectedIsOnFloor = false;
                                selectedPart.transform.position = hit.point;
                                
                            }
                        }
                    }
                }else if(toucheAct.phase == TouchPhase.Ended && toucheAct.fingerId == SelectedPartFingerID) {
                    if(selectedPart != null) {
                        if(selectedIsOnFloor){
                            if(selectedPart != null ){
                                trashItem();
                            }
                        }else{
                            selectedPart.transform.position = selectedPart.GetComponent<EditorInfo>().linkedAnchor.transform.position;
                            selectedPart.GetComponent<Collider>().enabled = true;
                            selectedPart = null;
                            cameraMenu.blockCam = false;
                            trashUI.fingerid=0;
                            trashUI.isSelecting = false;
                            trashUI.gameObject.SetActive(false);
                            
                        }

                    }
                }else if(toucheAct.phase == TouchPhase.Began){
                    Ray ray = Camera.main.ScreenPointToRay( toucheAct.position );
                    RaycastHit hit;
                    if( Physics.Raycast( ray, out hit, 100 ) ) {
                        if(hit.transform.gameObject.tag == "mWeapon" || hit.transform.gameObject.tag == "wheel" ) {
                            selectedPart = hit.transform.gameObject;
                            SelectedPartFingerID = toucheAct.fingerId;
                            selectedPart.GetComponent<Collider>().enabled = false;
                            cameraMenu.blockCam = true;
                            trashUI.fingerid=SelectedPartFingerID;
                            trashUI.isSelecting = true;
                            trashUI.gameObject.SetActive(true);
                        }
                    }  
                }else if(slideSelectedPartInDMs!=null){
                    selectedPart = slideSelectedPartInDMs;
                    slideSelectedPartInDMs = null;
                    selectedPart.GetComponent<Collider>().enabled = false;
                    cameraMenu.blockCam = true;
                    trashUI.fingerid=SelectedPartFingerID;
                    trashUI.isSelecting = true;
                    trashUI.gameObject.SetActive(true);
                }
            }
        }
    }
    private void trashItem() {
        hasChangedSinceLastSave = true;
        EditorInfo editorInfoActuel = selectedPart.GetComponent<EditorInfo>();
        // la position avant déplacement de la pièce
        int oldAnchorPos = editorInfoActuel.linkedAnchor.GetComponent<AnchorPlace>().infos.anchorPos;
        // L'ancre avant déplacement de la pièce
        AnchorPlace editorInfoActAnchor = editorInfoActuel.linkedAnchor.GetComponent<AnchorPlace>();
        // On réinitialise l'ancre précédente a une ancre libre
        editorInfoActAnchor.infos = new AnchorInfo();
        editorInfoActAnchor.infos.anchorPos = oldAnchorPos;
        editorInfoActAnchor.isOccupied = false;
        Destroy(selectedPart);
        selectedPart = null;
        cameraMenu.blockCam = false;
        trashUI.fingerid=0;
        trashUI.isSelecting = false;
        
    }
    private void swapAnchor(RaycastHit hit){
        // les info de la pièce en déplacement
        EditorInfo editorInfoActuel = selectedPart.GetComponent<EditorInfo>();
        // la position avant déplacement de la pièce
        int oldAnchorPos = editorInfoActuel.linkedAnchor.GetComponent<AnchorPlace>().infos.anchorPos;
        // L'ancre avant déplacement de la pièce
        AnchorPlace editorInfoActAnchor = editorInfoActuel.linkedAnchor.GetComponent<AnchorPlace>();
        // On réinitialise l'ancre précédente a une ancre libre
        editorInfoActAnchor.infos = new AnchorInfo();
        editorInfoActAnchor.infos.anchorPos = oldAnchorPos;
        editorInfoActAnchor.isOccupied = false;
        // on lie la nouvelle ancre a la pièce (_dans son Editor Info)
        editorInfoActuel.linkedAnchor = hit.transform.gameObject;
        // on va re-chercher les info de cette nouvelle ancre
        AnchorPlace aPActuel = editorInfoActuel.linkedAnchor.GetComponent<AnchorPlace>();
        // l'ancre sait sa position reste seulement a la pièce de s'identifier
        aPActuel.infos.itemName = editorInfoActuel.nomItem;
        aPActuel.isOccupied=true;
        //solution pour le moment: désacitver la pièce, effectuer la permutation puis réactiver la pièce -- marche bien en fin de compte
        selectedPart.SetActive(false);
        selectedPart.transform.position = hit.transform.position;
        selectedPart.transform.rotation = hit.transform.rotation;
        selectedPart.SetActive(true);
        hasChangedSinceLastSave = true;
    }

    public void spawnEditBot() {
        hasChangedSinceLastSave = false;
        GameObject botSpawner = new GameObject();
        botSpawner.transform.position = botSpawnPos.position;
        botSpawner.transform.rotation = botSpawnPos.rotation;
        BotEntityBuilder entityBuilder = botSpawner.AddComponent<BotEntityBuilder>();
        entityBuilder.SetBotBuilder = GameManager.instance.GetActiveBot;
        entityBuilder.NameSetterValue = "menuBot";
        entityBuilder.Edited = true;
    }

    public GameObject loadGO(string nomItem) {
        return Resources.Load(nomItem) as GameObject;
    }

    public void switchbot(int pos) {
        if(isEditing && hasChangedSinceLastSave){
            GameObject modal = Instantiate(modalBox,new Vector3(0, 0, 0), Quaternion.identity);
            modal.transform.SetParent (GameObject.FindGameObjectWithTag("Canvas").transform, false);
            modal.GetComponent<Modal>().init("Save bot before switching?",()=>{switchbotModalCallBackWithSave(pos);},"Yes","No",()=>{switchbotModalCallBackWithoutSave(pos);});
        } else {
            switchbotModalCallBackWithoutSave(pos);
        }
    }

    public void switchbotModalCallBackWithSave(int pos){
        saveBot(showedBot.saveBotPieces());
        GameManager.instance.SelectedBotIndex = pos;
        

    }
    public void switchbotModalCallBackWithoutSave(int pos){
        GameManager.instance.SelectedBotIndex = pos;
        spawnbotEdit();
    }
    public void spawnbotEdit(){
        Destroy(botContainer);
        botContainer = new GameObject();
        spawnEditBot();
    }

    public void callchangeBase() {
        GameObject modal = Instantiate(modalBox,new Vector3(0, 0, 0), Quaternion.identity);
        modal.transform.SetParent (GameObject.FindGameObjectWithTag("Canvas").transform, false);
        modal.GetComponent<Modal>().init("This will delete your current set-up!",changerBase,"Ok","Cancel");
        modalParamBase = menuParam;
    }
    public void changerBase() {
        Destroy(botContainer);
        botContainer = new GameObject();
        GameManager.instance.GetActiveBot.platform = modalParamBase;
        GameManager.instance.GetActiveBot.listParts = new AnchorInfo[0];
        spawnEditBot();
        hasChangedSinceLastSave = true;
    }

    public void callplacerItem() {
        blockItemClick = false;        
        cameraMenu.blockCam = true;
        GameObject item = Instantiate(loadGO("edit"+menuParam),transitAnchor.transform.position,transitAnchor.transform.rotation);
        item.transform.parent = botContainer.transform;
        editorInfoFiller(item.GetComponent<EditorInfo>(),menuParam);

        slideSelectedPartInDMs = item;
       

    }
    public void calltest(){
        GameManager.instance.changeScene(Scenes.botMaker);
    }
    public void callbattle(){
        GameManager.instance.startBattle(menuParam);
    }
    public void callcustomize(){
        isEditing = true;
        barreMenu.montrerCustomize();
    }
    public void callsettings(){
        barreMenu.montrerSettings();
    }
    public void callprincipal(){
        if(isEditing) {
            if(hasChangedSinceLastSave) {
                savebotbutton();
            }
            
            isEditing = false;
        }
        PanneauMulti.SetActive(false);
        barreMenu.montrerPrincipal();
    }
    public void callsubCustomize(){
        barreMenu.montrerSubMenuCustom(menuParam);
    }
    public void calldisconnect(){
        PlayerPrefs.SetString("apiKey","ERASED");
        GameManager.instance.changeScene(Scenes.Login);
    }
    public void callmultiplayer() {
        PanneauMulti.SetActive(true);
    }

    public void savebotbutton(){
        GameObject modal = Instantiate(modalBox,new Vector3(0, 0, 0), Quaternion.identity);
        modal.transform.SetParent (GameObject.FindGameObjectWithTag("Canvas").transform, false);
        modal.GetComponent<Modal>().init("Save Bot before exiting edit mode?",saveModalCallback,"Yes","No",spawnbotEdit);
    }
    public void  saveModalCallback() {
        saveBot(showedBot.saveBotPieces());
    }
    public void trashCanShortcut(){
        trashItem();
    }

    private void saveBot(string[] jsonPiecesProcessed) {
        string botid = GameManager.instance.GetActiveBot.botID;
        string query = "&action=saveBot&jsonPieces="+jsonPiecesProcessed[0]+"&baseName="+jsonPiecesProcessed[1]+"&botID="+botid;
        print(query);
        StartCoroutine(DatabaseManager.instance.Query(saveCallback , query));
    }

    public void saveCallback(string inutile) {
        print("saving game");
        StartCoroutine(DatabaseManager.instance.loadBotsEdit());
    }

    public void editorInfoFiller(EditorInfo editorPiece, string nomPiece) {
        editorPiece.nomItem = nomPiece;
        editorPiece.linkedAnchor = transitAnchor;
    }

    public bool SetClickPieces {
        get{return blockItemClick;}
        set{blockItemClick = value;}
    }

    public Bot ShowCasedBot {
        get {return showedBot;}
        set {showedBot = value;}
    }

    public string MenuParameters {
        get {return menuParam;}
        set {menuParam = value;}
    }


}

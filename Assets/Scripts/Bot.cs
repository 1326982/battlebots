/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
// Nom de la classe: Bot
// Fonctionnement de la classe: Contrôle les bots
// notes:
// bugs:
// todo:
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
/*-----------------------------------------------------------------------------------------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Bot : MonoBehaviour {
    private GameObject _impactParticle;
    private string _baseName;
    public bool isEdited = false;
    private Rigidbody _rbody;
    private GameObject _cible;
    private GameObject[] _nav;
    private GameObject _opponent;
    public GameObject linkedAnchor;
    private int botID;
    private GameObject _editingParent;
    private Destroyable _coreDest;
    private GameObject _core;

    private float _totalHP;
    private List<Destroyable> _tDestroyable = new List<Destroyable>{};

    private float tempsStagne = 0; // accumule le temps en fram que le gameobject n'a pas bougé
    private bool antiStagnationActive = true;

    private float _rotationSpeed = 0;
    private float _speed = 0;

    private int frameEnversMax = 300;
    private int frameEnversAct = 0;

    private BotState _state = BotState.Created;

    private Anchors _ancres;

    private causeOfDeath _causedeath;

    private bool _actif = false;
    /* LOCOMOTION */
    private List<Moteur> _rouesmoteur = new List<Moteur>{};



    private void Start() {
        _impactParticle = Resources.Load("sparkParticle") as GameObject;
    }
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    void FixedUpdate() {
        if(_actif){
            Debug.DrawLine(transform.position,_cible.transform.position,Color.red,0.1f);
            foncerSur(_cible.transform.position);
            antiStagnation();
            checkState();
        }    
        
    }

    public void checkState(){
        if(percentHP() <=0 ) {
                _causedeath = causeOfDeath.Totaled;
                BattleManager.instance.DeclareDeath(gameObject.name);
                deactivate();
        }else if(_coreDest == null) {
                _causedeath = causeOfDeath.CoreDestroyed;
                BattleManager.instance.DeclareDeath(gameObject.name);
                CamSelect cam = (gameObject.name == "visitor")?CamSelect.CoreVisitor:CamSelect.CoreLocal;
                BattleManager.instance.switchCam(cam);
                deactivate();
        }else if(verifierEnvers()) {
                _causedeath = causeOfDeath.UpsideDown;
                BattleManager.instance.DeclareDeath(gameObject.name);
                deactivate();
        }
    }

    public float percentHP(){
        float totalCheckedHP = _totalHP - ((_totalHP/100)*25);
        float diff = _totalHP - totalCheckedHP;
        float hpActPercent = Mathf.Clamp(((getHp()-diff)*100)/totalCheckedHP,0f,100f);
        return hpActPercent;
    }


    public void Build(BotBuilder instructions){
        _state = BotState.Building;
        _core = transform.Find("core").gameObject;
        _coreDest = _core.GetComponent<Destroyable>();
        _baseName = instructions.platform;
        _ancres = GetComponent<Anchors>(); 
        _rbody = GetComponent<Rigidbody>();
        _rotationSpeed = instructions.rotationSpeed;
        _speed = instructions.speed;
        getDestroyables(gameObject);
        connectParts(instructions.listParts, _ancres.anchorsList );
        _totalHP = getHp();
        if(isEdited) {
            transform.parent= MenuManager.instance.botContainer.transform;
            MenuManager.instance.ShowCasedBot = this;
             _rbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ ;//| RigidbodyConstraints.FreezePositionY;
             _rbody.freezeRotation = true;
             Destroy(_rbody);
        }
        _state=BotState.Ready;
    }
    string testjson(AnchorInfo[] list) {
        string listeTmp ="[";
        for(int i = 0;i<list.Length;i++ ){
            string tmp = list[i].sauvegarderAncre();
            if(i < list.Length-1) {
                listeTmp = listeTmp+tmp +"," ;
            }else {
                listeTmp = listeTmp+tmp;
            }
            
            
        }
        listeTmp+= "]";
        return listeTmp;
    }
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    private void connectParts(AnchorInfo[] list, GameObject[] goPosition) {
        for(int i = 0; i<goPosition.Length;i++){
            goPosition[i].GetComponent<AnchorPlace>().infos.anchorPos = i;
        }
        foreach(AnchorInfo info in list){
            
            //goPosition[info.anchorPos].transform.parent = null;
            string prefix = (isEdited)?"edit":"";
            GameObject itemTmp = Instantiate(loadGO(prefix+info.itemName),goPosition[info.anchorPos].transform.position,goPosition[info.anchorPos].transform.rotation );
            itemTmp.GetComponent<EditorInfo>().linkedAnchor = goPosition[info.anchorPos];
            itemTmp.GetComponent<EditorInfo>().linkedAnchor.GetComponent<AnchorPlace>().isOccupied = true;
            itemTmp.GetComponent<EditorInfo>().nomItem = info.itemName;
            
            if(itemTmp.tag == "wheel"){
                goPosition[info.anchorPos].GetComponent<AnchorPlace>().infos = info;
                getDestroyables(itemTmp);
                _rouesmoteur.Add(itemTmp.GetComponent<Moteur>());
                itemTmp.transform.parent= gameObject.transform;
                if(!isEdited){
                    itemTmp.GetComponent<Moteur>().activate();
                }

        
            }else if(itemTmp.tag == "mWeapon"){
                goPosition[info.anchorPos].GetComponent<AnchorPlace>().infos = info;
                getDestroyables(itemTmp);
                itemTmp.transform.parent= gameObject.transform;
            }
            if(!isEdited) {
                DestroySpawners(goPosition);
            }
        }
    }
    private void DestroySpawners(GameObject[] goPosition){
        foreach(GameObject go in goPosition){
            Destroy(go);
        }
    }
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    public GameObject loadGO(string nomItem) {
        return Resources.Load(nomItem) as GameObject;
    }

    public GameObject[] loadGoFromStringArray(string[] tmpArray){
        GameObject[] goArray= new GameObject[tmpArray.Length];
        for(int i =0;i<tmpArray.Length;i++) {
            goArray[i] = Resources.Load(tmpArray[i]) as GameObject;
        }
        return goArray;
    }

    public void activate() {
        if(!_actif) {
            findOpponent();
            _cible = _opponent;
            createNavMesh();
            _actif = true;
            foreach(Moteur roue in _rouesmoteur) {
                roue.SetVitesse = 5;
            }
        }
    }

    public void deactivate(){
        _actif = false;
        foreach(Moteur roue in _rouesmoteur) {
            roue.SetVitesse = 0;
        }
    }

    public string[] saveBotPieces() {
        List<AnchorPlace> temp = new List<AnchorPlace>(){};
        foreach(GameObject ancre in  _ancres.anchorsList) {
            AnchorPlace anchorPlaceAct = ancre.GetComponent<AnchorPlace>();
            if(anchorPlaceAct.isOccupied){
                temp.Add(anchorPlaceAct);
            }
        }

        List<string> listJsonPieces = new List<string>();
        temp.ForEach((x) => listJsonPieces.Add(JsonUtility.ToJson(x.infos)));
        string jsonPieces = "[";
        for(int i = 0; i<listJsonPieces.Count;i++) {
            jsonPieces += listJsonPieces[i];
            jsonPieces += (i!=listJsonPieces.Count-1)?",":"";
        }
        jsonPieces += "]";
        return new string [2] {jsonPieces , _baseName};
    }
    
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    void createNavMesh(){
        _nav = GameObject.FindGameObjectsWithTag("navPoint");
    }
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    void findOpponent() {
        if(gameObject.name == "local") {
            _opponent = GameObject.Find("visitor");
        }else {
            _opponent = GameObject.Find("local");
        }
    }


    public float getHp() {
        float tmp = 0;
        for(int i =0; i<_tDestroyable.Count;i++) {
            if(_tDestroyable[i] != null) {
                tmp += _tDestroyable[i].HP;
            }
        }
        return tmp;
    }

    private void getDestroyables(GameObject go){
        Destroyable[] tmp = go.GetComponentsInChildren<Destroyable>();
        foreach(Destroyable dest in tmp) {
            _tDestroyable.Add(dest);
        }

    }
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    private void antiStagnation()  {
        if(antiStagnationActive) {
            float vitesseAct = _rbody.velocity.magnitude;
            if(vitesseAct<0.2f) {
                tempsStagne++;
            }else {
                tempsStagne = 0;
            }
            if(tempsStagne >= 120) {
                tempsStagne = 0;
                StartCoroutine("contourner");
                StartCoroutine("rotationBoost");
            }
        }
        
    }

    private bool verifierEnvers(){
        if(transform.up.y < 0.7f){
            frameEnversAct++;
        }else {
            frameEnversAct = 0;
        }
        if(frameEnversAct >= frameEnversMax){
            return true;
        }
        return false;

    }
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    IEnumerator contourner() {
        GameObject memoire = _cible;
        _cible = _nav[UnityEngine.Random.Range(0,_nav.Length)];
        yield return new WaitForSeconds(3f);
        _cible = memoire;
        yield return null;
    }
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    IEnumerator rotationBoost() {
        float memoireSpeed = _speed;
        float memoireRotSpeed = _rotationSpeed;
        _rotationSpeed = memoireRotSpeed*100f;
        _speed = memoireSpeed*3;
        yield return new WaitForSeconds(1f);
        _rotationSpeed = memoireRotSpeed;
        _speed = memoireSpeed;
        yield return null;
    }
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    void foncerSur(Vector3 cible) {
        Vector3 targetDir = cible - transform.position;
        float angle = Vector3.SignedAngle(transform.forward, targetDir, Vector3.up );
        //Debug.DrawRay(transform.position, targetDir ,Color.red, 0.2f);
       // Debug.DrawRay(transform.position, transform.forward *2 ,Color.blue, 0.2f);

        
        if(transform.up.y > 0.9f && transform.position.y <0.25f ) {
            float precision = UnityEngine.Random.Range(14f,20f);
            if(angle > precision) {
                _rbody.AddTorque(transform.up*(_rotationSpeed/1000) * 1, ForceMode.Impulse);
                
            } else if(angle < -precision) {
                _rbody.AddTorque(transform.up*(_rotationSpeed/1000) * -1, ForceMode.Impulse);
            } else {
                // m.force = 1000 * _speed ;
                // m.targetVelocity = 1000 * _speed;
            }
            if(Mathf.Abs(angle)<90) {
                foreach(Moteur roue in _rouesmoteur) {
                  roue.SetVitesse = 3.5f;
                }
            }
        }else {
            foreach(Moteur roue in _rouesmoteur) {
                  roue.SetVitesse = 0;
                }
        }
        
    }

    private void OnCollisionStay(Collision other) {
        //Debug.Log("From "+gameObject.name+" ->Collider: "+ other.contacts[0].thisCollider.name + "collided with " + other.contacts[0].otherCollider);
        GameObject colliderEnfant = other.contacts[0].thisCollider.gameObject;
        GameObject autreCollider = other.contacts[0].otherCollider.gameObject;
        if(autreCollider.gameObject.tag == "weaponCollider") {
            Destroyable _destroyableTemp= other.contacts[0].thisCollider.gameObject.GetComponent<Destroyable>();
            if(_destroyableTemp != null) {
                Instantiate(_impactParticle,other.contacts[0].point,transform.rotation);
                Weapon compWeapon = autreCollider.GetComponent<Weapon>();
                int probabiliteKnockBack = Mathf.RoundToInt(UnityEngine.Random.Range(0,100));
                if(probabiliteKnockBack < 10) {
                    Vector3 direction = calculDirectionKnockback(other.contacts[0].point , transform.position);
                    _rbody.AddForce(direction*compWeapon.KnockBack, ForceMode.Impulse);
                }
                _destroyableTemp.endommager(compWeapon.ValDommage );
                
            }
        }
        

    }
    private Vector3 calculDirectionKnockback(Vector3 contact, Vector3 receveur){
        float DeltaX = receveur.x - contact.x;
        float DeltaY = receveur.y - contact.y;
        float DeltaZ = receveur.z - contact.z;
        return new Vector3(DeltaX,DeltaY,DeltaZ).normalized;
    }
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    public GameObject cible {
        get{return _cible;}
        set{_cible = value;}
    }

    public causeOfDeath DeathCause{
        get{return _causedeath;}
        set{_causedeath = value;}
    }

    public GameObject CoreGO {
        get{return _core;}
    }

    public BotState State {
        get{return _state;}
        set{_state = value;}
    }
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    /*------------------------------------------------------------ */
    public float MaxHP {
        get{return _totalHP;}
    }

}
public enum causeOfDeath {
    CoreDestroyed,Totaled,UpsideDown,Damage
}



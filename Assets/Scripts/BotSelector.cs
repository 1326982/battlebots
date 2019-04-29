using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSelector : MonoBehaviour {
    [SerializeField]int pos;

    public void select() {
        MenuManager.instance.switchbot(pos);
    }

}

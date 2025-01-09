using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// what in here is responsible for the "clicking"?

// where's this put on? on the melee, range, and run buttons in the ActionMenu:
public class MakeButton : MonoBehaviour
{
    [SerializeField]
    private bool physical;
    public GameObject hero;
    private GameObject actionMenu;
    private GameController gameControllerScript;

    void Awake() {
        gameControllerScript = GameObject.Find("GameControllerObject").GetComponent<GameController>();
    }

    void Start()
    {
        string temp = gameObject.name;
        gameObject.GetComponent<Button>().onClick.AddListener(() => AttachCallback(temp));

        hero = gameControllerScript.currentHeroObj;

        actionMenu = GameObject.Find("ActionMenu");
    }

    // where's this used? in this start()
    private void AttachCallback(string btn)
    {
        if (btn.CompareTo("MeleeBtn") == 0)
        {
            hero.GetComponent<FighterAction>().SelectAttack("melee");
        } 
        else if (btn.CompareTo("RangeBtn") == 0)
        {
            hero.GetComponent<FighterAction>().SelectAttack("range");
        } 
        else
        {
            hero.GetComponent<FighterAction>().SelectAttack("run");
        }

        DisableActionMenu();
    }

    private void DisableActionMenu()
    {
        actionMenu.SetActive(false);
    }   

    void OnMouseExit()
    {
        return;
    }
}

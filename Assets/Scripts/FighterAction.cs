using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// one of two scripts (other is FighterStats.cs) placed on the INDIVIDUAL character objects:
// placed on both friendlies and foes:
public class FighterAction : MonoBehaviour
{
    private GameObject hero;

    // Where is this set? in FighterStatsScript and GameController (???)
    public GameObject enemy;           // this should probably be set externally. 

    [SerializeField]
    private GameObject meleePrefab;

    [SerializeField]
    private GameObject rangePrefab;

    [SerializeField]
    private Sprite faceIcon;

    private GameObject currentAttack;

    // a SCRIPT:
    private GameController gameControllerScript;

    private GameController gameController;
    
    // serious rehauling needed:
    void Awake()
    {
        // gameController = gameController.Instance;
        gameControllerScript = GameObject.Find("GameControllerObject").GetComponent<GameController>();

        hero = GameObject.Find("WizardHero");

        // remember, selectedCharacter is the first child in "Enemies" parent:
        if (gameControllerScript.selectedCharacter != null) {
            Debug.Log("Default character READY TO KILL");
        } else {
            Debug.Log("Default character not found!");
        }
    }

    // 1 of 3 strings can be passed: "melee", "range", "run":
    public void SelectAttack(string btn)
    {
        GameObject victim = hero;

        if (name == "WizardHero")       // yes, object names can simply be retrieved like this
        {
            victim = enemy;
        }
        if (btn.CompareTo("melee") == 0)
        {
            meleePrefab.GetComponent<AttackScript>().Attack(victim);

        } else if (btn.CompareTo("range") == 0)
        {
            rangePrefab.GetComponent<AttackScript>().Attack(victim);
        }
    }
}

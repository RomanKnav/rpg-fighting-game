using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// one of two scripts (other is FighterStats.cs) placed on the INDIVIDUAL character objects:
// placed on both friendlies and foes:
public class FighterAction : MonoBehaviour
{
    // what's this for again? determine which character enemy attacks:
    // used externally? NOPE
    public GameObject hero;

    // Where is this set? in FighterStatsScript and GameController (???)
    public GameObject enemy;          

    [SerializeField]
    private GameObject meleePrefab;

    [SerializeField]    
    private GameObject rangePrefab;

    [SerializeField]

    // a SCRIPT:
    private GameController gameControllerScript;

    private GameController gameController;

    // NEED REFERENCE TO fighterscript. 
    public FighterStatsScript characterStatsScript;
    
    // serious rehauling needed:
    void Awake()
    {
        // gameController = gameController.Instance;
        gameControllerScript = GameObject.Find("GameControllerObject").GetComponent<GameController>();

        characterStatsScript = this.gameObject.GetComponent<FighterStatsScript>();
    }

    void Start() {
        // won't work if added to Awake():

        // this is the hero that all enemies will attack:
        hero = gameControllerScript.currentHeroObj;
        // hero = gameControllerScript.randomHeroObj;
    }

    // where is this used? in GameController's NextTurn() by enemy
    // and by AttachCallback() in MakeButton(), which handles the buttons (in MakeButton.cs):

    // DOESN'T RUN UNTIL ATTACK HAPPENS: 
    // ISSUE IS NOT HERE, STOP LOOKING:
    public void SelectAttack(string btn)
    {
        // for SELF:
        FighterStatsScript characterScript = gameObject.GetComponent<FighterStatsScript>();

        GameObject victim;

        // if it's an enemy attacking, attack the hero (here's what makes enemies attack the same hero):
        if (!characterScript.isFriendly) 
        {
            // victim = hero;
            // victim = gameControllerScript.GetRandomHero();
            victim = gameControllerScript.randomHeroObj;


            Debug.Log($"ATTACKING RANDOM HERO: {victim}");
        }
        else
        {
            victim = enemy;
             Debug.Log($"FUCKING VICTIM SET IN IF STATEMENT: {hero}"); 
        }

        // victim = enemy;

        /* when using CompareTo: 
            < 0, current precedes other object 
              0, current appears in the same position in the sort order as other object
            > 0, current is greater than other object
        */
        if (btn.CompareTo("melee") == 0)
        {
            meleePrefab.GetComponent<AttackScript>().Attack(victim);

        } else if (btn.CompareTo("range") == 0)
        {
            rangePrefab.GetComponent<AttackScript>().Attack(victim);
        }
    }
}
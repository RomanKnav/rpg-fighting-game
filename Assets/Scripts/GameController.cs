using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Transactions;
using UnityEngine.SocialPlatforms;
using System.Linq;

// MACRO FILE:

// put on GameControllerObject 

// since I use this crap everywhere, let's make it a singleton:
public class GameController : MonoBehaviour
{
    public GameObject actionMenu;

    public Text battleText;

    // MY CRAP:
    [Header("MY CRAP:")]
    [SerializeField] AudioSource musicSource1;
    public AudioSource musicSource;

    private GameObject friendliesParent;
    private GameObject enemiesParent;

    // LIST OF GAMEOBJECTS:
    public List<GameObject> charactersList;

    // MAKE THIS LIST OF CHARACTER SCRIPTS:
    public List<GameObject> priorityList = new List<GameObject>();     // this one MUST be initialized
    public List<FighterStatsScript> priorityScriptsList = new List<FighterStatsScript>();    

    private List<float> agilityPointsList = new List<float>();

    public FighterAction playerActionScript;
    public AttackScript playerAttackScript;         // this is on the PREFAB

    // CHARACTER SELECTION CRAP:
    // all 3 of these used EXTENSIVELY:
    public GameObject selectedCharacter;        // actual object of the selected character (there should be a default)
    public bool cursorAlreadyActive = true;
    public bool characterManuallySelected = false;

    public FighterStatsScript EnemyScript;
    private FighterStatsScript playerFighterStatsScript;    // formerly HeroScript
    public FighterStatsScript currentStatsScript;

    public GameObject currentHeroObj;                       // what's this? set by FindHeroes(). The first friendly found in PriorityList
    public GameObject secondHeroObj;

    // so that player can't attack while attacks are happening:
    public bool freeState = true;                           // true ONLY when player is able to select crap/enemies. False when attacking. 

    public bool movementHappening = false;                  // determines when there is MOVEMENT (from player and enemy)

    public bool aCharacterLockedIn = false;

    private void Awake()
    {
        charactersList = (GameObject.FindGameObjectsWithTag("Character")).ToList();
        CreatePriorityList();
        currentStatsScript = priorityScriptsList[0];
        FindHeroes();               // defines currentHeroObj and secondHeroObj

        playerActionScript = currentHeroObj.gameObject.GetComponent<FighterAction>();

        // what this do? get the WMeleePrefab, where attackScript is located:
        playerAttackScript = currentHeroObj.transform.GetChild(0).gameObject.GetComponent<AttackScript>();
        playerFighterStatsScript = currentHeroObj.GetComponent<FighterStatsScript>();

        actionMenu = GameObject.Find("ActionMenu");

        friendliesParent = GameObject.Find("Friendlies");                           // success
        enemiesParent = GameObject.Find("Enemies");

        // get default character to attack at start:
        if (enemiesParent.transform.childCount > 0) {
            selectedCharacter = enemiesParent.transform.GetChild(0).gameObject;     // selects first character listed in the parent
        }

        // what this do? if playerActionScript found, set the enemy in that script:
        if (playerActionScript != null) {
            playerActionScript.enemy = selectedCharacter;   // THIS IS NULL BY DEFAULT
        }

        // UPDATE THIS VAR DYNAMICALLY:
        EnemyScript = selectedCharacter.GetComponent<FighterStatsScript>();
    }

    // runs only ONCE:
    void Start()
    {
        musicSource = musicSource1;
        musicSource.Play();

        // gets INITIAL enemy:
        if (EnemyScript != null) {
            EnemyScript.SetEnemyThumbnail();
        }

        NextTurn();
    }

    void Update() {
        SetFreeState();
    }

    void SetFreeState() {
        if (movementHappening == true || currentStatsScript.isFriendly == false || !currentStatsScript.isFriendly) {
            freeState = false;
        } else {
            freeState = true;
        }
    }

    // REMEMBER: priorityList already sorted by agility points:
    public void FindHeroes() {
        bool firstFound = false;

        foreach (GameObject character in priorityList)
        {
            if (character.GetComponent<FighterStatsScript>().isFriendly == true) {
                if (!firstFound) {
                    currentHeroObj = character;
                    firstFound = true;
                } else {
                    secondHeroObj = character;
                    break;
                }
            }
        }
    }

    // if four characters are in battle scene, this adds SIX gameObjects. Why?
    public void CreatePriorityList() {
        if (charactersList.Count > 0) {
            foreach(GameObject character in charactersList) {

                var characterScript = character.GetComponent<FighterStatsScript>();
                var currentAgility = characterScript.agility;

                agilityPointsList.Add(currentAgility);
            }

            // SORT the list from greatest to least:
            agilityPointsList.Sort((a, b) => b.CompareTo(a));

            // loop through the floats in the list and set the priority list according to their agilityPoints:
            foreach(float highAgility in agilityPointsList) {
                
                foreach(GameObject character in charactersList) {

                    FighterStatsScript characterScript = character.GetComponent<FighterStatsScript>();
                    
                    var currentAgility = characterScript.agility;

                    // && character not in priorityList
                    if (currentAgility == highAgility && !priorityList.Contains(character)) {
                        priorityList.Add(character);
                        priorityScriptsList.Add(characterScript);
                    }
                }
            }
        }
    }

    // should NOT run at initial run (hence first if statement):

    // does this only work at init? NOPE. Works after an enemy is killed:
    public void AutoSelectNextEnemy() {
        if (selectedCharacter == null) {
            if (priorityList.Count > 0) {
                foreach (GameObject character in priorityList) 
                {
                    FighterStatsScript characterScript = character.GetComponent<FighterStatsScript>();

                    if (!characterScript.isFriendly && !characterScript.dead) {

                        characterScript.selected = true;

                        // global var in this file:
                        selectedCharacter = character;

                        // set enemy variable in FighterAction.cs (determine who to attack):
                        playerActionScript.enemy = selectedCharacter;

                        // draw cursor:
                        // irrelevant to the issue:
                        selectedCharacter.GetComponent<FighterStatsScript>().highlightCursor.gameObject.SetActive(true);

                        playerFighterStatsScript.SelectNewCharacter();

                        // should only happen ONCE per loop:
                        break;
                    }
                }
            }
        }
    }

    // Successfully replaced other crap with PriorityScriptsList:
    public void NextTurn()
    {
        // global variable battleText is the damage delt:
        battleText.gameObject.SetActive(false);

        // goes through the WHOLE LIST:
        FighterStatsScript currentFighterStatsScript = priorityScriptsList[0];
        currentStatsScript = priorityScriptsList[0];


        // why're we removing it?? removed only temporarily as their turn's being processed, then add it to back of list to be processed again later:
        priorityScriptsList.Remove(currentFighterStatsScript);

        if (!currentFighterStatsScript.GetDead())
        // if current fighter is NOT dead:
        {
            // from my understanding: we remove current script, get it's gameObj, then readd it immediatley???
            GameObject currentCharacterObj = currentFighterStatsScript.gameObject;

            // if not dead, just add it to the end (was at the start before):
            // readd the SCRIPT, not the game object:

            priorityScriptsList.Add(currentFighterStatsScript);
            
            Debug.Log($"CURRENT CHARACTER'S TURN: {currentCharacterObj.name}");

            // drawTheCircle NOT RUNNING FOR FRIENDLY:
            if (currentFighterStatsScript.isFriendly == true)
            {
                Debug.Log($"PRIORITY LIST INDEX 0 {currentFighterStatsScript.name}");       // successfully gets friendly guy
                currentFighterStatsScript.turnInProgress = true;
                currentFighterStatsScript.turnIsOver = false;

                // enable/disable respective circles:
                // playerFighterStatsScript.drawTheCircle = true;
                currentFighterStatsScript.drawTheCircle = true;

                // NOT GOOD: EnemyScript should be updated during EACH character's turn:
                EnemyScript.drawTheCircle = false;

                if (selectedCharacter != null) {
                    this.actionMenu.SetActive(true);
                } 
                // we want to automatically select next enemy character:
                else {
                   return; 
                }
            }
            
            // if player isn't current unit, undraw circle, disable actionMenu, and have enemy attack:
            else
            {
                EnemyScript = currentCharacterObj.GetComponent<FighterStatsScript>();
                // seems to only ever work with ONE enemy:
                currentFighterStatsScript.turnIsOver = false;

                // playerFighterStatsScript.drawTheCircle = false;
                currentFighterStatsScript.drawTheCircle = false;

                EnemyScript.drawTheCircle = true;

                this.actionMenu.SetActive(false);

                // what is this? select random attack for enemy
                string attackType = Random.Range(0, 2) == 1 ? "melee" : "range";
                currentCharacterObj.GetComponent<FighterAction>().SelectAttack(attackType);
            }
        } 
        // otherwise, current character is dead, move on to next:
        else
        {
            NextTurn();     // RECURSION!!!! To recurse through all the characters in the list
        }
    }
}
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
    // how to access singletons everywhere again?
    // public static GameController Instance { get; private set; } 

    public GameObject actionMenu;

    public Text battleText;

    // MY CRAP:
    [Header("MY CRAP:")]
    private GameObject friendliesParent;
    private GameObject enemiesParent;

    // LIST OF GAMEOBJECTS:
    public List<GameObject> charactersList;

    // MAKE THIS LIST OF CHARACTER SCRIPTS:
    public List<GameObject> priorityList = new List<GameObject>();     // this one MUST be initialized
    public List<FighterStatsScript> priorityScriptsList = new List<FighterStatsScript>();    

    private List<float> agilityPointsList = new List<float>();

    public FighterStatsScript EnemyScript;
    private FighterStatsScript HeroScript;
    public string nextPlayerAction;

    public GameObject playerObject;

    // FYI: this is already attained in FighterAction.cs:
    public GameObject playerMeleePrefab;            // need this to access "VictimAnimator" crap.

    public FighterAction playerActionScript;
    public AttackScript playerAttackScript;         // this is on the PREFAB

    // CHARACTER SELECTION CRAP:
    // all 3 of these used EXTENSIVELY:
    public GameObject selectedCharacter;        // actual object of the selected character (there should be a default)
    public bool cursorAlreadyActive = true;

    private void Awake()
    {
        playerObject = GameObject.Find("WizardHero");
        playerActionScript = GameObject.Find("WizardHero").GetComponent<FighterAction>();

        playerAttackScript = playerObject.transform.GetChild(0).gameObject.GetComponent<AttackScript>();

        actionMenu = GameObject.Find("ActionMenu");

        // seems unecessary to include this on EVERY character:
        friendliesParent = GameObject.Find("Friendlies");           // success
        enemiesParent = GameObject.Find("Enemies");

        // get default character to attack at start:
        if (enemiesParent.transform.childCount > 0) {
            selectedCharacter = enemiesParent.transform.GetChild(0).gameObject;
        }

        // what this do? if playerActionScript found, set the enemy in that script:
        if (playerActionScript != null) {
            playerActionScript.enemy = selectedCharacter;   // THIS IS NULL BY DEFAULT
        }

        charactersList = (GameObject.FindGameObjectsWithTag("Character")).ToList();

        HeroScript = GameObject.Find("WizardHero").GetComponent<FighterStatsScript>();

        // UPDATE THIS VAR DYNAMICALLY:
        EnemyScript = selectedCharacter.GetComponent<FighterStatsScript>();
    }

    // runs only ONCE:
    void Start()
    {
        CreatePriorityList();

        // gets INITIAL enemy:
        if (EnemyScript != null) {
            EnemyScript.SetEnemyThumbnail();
        }

        NextTurn();
    }

    // if four characters are in battle scene, this adds SIX gameObjects. Why?
    public void CreatePriorityList() {
        if (charactersList.Count > 0) {
            Debug.Log($"Inside charactersList: {charactersList.Count}");   // successfully returns 3

            foreach(GameObject character in charactersList) {

                var characterScript = character.GetComponent<FighterStatsScript>();
                var currentAgility = characterScript.agility;

                agilityPointsList.Add(currentAgility);
            }

            // SORT the list from greatest to least:
            agilityPointsList.Sort((a, b) => b.CompareTo(a));

            // add to priority list in descending order according to agility of each character (SUCCESS):
            Debug.Log($"Inside agilityPointsList: {agilityPointsList.Count}");  

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

            Debug.Log($"Inside PriorityList: {priorityList.Count}");
            foreach (GameObject character in priorityList)
            {
                Debug.Log(character);
            }
        }
    }

    // should NOT run at initial run (hence first if statement):
    public void AutoSelectNextEnemy() {
        if (selectedCharacter == null) {
            if (priorityList.Count > 0) {
                foreach (GameObject character in priorityList) 
                {
                    FighterStatsScript characterScript = character.GetComponent<FighterStatsScript>();

                    if (!characterScript.isFriendly && !characterScript.dead) {

                        // set enemy variable in FighterAction.cs (determine who to attack):
                        playerActionScript.enemy = selectedCharacter;

                        // global var in this file:
                        selectedCharacter = character;

                        // draw cursor:
                        selectedCharacter.GetComponent<FighterStatsScript>().highlightCursor.gameObject.SetActive(true);

                        // EnemyScript = selectedCharacter.GetComponent<FighterStatsScript>();

                        HeroScript.SelectNewCharacter();

                        // should only happen ONCE per loop:
                        break;
                    }
                }
                Debug.Log("AUTOMATICALLY SELECTED NEW ENEMY");
            }
        }
    }

    // Successfully replaced other crap with PriorityScriptsList:
    public void NextTurn()
    {
        // global variable battleText is the damage delt:
        battleText.gameObject.SetActive(false);

        FighterStatsScript currentFighterStatsScript = priorityScriptsList[0];

        // why're we removing it?? removed only temporarily as their turn's being processed, then add it to back of list to be processed again later:

        priorityScriptsList.Remove(currentFighterStatsScript);

        if (!currentFighterStatsScript.GetDead())
        // if current fighter is NOT dead:
        {
            // from my understanding: we remove current script, get it's gameObj, then readd it immediatley???
            GameObject currentGameObj = currentFighterStatsScript.gameObject;

            // if not dead, just add it to the end (was at the start before):
            // readd the SCRIPT, not the game object:

            priorityScriptsList.Add(currentFighterStatsScript);

            if (currentGameObj.name == "WizardHero")
            {
                currentFighterStatsScript.turnInProgress = true;
                currentFighterStatsScript.turnIsOver = false;

                // enable/disable respective circles:
                HeroScript.drawTheCircle = true;

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
                EnemyScript = currentGameObj.GetComponent<FighterStatsScript>();
                // seems to only ever work with ONE enemy:
                currentFighterStatsScript.turnIsOver = false;
                HeroScript.drawTheCircle = false;
                EnemyScript.drawTheCircle = true;

                this.actionMenu.SetActive(false);

                // what is this? select random attack for enemy
                string attackType = Random.Range(0, 2) == 1 ? "melee" : "range";
                currentGameObj.GetComponent<FighterAction>().SelectAttack(attackType);

                // looks like a nice place to disable the circle:
                // currentGameObj
            }
        } 
        // otherwise, current character is dead, move on to next:
        else
        {
            NextTurn();     // RECURSION!!!! To recurse through all the characters in the list
        }
    }
}
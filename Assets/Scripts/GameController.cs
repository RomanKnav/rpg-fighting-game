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

    // what exactly is this? A FUCKING LIST OF SCRIPTS!!!
    // what's it for? used in NextTurn()
    private List<FighterStatsScript> fighterStatsScriptList = new List<FighterStatsScript>();

    public GameObject actionMenu;

    public Text battleText;

    // MY CRAP:
    [Header("MY CRAP:")]
    private GameObject friendliesParent;
    private GameObject enemiesParent;

    // LIST OF GAMEOBJECTS:
    public List<GameObject> charactersList;
    public List<GameObject> priorityList = new List<GameObject>();     // this one MUST be initialized
    private List<float> agilityPointsList = new List<float>();

    private FighterStatsScript EnemyScript;
    private FighterStatsScript HeroScript;

    public bool aCharacterIsSelected;   
    public string nextPlayerAction;

    public GameObject playerObject;

    // FYI: this is already attained in FighterAction.cs:
    public GameObject playerMeleePrefab;            // need this to access "VictimAnimator" crap.

    public FighterAction playerActionScript;
    public AttackScript playerAttackScript;         // this is on the PREFAB

    // this'll need to be exported elsewhere:
    // so far, only used to set default character:
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
        EnemyScript = selectedCharacter.GetComponent<FighterStatsScript>();
    }

    // runs only ONCE:
    // maybe right off the start, add all the characters to fighterStatsScriptList?
    // this only adds TWO characters to fighterStatsScriptList:
    void Start()
    {
        CreatePriorityList();

        // gets INITIAL enemy:
        if (EnemyScript != null) {
            EnemyScript.SetEnemyThumbnail();
        }

        GameObject hero = GameObject.Find("WizardHero");

        FighterStatsScript currentHeroStats = hero.GetComponent<FighterStatsScript>();

        // hero's is first script added to fighterStatsScriptList:
        fighterStatsScriptList.Add(currentHeroStats);       

        GameObject enemy = selectedCharacter;

        // add first enemy's statsScript to list:
        FighterStatsScript currentEnemyStats = enemy.GetComponent<FighterStatsScript>();
        
        fighterStatsScriptList.Add(currentEnemyStats);

        // TODO: add rest of character's scripts to fighterStatsScriptList:
        NextTurn();
    }

    public void CreatePriorityList() {
        if (charactersList.Count > 0) {
            Debug.Log($"Inside charactersList: {charactersList.Count}");   // successfully returns 3

            foreach(GameObject character in charactersList) {

                var characterScript = character.GetComponent<FighterStatsScript>();
                var currentAgility = characterScript.agility;

                agilityPointsList.Add(currentAgility);
            }

            // SORT the list:
            agilityPointsList.Sort((a, b) => b.CompareTo(a));

            // add to priority list in descending order according to agility of each character (SUCCESS):
            Debug.Log($"Inside agilityPointsList: {agilityPointsList.Count}");  

            foreach(float highAgility in agilityPointsList) {
                
                foreach(GameObject character in charactersList) {
                    
                    var currentAgility = character.GetComponent<FighterStatsScript>().agility;

                    if (currentAgility == highAgility) {
                        priorityList.Add(character);
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
    // so this does it's job, but attacking does NOTHING.
    // must derive the animator of next selected enemy to set as "VictimAnimator" in Hero's AttackScript:
    public void AutoSelectNextEnemy() {
        if (!aCharacterIsSelected && selectedCharacter == null) {
            if (priorityList.Count > 0) {
                foreach (GameObject character in priorityList) 
                {
                    FighterStatsScript characterScript = character.GetComponent<FighterStatsScript>();

                    if (!characterScript.isFriendly && !characterScript.dead) {

                        // set enemy variable in FighterAction.cs (determine who to attack):
                        playerActionScript.enemy = selectedCharacter;

                        // global var in this file:
                        selectedCharacter = character;

                        // in this file:
                        aCharacterIsSelected = true;  

                        // draw cursor:
                        selectedCharacter.GetComponent<FighterStatsScript>().highlightCursor.gameObject.SetActive(true);

                        HeroScript.SelectNewCharacter();

                        // should only happen ONCE per loop:
                        break;
                    }
                }
                Debug.Log("AUTOMATICALLY SELECTED NEW ENEMY");
            }
        }
    }

    // still have no fucking idea how this works:
    public void NextTurn()
    {
        // global variable battleText is the damage delt:
        battleText.gameObject.SetActive(false);

        FighterStatsScript currentFighterStatsScript = fighterStatsScriptList[0];

        // why're we removing it?? removed only temporarily as their turn's being processed:
        fighterStatsScriptList.Remove(currentFighterStatsScript);

        if (!currentFighterStatsScript.GetDead())
        // if current fighter is NOT dead:
        {
            GameObject currentGameObj = currentFighterStatsScript.gameObject;

            // this would just add it to the end (was at the start before):
            fighterStatsScriptList.Add(currentFighterStatsScript);

            if (currentGameObj.name == "WizardHero")
            {
                currentFighterStatsScript.turnIsOver = false;

                // enable/disable respective circles:
                HeroScript.drawCircle = true;
                EnemyScript.drawCircle = false;

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
                // seems to only ever work with ONE enemy:
                currentFighterStatsScript.turnIsOver = false;
                HeroScript.drawCircle = false;
                EnemyScript.drawCircle = true;

                this.actionMenu.SetActive(false);

                // what is this? select random attack for enemy
                string attackType = Random.Range(0, 2) == 1 ? "melee" : "range";
                currentGameObj.GetComponent<FighterAction>().SelectAttack(attackType);
            }
        } 
        else
        {
            NextTurn();     // RECURSION!!!!
        }
    }
}
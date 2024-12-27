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

    // what exactly is this? SCRIPT!!!  
    private List<FighterStatsScript> fighterStatsScript = new List<FighterStatsScript>();

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

    private void Awake()
    {
        playerObject = GameObject.Find("WizardHero");
        playerActionScript = GameObject.Find("WizardHero").GetComponent<FighterAction>();

        actionMenu = GameObject.Find("ActionMenu");

        // seems unecessary to include this on EVERY character:
        friendliesParent = GameObject.Find("Friendlies");           // success
        enemiesParent = GameObject.Find("Enemies");

        // get default character to attack at start:
        if (enemiesParent.transform.childCount > 0) {
            selectedCharacter = enemiesParent.transform.GetChild(0).gameObject;
        }

        if (playerActionScript != null) {
            playerActionScript.enemy = selectedCharacter;   // THIS IS NULL BY DEFAULT
        }

        charactersList = (GameObject.FindGameObjectsWithTag("Character")).ToList();

        HeroScript = GameObject.Find("WizardHero").GetComponent<FighterStatsScript>();
        EnemyScript = selectedCharacter.GetComponent<FighterStatsScript>();
    }

    // runs only ONCE:
    void Start()
    {
        CreatePriorityList();

        if (EnemyScript != null) {
            EnemyScript.SetEnemyThumbnail();
            EnemyScript.highlightCursor.gameObject.SetActive(true);
        }

        GameObject hero = GameObject.Find("WizardHero");

        // is not being set:
        FighterStatsScript currentHeroStats = hero.GetComponent<FighterStatsScript>();

        // what is CalculateNextTurn()? takes integer argument.
        currentHeroStats.CalculateNextTurn(0);
        fighterStatsScript.Add(currentHeroStats);

        GameObject enemy = selectedCharacter;
        FighterStatsScript currentEnemyStats = enemy.GetComponent<FighterStatsScript>();
        
        currentEnemyStats.CalculateNextTurn(0);
        fighterStatsScript.Add(currentEnemyStats);

        fighterStatsScript.Sort();

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
                    if (!character.GetComponent<FighterStatsScript>().isFriendly) {
                        playerActionScript.enemy = selectedCharacter;
                        selectedCharacter = character;
                        aCharacterIsSelected = true;  


                        
                        // should only happen ONCE per loop:
                        break;
                    }
                }
                Debug.Log("AUTOMATICALLY SELECTED NEW ENEMY");
            }
        }
    }

    public void NextTurn()
    {
        battleText.gameObject.SetActive(false);
        FighterStatsScript currentFighterStatsScript = fighterStatsScript[0];
        fighterStatsScript.Remove(currentFighterStatsScript);
        if (!currentFighterStatsScript.GetDead())
        {
            GameObject currentUnit = currentFighterStatsScript.gameObject;
            currentFighterStatsScript.CalculateNextTurn(currentFighterStatsScript.nextActTurn);
            fighterStatsScript.Add(currentFighterStatsScript);
            fighterStatsScript.Sort();
            if (currentUnit.name == "WizardHero")
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
                    
                }
                
            }
            
            // if player is not current unit:
            else
            {
                currentFighterStatsScript.turnIsOver = false;
                HeroScript.drawCircle = false;
                EnemyScript.drawCircle = true;

                this.actionMenu.SetActive(false);

                // what is this? select random attack for enemy
                string attackType = Random.Range(0, 2) == 1 ? "melee" : "range";
                currentUnit.GetComponent<FighterAction>().SelectAttack(attackType);
            }
        } 
        else
        {
            NextTurn();
        }
    }
}
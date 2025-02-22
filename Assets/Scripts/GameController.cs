﻿using System.Collections;
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
    public List<GameObject> heroesList;
    public List<GameObject> EnemiesList;
    public List<GameObject> charactersList;

    // MAKE THIS LIST OF CHARACTER SCRIPTS:
    public List<GameObject> priorityList = new List<GameObject>();     // this one MUST be initialized
    public List<FighterStatsScript> priorityScriptsList = new List<FighterStatsScript>();

    private List<float> agilityPointsList = new List<float>();

    // this update after every turn? YES
    public FighterAction playerActionScript;
    public AttackScript playerAttackScript;         // this is on the PREFAB

    // CHARACTER SELECTION CRAP:
    // all 3 of these used EXTENSIVELY:
    public GameObject selectedCharacter;        // actual object of the selected character (there should be a default)
    public bool cursorAlreadyActive = true;
    public bool characterManuallySelected = false;

    public FighterStatsScript EnemyScript;                  // how's this initially assigned? first child of "enemiesParent"
    private FighterStatsScript playerFighterStatsScript;    // formerly HeroScript
    public FighterStatsScript currentStatsScript;

    // purpose of these is to have a secondary option to fall back to when ally 1 dies:
    public GameObject currentHeroObj;     // what's this? set by SetHeroes(). The first friendly found in PriorityList

    // i FORGOT what the purpose of this is! We used to have "playerObject = GameObject.Find("WizardHero");" hardcoded
    public GameObject secondHeroObj;

    // SUCCESS:
    public GameObject randomHeroObj;
    // make it so that enemies attack a random hero each time. Should change after every turn.

    // so that player can't attack while attacks are happening:
    public bool freeState = true;                           // true ONLY when player is able to select crap/enemies. False when attacking. 

    public bool movementHappening = false;                  // determines when there is MOVEMENT (from player and enemy)
    public bool aCharacterLockedIn = false;

    // new button crap:
    public List<GameObject> buttonsList;        // list of gameObjects
    public MakeButton makeButtonScript;

    private void Awake()
    {
        buttonsList = (GameObject.FindGameObjectsWithTag("Button")).ToList();

        // this originally an array:
        // where for loop used for this? CreatePriorityList()
        charactersList = (GameObject.FindGameObjectsWithTag("Character")).ToList(); ;

        CreatePriorityList();
        SetHeroes();               // defines currentHeroObj and secondHeroObj
        currentStatsScript = priorityScriptsList[0];

        playerActionScript = currentHeroObj.gameObject.GetComponent<FighterAction>();

        // what this do? get the WMeleePrefab, where attackScript is located:
        playerAttackScript = currentHeroObj.transform.GetChild(0).gameObject.GetComponent<AttackScript>();
        playerFighterStatsScript = currentHeroObj.GetComponent<FighterStatsScript>();

        actionMenu = GameObject.Find("ActionMenu");

        friendliesParent = GameObject.Find("Friendlies");                           // success
        enemiesParent = GameObject.Find("Enemies");

        // get default character to attack at start:
        if (enemiesParent.transform.childCount > 0)
        {
            selectedCharacter = enemiesParent.transform.GetChild(0).gameObject;     // selects first character listed in the parent
        }

        // set name of INITIAL enemy to attack:
        GameObject oppNameObject = GameObject.Find("EnemyFrameLabel");
        oppNameObject.GetComponent<Text>().text = selectedCharacter.name;

        // UPDATE THIS VAR DYNAMICALLY:
        EnemyScript = selectedCharacter.GetComponent<FighterStatsScript>();
    }

    // runs only ONCE:
    void Start()
    {
        musicSource = musicSource1;
        musicSource.Play();

        // gets INITIAL enemy:
        if (EnemyScript != null)
        {
            EnemyScript.SetEnemyThumbnail();
        }

        NextTurn();
    }

    void Update()
    {
        SetFreeState();
    }

    // REMEMBER: priorityList already sorted by agility points:
    public void SetHeroes()
    {
        bool firstFound = false;

        foreach (GameObject character in priorityList)
        {
            Debug.Log($"{character.name}, isFriendly: {character.GetComponent<FighterStatsScript>().isFriendly}");


            if (character.GetComponent<FighterStatsScript>().isFriendly == true)
            {

                if (!firstFound)
                {
                    currentHeroObj = character;
                    firstFound = true;
                }
                else
                {
                    secondHeroObj = character;
                    break;
                }
            }
        }

        Debug.Log($"HERO LIST LENGTH: {heroesList.Count}");
        SetRandomHero();
    }

    public void SetRandomHero()
    {
        System.Random random = new System.Random();

        // Get a random index
        int randomIndex = random.Next(heroesList.Count);
        randomHeroObj = heroesList[randomIndex];
    }


    void SetFreeState()
    {
        if (movementHappening == true || currentStatsScript.isFriendly == false || !currentStatsScript.isFriendly)
        {
            freeState = false;
        }
        else
        {
            freeState = true;
        }
    }

    // responsible for setting "hero" in buttons to next hero:
    // best place to put this?
    void resetButtons()
    {
        if (buttonsList.Count > 0)
        {
            foreach (GameObject button in buttonsList)
            {
                button.GetComponent<MakeButton>().hero = currentHeroObj;
            }
        }
    }

    // if four characters are in battle scene, this adds SIX gameObjects. Why?
    public void CreatePriorityList()
    {
        if (charactersList.Count > 0)
        {
            foreach (GameObject character in charactersList)
            {
                var characterScript = character.GetComponent<FighterStatsScript>();
                var currentAgility = characterScript.agility;
                agilityPointsList.Add(currentAgility);

                if (characterScript.isFriendly == true)
                {
                    Debug.Log($"ADDING TO HERO LIST: {character.name}");
                    heroesList.Add(character);
                }
                else {
                    EnemiesList.Add(character);
                }
            }

            // SORT the list from greatest to least:
            agilityPointsList.Sort((a, b) => b.CompareTo(a));

            foreach (float highAgility in agilityPointsList)
            {

                foreach (GameObject character in charactersList)
                {

                    FighterStatsScript characterScript = character.GetComponent<FighterStatsScript>();

                    var currentAgility = characterScript.agility;

                    if (currentAgility == highAgility && !priorityList.Contains(character))
                    {
                        priorityList.Add(character);
                        priorityScriptsList.Add(characterScript);
                    }
                }
            }
            Debug.Log($"PRIORITYLIST LENGTH: {priorityList.Count}");
        }
    }

    // Successfully replaced other crap with PriorityScriptsList:
    public void NextTurn()
    {
        // global variable battleText is the damage delt:
        battleText.gameObject.SetActive(false);

        // goes through the WHOLE LIST:
        FighterStatsScript currentFighterStatsScript = priorityScriptsList[0];

        // for the GLOBAL variable:
        currentStatsScript = priorityScriptsList[0];

        // if current-turn character is friendly:
        if (currentFighterStatsScript.isFriendly)
        {
            Debug.Log($"NEW ALLY TURN: {currentFighterStatsScript.name}");
            // MUST GET CURRENT ACTIONSCRIPT:
            // currentFighterStatsScript.playerActionScript = currentFighterStatsScript.gameObject.GetComponent<FighterAction>();

            if (playerActionScript.enemy == null) {
                currentFighterStatsScript.playerActionScript.enemy = selectedCharacter;
            }

            // what this? runs at start of each character's turn:
            if (selectedCharacter != null)
            {
                playerActionScript.enemy = selectedCharacter;
            }
            else
            {
                Debug.Log("SELECTEDCHARACTER NOT FOUND: ");
            }
        }

        // why're we removing it?? removed only temporarily as their turn's being processed, then add it to back of list to be processed again later:
        priorityScriptsList.Remove(currentFighterStatsScript);

        if (!currentFighterStatsScript.GetDead())
        // if current fighter is NOT dead:
        {
            // from my understanding: we remove current script, get it's gameObj, then readd it immediatley??? yes, add to the BACK: 
            GameObject currentCharacterObj = currentFighterStatsScript.gameObject;

            priorityScriptsList.Add(currentFighterStatsScript);

            SetRandomHero();

            Debug.Log($"CURRENT CHARACTER'S TURN: {currentCharacterObj.name}");

            // drawTheCircle NOT RUNNING FOR FRIENDLY:
            if (currentFighterStatsScript.isFriendly == true)
            {
                currentHeroObj = currentFighterStatsScript.gameObject;

                currentFighterStatsScript.turnInProgress = true;
                currentFighterStatsScript.turnIsOver = false;

                // enable/disable respective circles:
                currentFighterStatsScript.drawTheCircle = true;

                // what this do? makes so buttons makes current-turn hero take action. Used at new hero's turn:
                resetButtons();

                // NOT GOOD: EnemyScript should be updated during EACH character's turn:
                EnemyScript.drawTheCircle = false;

                // SWITCHING TO NEW HERO:
                playerAttackScript = currentHeroObj.transform.GetChild(0).gameObject.GetComponent<AttackScript>();
                playerActionScript = currentHeroObj.GetComponent<FighterAction>();

                // RESPONSIBLE FOR not showing action menu until a character is selected:
                if (selectedCharacter != null)
                {
                    this.actionMenu.SetActive(true);
                }
                // we want to automatically select next enemy character:
                else
                {
                    return;
                }
            }

            // if player isn't current unit, undraw circle, disable actionMenu, and have enemy attack:
            else
            {
                EnemyScript = currentCharacterObj.GetComponent<FighterStatsScript>();
                currentFighterStatsScript.turnIsOver = false;

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
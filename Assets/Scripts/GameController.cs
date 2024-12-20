using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Transactions;
using UnityEngine.SocialPlatforms;
using System.Linq;

// MACRO FILE:

// put on GameControllerObject 
public class GameController : MonoBehaviour
{
    private List<FighterStats> fighterStats;        // what's this for? Contains the stats of ONE character

    private GameObject battleMenu;

    public Text battleText;

    // MY CRAP:
    private GameObject friendliesParent;
    private GameObject enemiesParent;

    // is this a list or array? ARRAY:
    // private GameObject[] charactersList;
    // private GameObject[] priorityList;      // new, SORTED list of characters with correct priority.

    private List<GameObject> charactersList;
    private List<GameObject> priorityList = new List<GameObject>();     // this one MUST be initialized
    private List<float> agilityPointsList = new List<float>();

    private void Awake()
    {
        battleMenu = GameObject.Find("ActionMenu");

        // seems unecessary to include this on EVERY character:
        friendliesParent = GameObject.Find("Friendlies");           // success
        enemiesParent = GameObject.Find("Enemies");

        charactersList = (GameObject.FindGameObjectsWithTag("Character")).ToList();
    }

    // runs only ONCE:
    void Start()
    {
        // MY CRAP:
        if (friendliesParent != null && enemiesParent != null)
        {
            Debug.Log("character parent objects found");
        }

        CreatePriorityList();

        fighterStats = new List<FighterStats>();

        GameObject hero = GameObject.Find("WizardHero");

        // is not being set:
        FighterStats currentHeroStats = hero.GetComponent<FighterStats>();

        // what is CalculateNextTurn()? takes integer argument.
        currentHeroStats.CalculateNextTurn(0);
        fighterStats.Add(currentHeroStats);

        // GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        GameObject enemy = GameObject.Find("GiantEnemy");

        FighterStats currentEnemyStats = enemy.GetComponent<FighterStats>();
        currentEnemyStats.CalculateNextTurn(0);
        fighterStats.Add(currentEnemyStats);

        fighterStats.Sort();
        
        NextTurn();
    }

    public void CreatePriorityList() {
        if (charactersList.Count > 0) {
            Debug.Log("charactersList is filled!");
            Debug.Log($"Inside charactersList: {charactersList.Count}");                // successfully returns 3

            foreach(GameObject character in charactersList) {

                var characterScript = character.GetComponent<FighterStats>();
                var currentAgility = characterScript.agility;

                agilityPointsList.Add(currentAgility);
            }

            // SORT the list:
            agilityPointsList.Sort((a, b) => b.CompareTo(a));

            // add to priority list in descending order according to agility of each character (SUCCESS):
            Debug.Log($"Inside agilityPointsList: {agilityPointsList.Count}");  

            foreach(float highAgility in agilityPointsList) {
                
                foreach(GameObject character in charactersList) {
                    
                    var currentAgility = character.GetComponent<FighterStats>().agility;

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

    public void NextTurn()
    {
        battleText.gameObject.SetActive(false);
        FighterStats currentFighterStats = fighterStats[0];
        fighterStats.Remove(currentFighterStats);
        if (!currentFighterStats.GetDead())
        {
            GameObject currentUnit = currentFighterStats.gameObject;
            currentFighterStats.CalculateNextTurn(currentFighterStats.nextActTurn);
            fighterStats.Add(currentFighterStats);
            fighterStats.Sort();
            if (currentUnit.name == "WizardHero")
            {
                this.battleMenu.SetActive(true);
            } else
            {
                this.battleMenu.SetActive(false);
                string attackType = Random.Range(0, 2) == 1 ? "melee" : "range";
                currentUnit.GetComponent<FighterAction>().SelectAttack(attackType);
            }
        } else
        {
            NextTurn();
        }
    }
}

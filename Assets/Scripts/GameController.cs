using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Transactions;
using UnityEngine.SocialPlatforms;

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
    private GameObject[] charactersList;

    private void Awake()
    {
        battleMenu = GameObject.Find("ActionMenu");

        // seems unecessary to include this on EVERY character:
        friendliesParent = GameObject.Find("Friendlies");           // success
        enemiesParent = GameObject.Find("Enemies");

        charactersList = GameObject.FindGameObjectsWithTag("Character");
    }

    void Start()
    {
        // MY CRAP:
        if (friendliesParent != null && enemiesParent != null)
        {
            Debug.Log("character parent objects found");
        }

        if (charactersList.Length > 0) {
            Debug.Log("characterList is filled!");
        }

        fighterStats = new List<FighterStats>();

        GameObject hero = GameObject.Find("WizardHero");

        // is not being set:
        FighterStats currentHeroStats = hero.GetComponent<FighterStats>();
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

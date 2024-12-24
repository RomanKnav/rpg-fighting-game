using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// one of two scripts (other is FighterStats.cs) placed on the INDIVIDUAL character objects:
// responsible for what? 
public class FighterAction : MonoBehaviour
{
    private GameObject hero;

    private GameObject enemy;

    [SerializeField]
    private GameObject meleePrefab;

    [SerializeField]
    private GameObject rangePrefab;

    [SerializeField]
    private Sprite faceIcon;

    private GameObject currentAttack;

    // a SCRIPT:
    private GameController gameControllerScript;
    
    // serious rehauling needed:
    void Awake()
    {
        gameControllerScript = GameObject.Find("GameControllerObject").GetComponent<GameController>();

        hero = GameObject.Find("WizardHero");
        // enemy = GameObject.Find("GiantEnemy");

        // remember, selectedCharacter is the first child in "Enemies" parent:

        if (gameControllerScript.selectedCharacter != null) {
            enemy = gameControllerScript.selectedCharacter;
            Debug.Log("Default character READY TO KILL");
        } else {
            Debug.Log("Default character not found!");
        }
    }

    public void SelectAttack(string btn)
    {
        GameObject victim = hero;

        if (name == "WizardHero")                      // yes, object names can simply be retrieved like this
        {

            // this to NOT be immediately determined:
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

    // SHORTEN THIS CRAP:
    // public void SetActiveCharacter() {
    //     if (name == "WizardHero")
    //     {
    //         hero.GetComponent<FighterStatsScript>().actionReady = true;
    //         hero.GetComponent<FighterStatsScript>().victim = true;

    //         enemy.GetComponent<FighterStatsScript>().actionReady = false;
    //         enemy.GetComponent<FighterStatsScript>().victim = false;
    //     }
    //     else if (name == "GiantEnemy")
    //     {
    //         enemy.GetComponent<FighterStatsScript>().actionReady = true;
    //         enemy.GetComponent<FighterStatsScript>().victim = true; 

    //         hero.GetComponent<FighterStatsScript>().actionReady = false;
    //         hero.GetComponent<FighterStatsScript>().victim = false;           
    //     }
    // }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// one of two scripts (other is FighterStats.cs) placed on the INDIVIDUAL character objects:
// placed on both friendlies and foes:
public class FighterAction : MonoBehaviour
{
    private GameObject hero;

    public GameObject enemy;           // this should probably be set externally

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
        // enemy = gameControllerScript.selectedCharacter;

        // remember, selectedCharacter is the first child in "Enemies" parent:
        if (gameControllerScript.selectedCharacter != null) {
            // enemy = gameControllerScript.selectedCharacter;
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
            // Debug.Log($"Attacking enemy: {enemy.name}");
            meleePrefab.GetComponent<AttackScript>().Attack(victim);

        } else if (btn.CompareTo("range") == 0)
        {
            rangePrefab.GetComponent<AttackScript>().Attack(victim);
        }
    }
}

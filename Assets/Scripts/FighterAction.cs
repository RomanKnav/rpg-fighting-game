﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// one of two scripts (other is FighterStats.cs) placed on the INDIVIDUAL character objects:
// placed on both friendlies and foes:
public class FighterAction : MonoBehaviour
{
    private GameObject hero;

    // Where is this set? in FighterStatsScript and GameController (???)
    public GameObject enemy;           // this should probably be set externally. 

    [SerializeField]
    private GameObject meleePrefab;

    [SerializeField]
    private GameObject rangePrefab;

    [SerializeField]

    // a SCRIPT:
    private GameController gameControllerScript;

    private GameController gameController;
    
    // serious rehauling needed:
    void Awake()
    {
        // gameController = gameController.Instance;
        gameControllerScript = GameObject.Find("GameControllerObject").GetComponent<GameController>();
    }

    void Start() {
        // won't work if added to Awake():
        hero = gameControllerScript.currentHeroObj;
        Debug.Log($"HERE'S YOUR HERO: {hero}");
    }

    // where is this used? in GameController's NextTurn() by enemy
    // and by AttachCallback(), which handles the buttons (in MakeButton.cs):
    public void SelectAttack(string btn)
    {
        FighterStatsScript characterScript = gameObject.GetComponent<FighterStatsScript>();

        GameObject victim = hero;

        // checks name of gameObject:
        if (characterScript.isFriendly == true)
        {
            victim = enemy;
        }

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
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// where's this put on? on the melee, range, and run buttons in the ActionMenu:
public class MakeButton : MonoBehaviour
{
    [SerializeField]
    private bool physical;

    private GameObject hero;

    private GameObject actionMenu;
    void Start()
    {
        string temp = gameObject.name;
        gameObject.GetComponent<Button>().onClick.AddListener(() => AttachCallback(temp));

        hero = GameObject.Find("WizardHero");
        actionMenu = GameObject.Find("ActionMenu");
    }

    private void AttachCallback(string btn)
    {
        if (btn.CompareTo("MeleeBtn") == 0)
        {
            hero.GetComponent<FighterAction>().SelectAttack("melee");
        } else if (btn.CompareTo("RangeBtn") == 0)
        {
            hero.GetComponent<FighterAction>().SelectAttack("range");
        } else
        {
            hero.GetComponent<FighterAction>().SelectAttack("run");
        }

        DisableActionMenu();
    }

    private void DisableActionMenu()
    {
        actionMenu.SetActive(false);
        Debug.Log("Action menu disabled!");
    }
}

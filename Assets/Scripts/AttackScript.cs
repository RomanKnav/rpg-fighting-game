using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script placed on MeleePrefab and RangePrefab PREFABS!!! (which compose the character object children):
// so, this script used for BOTH the melee and ranged attacks:
public class AttackScript : MonoBehaviour
{
    // what is owner? The gameObject using the prefab. 
    public GameObject owner;

    public FighterStatsScript ownerStats;

    [SerializeField]
    private string animationName;

    [SerializeField]
    private bool magicAttack;

    [SerializeField]
    private float magicCost;

    [SerializeField]
    private float minAttackMultiplier;

    [SerializeField]
    private float maxAttackMultiplier;

    [SerializeField]
    private float minDefenseMultiplier;

    [SerializeField]
    private float maxDefenseMultiplier;

    private FighterStatsScript attackerStats;
    private FighterStatsScript targetStats;
    private float damage = 0.0f;

    // MY CRAP:
    public Animator ownerAnimator;
    public Animator victimAnimator;

    public void Awake() {
        ownerAnimator = owner.GetComponent<Animator>();
    }
    
    // TODO: get global var of victim:
    // where this used? FighterAction.cs
    public void Attack(GameObject victim)
    {
        victimAnimator = victim.GetComponent<Animator>();

        // stats of the one doing the attacking:
        attackerStats = owner.GetComponent<FighterStatsScript>();

        // stats of the one BEING attacked:
        targetStats = victim.GetComponent<FighterStatsScript>();

        if (!targetStats.GetDead())
        {
            // does melee use magic? NOPE
            if (attackerStats.magic >= magicCost && !attackerStats.turnIsOver)
            {
                float multiplier = Random.Range(minAttackMultiplier, maxAttackMultiplier);

                // we do melee attack by DEFAULT. This is simply setting the damage amount:
                damage = multiplier * attackerStats.melee;      // this is a FLOAT

                // otherwise, do the RANGED attack:
                if (magicAttack)
                {
                    damage = multiplier * attackerStats.magicRange;
                }

                float defenseMultiplier = Random.Range(minDefenseMultiplier, maxDefenseMultiplier);
                damage = Mathf.Max(0, damage - (defenseMultiplier * targetStats.defense));

                // animation crap. Where are animations assigned? On the characters themselves (they have an "Animator" component, 
                // which have a "controller", which contains MULTIPLE animations):

                // owner.GetComponent<Animator>().Play(animationName);    
                ownerAnimator.Play(animationName);  

                // THIS is what seems to update the health when enemies attacked:
                targetStats.ReceiveDamage(Mathf.CeilToInt(damage));
                attackerStats.updateMagicFill(magicCost);

                attackerStats.turnIsOver = true;        // should REMAIN true until it is their turn on the priority list
            } else
            {
                Invoke("SkipTurnContinueGame", 2);
            }  
        } else {
            // ownerAnimator.enabled = false;
            // return;
            Debug.Log($"{owner.name} is dead!!!");
        }
    }

    void SkipTurnContinueGame()
    {
        GameObject.Find("GameControllerObject").GetComponent<GameController>().NextTurn();
    }
}

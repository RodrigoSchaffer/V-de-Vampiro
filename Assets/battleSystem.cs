using System;
using System.Collections;
using System.Numerics;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;


public enum battleState { START, PLAYER_TURN, ENEMY_TURN, WON, LOST }

public class battleSystem : MonoBehaviour
{

    public GameObject player;

    public GameObject enemies;

    public Unit playerUnit;

    public Unit enemyUnit;

    public Transform battleStation;


    public battleState state;

    public int turnCount;

    public BattleHud playerHud;
    public BattleHud enemyHud;

    public Text combatLog;

    public AnimationController playerAnim;

    public AnimationController enemyAnim;




    void Start()
    {
        state = battleState.START;
        StartCoroutine(setUpBattle());

    }

    IEnumerator setUpBattle()
    {
        GameObject playerGO = Instantiate(player, battleStation.GetChild(0).transform);
        playerUnit = playerGO.GetComponent<Unit>();

        if (playerUnit.currentHp == 0)
        {
            playerUnit.currentHp = playerUnit.maxHp;
        }

        GameObject enemy = Instantiate(enemies.transform.
        GetChild(UnityEngine.Random.Range(0, 1)).gameObject,
        battleStation.GetChild(1).transform);
        enemyUnit = enemy.GetComponent<Unit>();
        enemyUnit._tag = UnitTag.ENEMY;

        enemyAnim = enemyUnit.GetComponentInParent<AnimationController>();
        playerAnim = playerUnit.GetComponent<AnimationController>();

        playerHud.setHuD(playerUnit);
        enemyHud.setHuD(enemyUnit);

        combatLog.text = "An enemy " + enemyUnit.unitName + " appears";

        enemyAnim.target = playerUnit;
        playerAnim.target = enemyUnit;

        yield return new WaitForSeconds(2f);

        state = battleState.PLAYER_TURN;
        playerTurn();



    }

    void playerTurn()
    {
        playerUnit.isBlocking = false;
        combatLog.text = "choose an action";


    }

    IEnumerator playerAttack()
    {
        state = battleState.ENEMY_TURN;

        playerAnim.dmg = playerUnit.unitDmg;
        playerAnim.PlayAction("Attack");
        
        
        

        yield return new WaitForSeconds(2f);

        isOver(enemyUnit);



    }

    public void basicAttack()
    {
        if (state != battleState.PLAYER_TURN)
        {
            return;
        }

        StartCoroutine(playerAttack());
    }

    public void leech()
    {
        if (state != battleState.PLAYER_TURN)
        {
            return;
        }

        StartCoroutine(leechAttack());
    }

    public void block()
    {
        if (state != battleState.PLAYER_TURN)
        {
            return;
        }

        StartCoroutine(blockAttacks());
    }

    IEnumerator leechAttack()
    {
        state = battleState.ENEMY_TURN;
        playerAnim.dmg = playerUnit.unitDmg - 5;
        playerAnim.PlayAction("Attack2");
        int leech = playerUnit.unitDmg - 5;
        if (playerUnit.currentHp >= playerUnit.maxHp)
        {
            playerUnit.currentHp = playerUnit.maxHp;
        }
        playerUnit.currentHp += leech / 2;
        
        

        yield return new WaitForSeconds(2f);

        isOver(enemyUnit);


    }

    IEnumerator blockAttacks()
    {
        state = battleState.ENEMY_TURN;
        playerUnit.isBlocking = true;

        yield return new WaitForSeconds(2f);

        isOver(enemyUnit);


    }

    IEnumerator enemyTurn()
    {

        enemyUnit.isBlocking = false;


        
        bool isDead = EnemyActionOptions();

        yield return new WaitForSeconds(2f);

        isOver(playerUnit);


    }

    public bool EnemyActionOptions()
    {
        if (enemyUnit.currentAp <= 1)
        {
            int randNum = UnityEngine.Random.Range(0, 10);
            if (randNum < 7)
            {
                combatLog.text = enemyUnit.unitName + " Used Slash";
                enemyAnim.dmg = enemyUnit.unitDmg;
                enemyAnim.PlayAction("Attack");
                


                return playerUnit.isDead();

            }
            else if (randNum > 7)
            {
                combatLog.text = enemyUnit.unitName + " Used Block";
                enemyUnit.isBlocking = true;
                enemyUnit.currentAp -= 1;
                return false;
            }


        }
        else if (enemyUnit.currentAp > 1)
        {
            int randNum = UnityEngine.Random.Range(0, 10);
            if (randNum <= 3)
            {
                combatLog.text = enemyUnit.unitName + " Used Slash";
                enemyAnim.target = playerUnit;
                enemyAnim.dmg = enemyUnit.unitDmg;
                enemyAnim.PlayAction("Attack");

                return playerUnit.isDead();

            }
            else if (randNum > 3 && randNum <= 5)
            {
                combatLog.text = enemyUnit.unitName + " Used Block";
                enemyUnit.isBlocking = true;
                enemyUnit.currentAp -= 1;
                return false;
            }
            else if (randNum > 5)
            {
                combatLog.text = enemyUnit.unitName + " Used Holy light";
                enemyUnit.currentAp -= 2;
                enemyAnim.dmg = enemyUnit.unitDmg + 10;
                enemyAnim.PlayAction("Attack2");
                return playerUnit.isDead();
            }

        }

        return false;


    }



    public void endBattle()
    {
        if (state == battleState.WON)
        {
            combatLog.text = "You Won!";

            
            StartCoroutine(restart());

        }
        else if (state == battleState.LOST)
        {
            combatLog.text = "You Lost.";

            StartCoroutine(restart());
        }


    }
    IEnumerator restart()
    {
        yield return new WaitForSeconds(2f);

        Start();
    }

    public void isOver(Unit unit)
    {
        
        if (unit._tag == UnitTag.ENEMY)
        {

            if (unit.state == UnitState.DEAD)
            {
                state = battleState.WON;
                endBattle();

            }
            else
            {
                StartCoroutine(enemyTurn());
            }
        }
        else
        {
            if (unit.state == UnitState.DEAD)
            {
                state = battleState.LOST;
                endBattle();

            }
            else
            {
                turnCount++;
                state = battleState.PLAYER_TURN;
                if (enemyUnit.currentAp < 2)
                {
                    enemyUnit.currentAp++;
                }
                playerTurn();
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        enemyHud.setHp(enemyUnit);
        playerHud.setHp(playerUnit);
      

    }

    
    

    
}

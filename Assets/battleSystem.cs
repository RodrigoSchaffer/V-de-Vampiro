using System;
using System.Collections;
using System.Numerics;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public enum battleState { START, PLAYER_TURN, ENEMY_TURN, WON, LOST }

public class battleSystem : MonoBehaviour
{

    public GameObject playerPrefab;

    public GameObject enemies;

    private Animator pAnim;
    private Animator enemyAnim;

    Unit playerUnit;
    
    Unit enemyUnit;

    public Transform battleStation;


    public battleState state;

    public int turnCount;

    public BattleHud playerHud;
    public BattleHud enemyHud;

    public Text combatLog;


    void Start()
    {
        pAnim = playerPrefab.GetComponent<Animator>();
        enemyAnim = enemies.transform.GetChild(0).GetComponent<Animator>();


        state = battleState.START;
        StartCoroutine(setUpBattle());

    }

    IEnumerator setUpBattle()
    {


        playerUnit = playerPrefab.GetComponent<Unit>();

        if (playerUnit.currentHp == 0) {
            playerUnit.currentHp = playerUnit.maxHp;
        }

        GameObject enemy = Instantiate(enemies.transform.
        GetChild(UnityEngine.Random.Range(0, 1)).gameObject,
        battleStation.GetChild(1).transform);
        enemyUnit = enemy.GetComponent<Unit>();

        playerHud.setHuD(playerUnit);
        enemyHud.setHuD(enemyUnit);

        combatLog.text = "An enemy " + enemyUnit.unitName + " appears";

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

        bool isDead = enemyUnit.takeDmg(playerUnit.unitDmg);
        enemyHud.setHp(enemyUnit);

        yield return new WaitForSeconds(2f);

        isOver(isDead, enemyUnit, true);

        
        
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

        int leechdmg = playerUnit.unitDmg - 10;

        
        if (playerUnit.currentHp >= playerUnit.maxHp)
        {
            playerUnit.currentHp = playerUnit.maxHp;
        }
        if (enemyUnit.isBlocking == true && leechdmg <= enemyUnit.block)
        {
            
        }
        else
        {
           playerUnit.currentHp += (leechdmg)/2; 
        }
        
        
        bool isDead = enemyUnit.takeDmg(leechdmg);
        enemyHud.setHp(enemyUnit);
        playerHud.setHp(playerUnit);

        yield return new WaitForSeconds(2f);

        isOver(isDead, enemyUnit, true);
        
        
    }

    IEnumerator blockAttacks()
    {
        state = battleState.ENEMY_TURN;
        playerUnit.isBlocking = true;

        bool isDead = false;

        yield return new WaitForSeconds(2f);

        isOver(isDead, enemyUnit, true);
        
        
    }

    IEnumerator enemyTurn()
    {
        
        enemyUnit.isBlocking = false;

        

        bool isDead = EnemyActionOptions();
        playerHud.setHp(playerUnit);
        yield return new WaitForSeconds(2f);

        isOver(isDead, playerUnit, false);

        
    }

    public bool EnemyActionOptions()
    {
        if (enemyUnit.currentAp <= 1)
        {
            int randNum = UnityEngine.Random.Range(0, 10);
            if (randNum < 7)
            {
                combatLog.text = enemyUnit.unitName + " Used Slash";
                return playerUnit.takeDmg(enemyUnit.unitDmg);
                
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
                return playerUnit.takeDmg(enemyUnit.unitDmg);
            }
            else if (randNum > 3 && randNum <= 5)
            {
                combatLog.text = enemyUnit.unitName + " Used Block";
                enemyUnit.isBlocking = true;
                enemyUnit.currentAp -= 1;
                return false;
            }
            else if (randNum > 5) {
                combatLog.text = enemyUnit.unitName + " Used Holy light";
                enemyUnit.currentAp -= 2;
                return playerUnit.takeDmg(enemyUnit.unitDmg + 10);
            }
            
        }

        return false;

        
    }



    public void endBattle()
    {
        if (state == battleState.WON)
        {
            combatLog.text = "You Won!";
            Destroy(enemyUnit);
            Start();
            
        }
        else if (state == battleState.LOST)
        {
            combatLog.text = "You Lost.";

        }


    }

    public void isOver(bool isDead, Unit unit, bool isEnemy)
    {
        if (isEnemy == true)
        {
            if (isDead)
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
            if (isDead)
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
 
    }
}

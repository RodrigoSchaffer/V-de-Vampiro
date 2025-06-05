using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.UI;


public enum battleState { START, PLAYER_TURN, ENEMY_TURN, WON, LOST }

public class battleSystem : MonoBehaviour
{

    public GameObject player;

    public List<GameObject> enemies;

    public Unit playerUnit;

    public Unit enemyUnit;

    public Transform battleStation;

    public float moveSpeed = 8f;
    public UnityEngine.Vector3 originalPosition;
    public float approachDistance = 1.5f;
    

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

        GameObject enemy = Instantiate(enemies[UnityEngine.Random.Range(0, 2)],
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

    private IEnumerator MoveToTargetAndBack(Unit unit, String action)
    {
        if (unit._tag == UnitTag.PLAYER) {

            originalPosition = battleStation.GetChild(0).position;
            Transform target = battleStation.GetChild(1);

            UnityEngine.Vector3 direction = (target.position - playerUnit.transform.position).normalized;

            yield return StartCoroutine(MoveToPosition(target.position - direction * approachDistance, playerUnit)); 

          
            playerAnim.PlayAction(action);

            yield return new WaitForSeconds(0.5f); 

            
            yield return StartCoroutine(MoveToPosition(originalPosition, playerUnit));
        }
        else {
            originalPosition = battleStation.GetChild(1).position;
            Transform target = battleStation.GetChild(0);

            UnityEngine.Vector3 direction = (target.position - enemyUnit.transform.position).normalized;


            
            yield return StartCoroutine(MoveToPosition(target.position - direction * approachDistance, enemyUnit));
            
            
            enemyAnim.PlayAction(action);
            

            
            yield return StartCoroutine(MoveToPosition(originalPosition, enemyUnit));
        }
    }

    private IEnumerator MoveToPosition(UnityEngine.Vector3 destination, Unit unit)
    {
        if (unit._tag == UnitTag.PLAYER)
        {


            while (UnityEngine.Vector3.Distance(playerUnit.transform.position, destination) > 0.05f)
            {
                playerUnit.transform.position = UnityEngine.Vector3.MoveTowards(playerUnit.transform.position, destination, moveSpeed * Time.deltaTime);
                yield return new WaitUntil(() => playerAnim.isPlayingAction == false);
            }

            playerUnit.transform.position = destination; 
        }
        else
        {
            while (UnityEngine.Vector3.Distance(enemyUnit.transform.position, destination) > 0.05f)
            {
                enemyUnit.transform.position = UnityEngine.Vector3.MoveTowards(enemyUnit.transform.position, destination, moveSpeed * Time.deltaTime);
                yield return new WaitUntil(() => enemyAnim.isPlayingAction == false);
            }

            enemyUnit.transform.position = destination;
        }
        
    }

    void playerTurn()
    {
        playerUnit.isBlocking = false;
        combatLog.text = "choose an action";


    }

    IEnumerator playerAttack()
    {
        state = battleState.ENEMY_TURN;

        combatLog.text = "Nyx used Attack";
        playerAnim.dmg = playerUnit.unitDmg;
        StartCoroutine(MoveToTargetAndBack(playerUnit, "Attack"));
        
        

        yield return new WaitForSeconds(3.5f);

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
        combatLog.text = "Nyx used Leech";
        playerAnim.dmg = playerUnit.unitDmg - 5;
        StartCoroutine(MoveToTargetAndBack(playerUnit, "Attack2"));
        int leech = playerUnit.unitDmg - 5;
        playerUnit.currentHp += leech / 2;
        
        

        yield return new WaitForSeconds(3.5f);

        isOver(enemyUnit);


    }

    IEnumerator blockAttacks()
    {
        state = battleState.ENEMY_TURN;
        combatLog.text = "Nyx used Block";
        playerUnit.isBlocking = true;

        yield return new WaitForSeconds(2f);

        isOver(enemyUnit);


    }

    IEnumerator enemyTurn()
    {

        enemyUnit.isBlocking = false;


        
        bool isDead = EnemyActionOptions();

        yield return new WaitForSeconds(3.5f);

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
                StartCoroutine(MoveToTargetAndBack(enemyUnit, "Attack"));
                
                


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
                enemyAnim.dmg = enemyUnit.unitDmg;
                StartCoroutine(MoveToTargetAndBack(enemyUnit, "Attack"));

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
                StartCoroutine(MoveToTargetAndBack(enemyUnit, "Attack2"));
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
            
            

        }
        else if (state == battleState.LOST)
        {
            combatLog.text = "You Lost.";

            
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
    public void gotHit(Unit unit)
    {
        if (unit.tookDmg == true)
        {

            unit.GetComponent<AnimationController>().PlayAction("Hit");
            unit.tookDmg = false;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        gotHit(playerUnit);
        gotHit(enemyUnit);

        enemyHud.setHp(enemyUnit);
        playerHud.setHp(playerUnit);
      

    }

    
    

    
}

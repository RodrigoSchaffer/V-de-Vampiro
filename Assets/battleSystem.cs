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

    public GameObject player0Prefab;
    public GameObject player1Prefab;

    public GameObject enemies;
    int actionsPerUnit = 2;

    /*
    public GameObject enemy0Prefab;
    public GameObject enemy1Prefab;
    */

    Unit playerUnit0;
    Unit playerUnit1;
    Unit enemyUnit0;
    Unit enemyUnit1;

    Unit enemyTestUnit;

    public Transform battleStation;


    public battleState state;

    void Start()
    {
        state = battleState.START;
        StartCoroutine(setUpBattle());

    }

    IEnumerator setUpBattle()
    {


        GameObject player0 = Instantiate(player0Prefab, battleStation.GetChild(0));
        playerUnit0 = player0.GetComponent<Unit>();

        GameObject player1 = Instantiate(player1Prefab, battleStation.GetChild(1));
        playerUnit0 = player1.GetComponent<Unit>();
        /*
        GameObject enemy0 = Instantiate(enemy0Prefab, battleStation.GetChild(2).transform);
        playerUnit0 = enemy0.GetComponent<Unit>();

        GameObject enemy1 = Instantiate(enemy1Prefab, battleStation.GetChild(3).transform);
        playerUnit0 = enemy1.GetComponent<Unit>();
        */

        GameObject enemy0 = Instantiate(enemies.transform.
        GetChild(UnityEngine.Random.Range(0, 1)).gameObject,
        battleStation.GetChild(2));
        enemyUnit0 = enemy0.GetComponent<Unit>();

        GameObject enemy1 = Instantiate(enemies.transform.
        GetChild(UnityEngine.Random.Range(0, 1)).gameObject,
        battleStation.GetChild(3));
        enemyUnit1 = enemy1.GetComponent<Unit>();

        yield return new WaitForSeconds(2f);


        state = battleState.PLAYER_TURN;
        playerTurn();


    }

    void playerTurn()
    {
        
        // Colocar HUD 

    }

    IEnumerator playerBasicAttack()
    {

        

        Unit currentUnit = null;
        if (actionsPerUnit == 2) {
            currentUnit = playerUnit0;
        }
        if (actionsPerUnit == 1)
        {
            currentUnit = playerUnit1;
        }
        actionsPerUnit--;

        Unit target = chooseTarget();
        bool isDead = target.takeDmg(currentUnit.unitDmg);

        yield return new WaitForSeconds(2f);

        if (isDead)
        {

            bool isLast = enemiesLeft();
            Destroy(target);
            if (isLast == true)
            {
                state = battleState.WON;
                endBattle();
            }
            else
            {
                if (actionsPerUnit > 0)
                {
                    state = battleState.PLAYER_TURN;
                }
                else
                {
                    state = battleState.ENEMY_TURN;   
                }
            }
        }
        else
        {
            if (actionsPerUnit > 0)
                {
                    state = battleState.PLAYER_TURN;
                }
                else
                {
                    state = battleState.ENEMY_TURN;   
                }
        }
        
    }

    public void basicAttack()
    {
        if (state != battleState.PLAYER_TURN)
        {
            return;
        }

        StartCoroutine(playerBasicAttack());
    }

    public Unit chooseTarget()
    {

        return enemyUnit0;
        
    }

    public bool enemiesLeft()
    {
        if (enemyUnit0.currentAp > 0 || enemyUnit1.currentHp > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void endBattle()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}

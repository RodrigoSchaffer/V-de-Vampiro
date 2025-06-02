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
        state = battleState.START;
        StartCoroutine(setUpBattle());

    }

    IEnumerator setUpBattle()
    {

        GameObject player = Instantiate(playerPrefab, battleStation.GetChild(0).transform);
        playerUnit = player.GetComponent<Unit>();
        
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
        combatLog.text = "choose an action";  
        

    }

    IEnumerator playerAttack()
    {
        state = battleState.ENEMY_TURN;

        bool isDead = enemyUnit.takeDmg(playerUnit.unitDmg);

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

    IEnumerator leechAttack()
    {
        state = battleState.ENEMY_TURN;

        playerUnit.currentHp += (playerUnit.unitDmg - 10)/2;
        if (playerUnit.currentHp >= playerUnit.maxHp)
        {
            playerUnit.currentHp = playerUnit.maxHp;
        }
        
        bool isDead = enemyUnit.takeDmg(playerUnit.unitDmg - 10);

        yield return new WaitForSeconds(2f);

        isOver(isDead, enemyUnit, true);
        
        
    }

    IEnumerator enemyTurn()
    {
        combatLog.text = enemyUnit.unitName + " Attacks";

        bool isDead = playerUnit.takeDmg(enemyUnit.unitDmg);

        yield return new WaitForSeconds(2f);

        isOver(isDead, enemyUnit, false);
        

    }



    public void endBattle()
    {
        if (state == battleState.WON)
        {
            combatLog.text = "You Won!";
            Destroy(enemyUnit);
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
                playerTurn();
            }
        }

        
    }

    // Update is called once per frame
    void Update()
    {
 
    }
}

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

        combatLog.text = "An enemy " + enemyUnit.unitName + "appears";

        yield return new WaitForSeconds(2f);

        state = battleState.PLAYER_TURN;
        playerTurn();


    }

    void playerTurn()
    {
        
        

    }

    IEnumerator playerAttack()
    {

        
        bool isDead = enemyUnit.takeDmg(playerUnit.unitDmg);

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = battleState.WON;
            endBattle();

        }
        else
        {
            state = battleState.ENEMY_TURN;
            StartCoroutine(enemyTurn());
        }
        
        
    }

    public void basicAttack()
    {
        if (state != battleState.PLAYER_TURN)
        {
            return;
        }

        StartCoroutine(playerAttack());
    }

    IEnumerator enemyTurn()
    {
        combatLog.text = enemyUnit.unitName + "Attacks";



        yield return new WaitForSeconds(2f);
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

    // Update is called once per frame
    void Update()
    {

    }
}

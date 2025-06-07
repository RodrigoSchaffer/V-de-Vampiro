using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public Vector3 originalPosition;
    public float approachDistance = 1.5f;
    public float waitSeconds = 3.5f;
    

    public battleState state;
    public GameObject playerGO = null;
    public GameObject enemy;
    

    public int turnCount;
    public int winCount;

    public BattleHud playerHud;
    public BattleHud enemyHud;

    public Text combatLog;

    public AnimationController playerAnim;

    public AnimationController enemyAnim;

    public Button strongAttack;
    public Button blockButton;




    void Start()
    {
        state = battleState.START;
        StartCoroutine(setUpBattle());

    }

    IEnumerator setUpBattle()
    {
        if (playerGO == null)
        {      
            playerGO = Instantiate(player, battleStation.GetChild(0).transform);
            playerUnit = playerGO.GetComponent<Unit>();
            winCount = 0;
        }

        if (playerUnit.currentHp == 0)
        {
            playerUnit.currentHp = playerUnit.maxHp;
        }

        enemy = Instantiate(enemies[Random.Range(0, 3)],
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

    private IEnumerator MoveToTargetAndBack(Unit unit, int attackIndex)
    {
        if (unit._tag == UnitTag.PLAYER) {

            originalPosition = battleStation.GetChild(0).position;
            Transform target = battleStation.GetChild(1);

            Vector3 direction = (target.position - playerUnit.transform.position).normalized;

            yield return StartCoroutine(MoveToPosition(target.position - direction * approachDistance, playerUnit));


            combatLog.text = unit.UseAttack(unit.attacks[attackIndex], playerAnim);

            yield return new WaitForSeconds(0.5f); 

            
            yield return StartCoroutine(MoveToPosition(originalPosition, playerUnit));
        }
        else {
            originalPosition = battleStation.GetChild(1).position;
            Transform target = battleStation.GetChild(0);

            Vector3 direction = (target.position - enemyUnit.transform.position).normalized;


            
            yield return StartCoroutine(MoveToPosition(target.position - direction * approachDistance, enemyUnit));
            
            
            combatLog.text = unit.UseAttack(unit.attacks[attackIndex], enemyAnim);
            

            
            yield return StartCoroutine(MoveToPosition(originalPosition, enemyUnit));
        }
    }

    private IEnumerator MoveToPosition(UnityEngine.Vector3 destination, Unit unit)
    {
        if (unit._tag == UnitTag.PLAYER)
        {


            while (Vector3.Distance(playerUnit.transform.position, destination) > 0.05f)
            {
                playerUnit.transform.position = Vector3.MoveTowards(playerUnit.transform.position, destination, moveSpeed * Time.deltaTime);
                yield return new WaitUntil(() => playerAnim.isPlayingAction == false);
            }

            playerUnit.transform.position = destination; 
        }
        else
        {
            while (Vector3.Distance(enemyUnit.transform.position, destination) > 0.05f)
            {
                enemyUnit.transform.position = Vector3.MoveTowards(enemyUnit.transform.position, destination, moveSpeed * Time.deltaTime);
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
        StartCoroutine(MoveToTargetAndBack(playerUnit, 0));
        
        

        yield return new WaitForSeconds(3f);

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
        StartCoroutine(MoveToTargetAndBack(playerUnit, 1));
        
        
        

        yield return new WaitForSeconds(3.5f);

        isOver(enemyUnit);


    }

    IEnumerator blockAttacks()
    {
        state = battleState.ENEMY_TURN;
        combatLog.text = "Nyx used Block";
        playerUnit.isBlocking = true;
        playerUnit.currentAp--;

        yield return new WaitForSeconds(2f);

        isOver(enemyUnit);


    }

    IEnumerator enemyTurn()
    {

        enemyUnit.isBlocking = false;


        
        EnemyActionOptions();

        yield return new WaitForSeconds(waitSeconds);
        waitSeconds = 3.5f;

        isOver(playerUnit);


    }

    public void EnemyActionOptions()
    {
        if (enemyUnit.currentAp <= 1)
        {
            int randNum = Random.Range(0, 10);
            if (randNum <= 7)
            {
                StartCoroutine(MoveToTargetAndBack(enemyUnit, 0));


            }
            else if (randNum > 7)
            {
                combatLog.text = enemyUnit.unitName + " Used Block";
                enemyUnit.isBlocking = true;
                enemyUnit.currentAp--;
                waitSeconds = 2f;
                
            }


        }
        else if (enemyUnit.currentAp > 1)
        {
            int randNum = Random.Range(0, 10);
            if (randNum <= 3)
            {
                StartCoroutine(MoveToTargetAndBack(enemyUnit, 0));



            }
            else if (randNum > 3 && randNum <= 5)
            {
                combatLog.text = enemyUnit.unitName + " Used Block";
                enemyUnit.isBlocking = true;
                enemyUnit.currentAp--;
                waitSeconds = 2f;
                
            }
            else if (randNum > 5)
            {
                StartCoroutine(MoveToTargetAndBack(enemyUnit, 1));

            }

        }

        


    }



    public IEnumerator EndBattle()
    {
        if (state == battleState.WON)
        {
            combatLog.text = "You Won!";
            winCount++;
            yield return new WaitForSeconds(3f);
            Destroy(enemy);
            Start();
            



        }
        else if (state == battleState.LOST)
        {
            combatLog.text = "You Lost.";
            yield return new WaitForSeconds(3f);
            Destroy(playerGO);
            playerGO = null;
            Destroy(enemy);
            turnCount = 0;
            Start();


        }


    }

    public void isOver(Unit unit)
    {
        
        if (unit._tag == UnitTag.ENEMY)
        {

            if (unit.state == UnitState.DEAD)
            {
                state = battleState.WON;
                StartCoroutine(EndBattle());

            }
            else
            {
                playerUnit.currentAp++;
                StartCoroutine(enemyTurn());
            }
        }
        else
        {
            if (unit.state == UnitState.DEAD)
            {
                state = battleState.LOST;
                StartCoroutine(EndBattle());

            }
            else
            {
                turnCount++;
                state = battleState.PLAYER_TURN;
                enemyUnit.currentAp++;
              
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

    void Update()
    {
        gotHit(playerUnit);
        gotHit(enemyUnit);

        enemyHud.setHp(enemyUnit);
        playerHud.setHp(playerUnit);

        if (playerUnit.currentAp < 2)
        {
            strongAttack.interactable = false;

        }
        else
        {
            strongAttack.interactable = true;
            
        }

        if (playerUnit.currentAp < 1)
        {
            blockButton.interactable = false;
        }
        else
        {
            blockButton.interactable = true;
        }
        
      

    }

    
    

    
}

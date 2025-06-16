using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public enum battleState { START, PLAYER_TURN, ENEMY_TURN, WON, LOST }

public enum dayTime{Day, Night}

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
    public List<GameObject> lights;
    public dayTime dayAndNight;
    public int dayAndNightCycle;
    public List<Sprite> battleBackgroundList;
    public GameObject battleBackground;
    public List<Sprite> currentTimeList;
    public Image displayCurrentTime;
    public int winCount;
    public BattleHud playerHud;
    public BattleHud enemyHud;
    public Text combatLog;
    public AnimationController playerAnim;
    public AnimationController enemyAnim;
    public Button strongAttack;
    public Button blockButton;
    public GameObject gameOverPanel;
    public AttackInfo attackInfo;

    void Awake()
    {
        lights[0].SetActive(true);
        dayAndNight = dayTime.Day;
        int backgroundChoice = 0;
        do
        {
            backgroundChoice = Random.Range(0, 8);
        } while (backgroundChoice % 2 != 0);
        {
            battleBackground.GetComponent<SpriteRenderer>()
            .sprite = battleBackgroundList[backgroundChoice];
        }
        

    }

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

        enemy = Instantiate(enemies[Random.Range(0, 4)],
        battleStation.GetChild(1).transform);
        enemyUnit = enemy.GetComponent<Unit>();
        enemyUnit._tag = UnitTag.ENEMY;

        enemyAnim = enemyUnit.GetComponentInParent<AnimationController>();
        playerAnim = playerUnit.GetComponent<AnimationController>();

        playerHud.setHuD(playerUnit);
        enemyHud.setHuD(enemyUnit);

        combatLog.text = "An enemy " + enemyUnit.unitName + " appears";

        enemyUnit.target = playerUnit;
        playerUnit.target = enemyUnit;

        yield return new WaitForSeconds(2f);

        state = battleState.PLAYER_TURN;
        playerTurn();



    }

    private IEnumerator MoveToTargetAndBack(Unit unit, int attackIndex)
    {
        if (unit._tag == UnitTag.PLAYER)
        {
            if (unit.attacks[attackIndex]._attackRange == AttackRange.Melee)
            {
                originalPosition = battleStation.GetChild(0).position;
                Transform target = battleStation.GetChild(1);

                Vector3 direction = (target.position - playerUnit.transform.position).normalized;

                yield return StartCoroutine(MoveToPosition(target.position - direction * approachDistance, playerUnit));


                combatLog.text = unit.UseAttack(unit.attacks[attackIndex], playerAnim, dayAndNight);

                yield return new WaitForSeconds(0.5f);


                yield return StartCoroutine(MoveToPosition(originalPosition, playerUnit));
            }
            else 
            {
                combatLog.text = unit.UseAttack(unit.attacks[attackIndex], playerAnim, dayAndNight);
            }
        }
        else
        {
            if (unit.attacks[attackIndex]._attackRange == AttackRange.Melee)
            {


                originalPosition = battleStation.GetChild(1).position;
                Transform target = battleStation.GetChild(0);

                Vector3 direction = (target.position - enemyUnit.transform.position).normalized;



                yield return StartCoroutine(MoveToPosition(target.position - direction * approachDistance, enemyUnit));


                combatLog.text = unit.UseAttack(unit.attacks[attackIndex], enemyAnim, dayAndNight);



                yield return StartCoroutine(MoveToPosition(originalPosition, enemyUnit));
            }
            else
            {
                combatLog.text = unit.UseAttack(unit.attacks[attackIndex], enemyAnim, dayAndNight);
            }
        }
    }

    private IEnumerator MoveToPosition(Vector3 destination, Unit unit)
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

    IEnumerator Attack1()
    {
        state = battleState.ENEMY_TURN;
        StartCoroutine(MoveToTargetAndBack(playerUnit, 0));



        yield return new WaitForSeconds(3f);

        isOver(enemyUnit);



    }

    public void firstAttack()
    {
        if (state != battleState.PLAYER_TURN)
        {
            return;
        }

        StartCoroutine(Attack1());
    }

    public void secondAttack()
    {
        if (state != battleState.PLAYER_TURN)
        {
            return;
        }

        StartCoroutine(Attack2());
    }

    public void block()
    {
        if (state != battleState.PLAYER_TURN)
        {
            return;
        }

        StartCoroutine(blockAttacks());
    }

    IEnumerator Attack2()
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
            turnCount++;
            dayAndNightCycle++;
            playerUnit.currentAp++;
            Start();




        }
        else if (state == battleState.LOST)
        {
            combatLog.text = "You Lost.";
            playerAnim.PlayAction("Death");
            yield return new WaitForSeconds(3f);
            Destroy(playerGO);
            playerGO = null;
            gameOverPanel.SetActive(true);


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
                dayAndNightCycle++;
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

    public void UpdateBackground(int n)
    {
        int index = battleBackgroundList.IndexOf(battleBackground.GetComponent<SpriteRenderer>().sprite);
        if (index == 0) {
            n = 0;
        }
        battleBackground.GetComponent<SpriteRenderer>().sprite = battleBackgroundList[index + n];
    }

    void Update()
    {
        if (dayAndNight == dayTime.Day)
        {

            if (dayAndNightCycle == 3)
            {
                lights[0].SetActive(false);
                dayAndNight = dayTime.Night;
                UpdateBackground(1);
                lights[1].SetActive(true);
                dayAndNightCycle = 0;
            }
        }
        else
        {
            if (dayAndNightCycle == 3)
            {
                lights[1].SetActive(false);
                dayAndNight = dayTime.Day;
                UpdateBackground(-1);
                lights[0].SetActive(true);
                dayAndNightCycle = 0;
            }

        }
        attackInfo.setTime(dayAndNight);
        attackInfo.winCount(winCount);


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

        switch (dayAndNightCycle)
        {
            case 0:
                if (dayAndNight == dayTime.Day)
                {
                    displayCurrentTime.sprite = currentTimeList[0];
                }
                else
                {
                    displayCurrentTime.sprite = currentTimeList[3];
                }
                break;
            case 1:
                if (dayAndNight == dayTime.Day)
                {
                    displayCurrentTime.sprite = currentTimeList[1];
                }
                else
                {
                    displayCurrentTime.sprite = currentTimeList[4];
                }
                break;
            case 2:
                if (dayAndNight == dayTime.Day)
                {
                    displayCurrentTime.sprite = currentTimeList[2];
                }
                else
                {
                    displayCurrentTime.sprite = currentTimeList[5];
                }
                break;
        }

    }





}

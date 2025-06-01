using System.Numerics;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public enum battleState { START, PLAYER_TURN, ENEMY_TURN, WON, LOST}

public class battleSystem : MonoBehaviour
{

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    Unit playerUnit0;
    Unit playerUnit1;
    Unit enemyUnit0;
    Unit enemyUnit1;

    public Transform battleStation;


    public battleState state;

    void Start()
    {
        state = battleState.START;
        setUpBattle();

    }

    void setUpBattle()
    {
        GameObject player0 = Instantiate(playerPrefab, battleStation.GetChild(0));
        playerUnit0 = player0.GetComponent<Unit>();

        GameObject player1 = Instantiate(playerPrefab, battleStation.GetChild(1));
        playerUnit0 = player1.GetComponent<Unit>();

        GameObject enemy0 = Instantiate(playerPrefab, battleStation.GetChild(2));
        playerUnit0 = enemy0.GetComponent<Unit>();

        GameObject enemy1 = Instantiate(playerPrefab, battleStation.GetChild(3));
        playerUnit0 = enemy1.GetComponent<Unit>();

    }

    // Update is called once per frame
    void Update()
    {

    }
}

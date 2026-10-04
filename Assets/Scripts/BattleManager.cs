using UnityEngine;

// BattleManager

public class BattleManager : MonoBehaviour
{
    bool playerAlive = true;
    int playerHealth = 200;
    int playerMaxHealth = 200;
    int playerDamage = 50;
    int healAmount = 25;
    int playerSheild = 0;
    int playerMaxSheild = 50;
    int sheildAmount = 20;
    int[] enemy = { 60, 80, 0 };
    int[] enemyArmour = { 25, 40, 70 };
    string[] enemyName = { "Goblin", "Orc", "Knight" };
    int[] enemyDamage = { 10, 20, 30 };
    enum AttackType
    {
        Normal,
        Heavy,
        Poison
        
    }
    AttackType[] enemyAttackType =
    {
        AttackType.Normal,
        AttackType.Heavy,
        AttackType.Heavy
    };

    AttackType playerAttackType = AttackType.Normal;

    int[] poisonTurns = { 0, 0, 0 };
    int poisonTurnsAmount = 3;

    int poisonAmount = 10;
    bool gameWon = false;
    bool defending = false;
    enum PlayerAction
    {
        Attack,
        Heal,
        Defend
    }

    void Start()
    {
        PlayerTurn(PlayerAction.Attack, 2);
        PlayerTurn(PlayerAction.Defend);
        PlayerTurn(PlayerAction.Attack, 1);
        PlayerTurn(PlayerAction.Attack, 1);
        PlayerTurn(PlayerAction.Defend);
        PlayerTurn(PlayerAction.Heal);
        PlayerTurn(PlayerAction.Attack, 2);
        PlayerTurn(PlayerAction.Attack, 2);
    }

    void PlayerTurn(PlayerAction action, int enemyIndex = 0)
    {
        if (playerAlive == false)
        {
            return;
        }

        if (gameWon == true)
        {
            return;
        }

        if (action == PlayerAction.Attack)
        {
            AttackEnemy(enemyIndex);
        }

        if (action == PlayerAction.Heal)
        {
            HealPlayer();
        }

        if (action == PlayerAction.Defend)
        {
            DefendPlayer();
        }
    }

    void ChangePlayerAttackType(AttackType newAttackType)
    {
        playerAttackType = newAttackType;
    }

    void ChangeEnemyAttackType(int enemyIndex, AttackType newAttackType)
    {
        enemyAttackType[enemyIndex - 1] = newAttackType;
    }

    void PoisonEnemy(int enemyIndex)
    {
        if (playerAttackType == AttackType.Poison)
        {
            poisonTurns[enemyIndex - 1] += poisonTurnsAmount;
        }
    }
    void EnemyTakeDamage(int enemyIndex, int damage)
    {
        
    }

    void AttackEnemy(int enemyIndex)
    {
        if (enemyIndex < 1 || enemyIndex > enemy.Length)
        {
            Debug.Log("Invalid enemy selection");
            return;
        }

        if (enemy[enemyIndex - 1] == 0)
        {
            Debug.Log(enemyName[enemyIndex - 1] + " is already dead");
            return;

        }

        //Debug.Log("Player attacks for " + playerDamage + " Damage.");
        
        if (playerAttackType == AttackType.Poison)
        {
            poisonTurns[enemyIndex - 1] += poisonTurnsAmount;
        }
        
        if (poisonTurns[enemyIndex - 1] > 0)
        {
            enemy[enemyIndex - 1] -= poisonAmount;
            poisonTurns[enemyIndex - 1] -= 1;
            Debug.Log(enemyName[enemyIndex - 1] + " took " + poisonAmount + " poison damage. Turns Reamaining: " + poisonTurns[enemyIndex - 1]);
        }

        if (enemyArmour[enemyIndex - 1] > 0)
        {
            enemyArmour[enemyIndex - 1] -= playerDamage;
        }

        else if (enemyArmour[enemyIndex - 1] == 0)
        {
            enemy[enemyIndex - 1] -= playerDamage;
        }

        if (enemyArmour[enemyIndex - 1] < 0)
        {
            enemy[enemyIndex - 1] -= (enemyArmour[enemyIndex - 1] * -1);
            enemyArmour[enemyIndex - 1] = 0;
        }

        if (enemy[enemyIndex - 1] <= 0)
        {
            enemy[enemyIndex - 1] = 0;
        }

        WinCheck();

        if (playerAlive == false)
        {
            return;
        }

        Debug.Log(enemyName[enemyIndex - 1] + " Health: " + enemy[enemyIndex - 1] + " Armour: " + enemyArmour[enemyIndex - 1]);

        if (gameWon)
        {
            return;
        }

        EnemyTurn();
    }
    void HealPlayer()
    {
        if (playerHealth == 0)
        {
            Debug.Log("Player already dead");
            return;
        }

        playerHealth += healAmount;

        if (playerHealth >= playerMaxHealth)
        {
            playerHealth = playerMaxHealth;
            Debug.Log(" Health full. Player Health: " + playerHealth + " Player Sheild: " + playerSheild);
        }

        else
        {
            Debug.Log("Player Health: " + playerHealth + " Player Sheild: " + playerSheild);
        }

        EnemyTurn();

    }

    void DefendPlayer()
    {
        defending = true;

        playerSheild += sheildAmount;

        if (playerSheild >= playerMaxSheild)
        {
            playerSheild = playerMaxSheild;
            Debug.Log("Player Health: " + playerHealth + " Player Sheild Full: " + playerSheild);
        }
        else
        {
            Debug.Log("Player Health: " + playerHealth + " Player Sheild: " + playerSheild);
        }

        EnemyTurn();
    }

    void EnemyTurn()
    {
        int excessDamage = 0;
        int totalDamage = 0;
        int[] enemyBonusDamage = { 0, 0, 0 };

        for (int i = 0; i < enemy.Length; i++)
        {
            if (enemy[i] > 0)
            {
                totalDamage += enemyDamage[i];

                if (enemyAttackType[i] == AttackType.Heavy)
                {
                    enemyBonusDamage[i] = enemyDamage[i];
                }

                totalDamage += enemyBonusDamage[i];
                Debug.Log(enemyName[i] + " attacks for " + (enemyDamage[i] + enemyBonusDamage[i]));
            }
        }

        int damageToTake = totalDamage;

        if (defending)
        {
            damageToTake = totalDamage / 2;
            defending = false;
        }

        playerSheild -= damageToTake;

        if (playerSheild >= 0)
        {
            Debug.Log("Player Health: " + playerHealth + " Player Shield: " + playerSheild);
            return;
        }

        else
        {
            excessDamage = (playerSheild * -1);
            playerSheild = 0;
            playerHealth -= excessDamage;
        }

        WinCheck();
        if (playerAlive == false)
        {
            return;
        }


        Debug.Log(" Player Health: " + playerHealth + " Player Sheild: " + playerSheild);
    }

    
    void WinCheck()
    {
        if (playerHealth <= 0)
        {
            playerHealth = 0;
            playerAlive = false;
            Debug.Log(" GAME OVER ");
            return;
        }

        int enemyCheck = 0;

        for (int i = 0; i < enemy.Length; i++)
        {
            if (enemy[i] == 0)
            {
                enemyCheck += 1;
            }
        }

        if (enemyCheck == enemy.Length)
        {
            gameWon = true;
            Debug.Log(" YOU WIN ");
        }
    }

    void Update()
    {
        
    }
}

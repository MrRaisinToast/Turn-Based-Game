using System.Collections.Generic;
using System.Collections;
using System.Reflection.Metadata.Ecma335;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static System.Net.Mime.MediaTypeNames;


public class Attempt2 : MonoBehaviour
{
    [SerializeField] private GameObject retryButton;

    public Button[] actionButtons;
    public Button[] attackTypeButtons;
    public Button[] enemyButtons;
    public Button backButton;
    public TMP_Text playerHealthText;
    public TMP_Text playerArmourText;
    public TMP_Text PlayerPoisonTurnsText;

    public TMP_Text GoblinHealthText;
    public TMP_Text GoblinArmourText;
    public TMP_Text GoblinPoisonTurnsText;

    public TMP_Text OrcHealthText;
    public TMP_Text OrcArmourText;
    public TMP_Text OrcPoisonTurnsText;

    public TMP_Text HydraHealthText;
    public TMP_Text HydraArmourText;
    public TMP_Text HydraPoisonTurnsText;

    public TMP_Text CombatLogText;
    public TMP_Text WinOrLoseText;

    List<string> combatMessages = new List<string>();

    int maxCombatMessages = 12;
    bool gameWon = false;
    bool playerAlive = true;
    bool[] enemyAlive = { true, true, true };
    bool playerTurnActive = true;

    int playerDamage = 40;
    int playerHealth = 200;
    int playerMaxHealth = 200;
    int playerHealAmount = 50;
    int playerArmour = 20;
    int playerMaxArmour = 50;
    int playerArmourAmount = 20;

    int poisonAmount = 20;
    int poisonTurnsAmount = 3;
    int poisonMaxTurns = 5;
    int playerPoisonTurns = 0;
    int[] enemyPoisonTurns = { 0, 0, 0 };

    string[] enemyName = { "Goblin", "Orc", "Hydra" };
    int[] enemyDamage = { 10, 20, 40 };
    int[] enemyHealth = { 40, 65, 100 };
    int[] enemyMaxHealth = { 40, 65, 150 };
    int[] enemyHealAmount = { 15, 20, 50 };
    int[] enemyArmour = { 10, 0, 50 };
    int[] enemyMaxArmour = { 20, 50, 80 };
    int[] enemyArmourAmount = { 10, 30, 30 };
    
    enum AttackType
    {
        None,
        Normal,
        Heavy,
        Poison
    }
    enum PlayerAction
    {
        None,
        Attack,
        Defend,
        Heal
    }
    enum EnemyAction
    {
        Attack,
        Defend,
        Heal
    }
    PlayerAction selectedAction = PlayerAction.None;
    AttackType selectedAttackType = AttackType.None;

    void Start()
    {
        UpdatePlayerText();
        UpdateEnemyText();
        retryButton.SetActive(true);
    }

    public void RetryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void SetAttackTypeButtonInteractable(int selectedButtonIndex)
    {
        for (int i = 0; i < attackTypeButtons.Length; i++)
        {
            if (i == selectedButtonIndex)
            {
                attackTypeButtons[i].interactable = true;
            }
            else
            {
                attackTypeButtons[i].interactable = false;
            }
        }
    }

    public void SetActionButtonInteractable(int selectedButtonIndex)
    {
        
        for (int i = 0; i < actionButtons.Length; i++)
        {
            
            if (i == selectedButtonIndex)
            {
                actionButtons[i].interactable = true;
            }
            else
            {
                actionButtons[i].interactable = false;
            }
        }
    }

    public void SetEnemyButtonInteractable()
    {
        for (int i = 0; i < enemyButtons.Length; i++)
        {
            if (enemyAlive[i] == false)
            {
                enemyButtons[i].interactable = false;
            }
            else
            {
                enemyButtons[i].interactable = true;
            }
        }
    }

    public void AttackButtonPressed()
    {
        selectedAction = PlayerAction.Attack;

        SetActionButtonInteractable(0);
    }

    public void AttackNormalTypeSelection()
    {
        if (selectedAction == PlayerAction.None)
        {
            return;
        }

        selectedAttackType = AttackType.Normal;

        SetAttackTypeButtonInteractable(0);
    }

    public void AttackHeavyTypeSelection()
    {
        if (selectedAction == PlayerAction.None)
        {
            return;
        }

        selectedAttackType = AttackType.Heavy;

        SetAttackTypeButtonInteractable(1);
    }

    public void AttackPoisonTypeSelection()
    {
        if (selectedAction == PlayerAction.None)
        {
            return;
        }

        selectedAttackType = AttackType.Poison;

        SetAttackTypeButtonInteractable(2);
    }

    public void DefendButtonPressed()
    {
        PlayerTurn(PlayerAction.Defend);

        SetActionButtonInteractable(1);
    }

    public void HealButtonPressed()
    {
        if (playerHealth == playerMaxHealth)
        {
            UpdateCombatLogText("Player is at Max Health");
            actionButtons[0].interactable = true;
            actionButtons[1].interactable = true;
            return;
        }
        PlayerTurn(PlayerAction.Heal);

        SetActionButtonInteractable(2);
    }

    public void BackButtonPressed()
    {
        for (int i = 0; i < actionButtons.Length; i++)
        {
            actionButtons[i].interactable = true;
        }
        for (int i = 0; i < attackTypeButtons.Length; i++)
        {
            attackTypeButtons[i].interactable = true;
        }
        for (int i = 0; i < enemyButtons.Length; i++)
        {
            if (enemyAlive[i] == false)
            {
                enemyButtons[i].interactable = false;
            }
            else
            {
                enemyButtons[i].interactable = true;
            }
        }
    }

    public void EnemySelection(int enemyIndex)
    {
        if (selectedAction == PlayerAction.None || selectedAttackType == AttackType.None)
        {
            return;
        }

        PlayerTurn(selectedAction, enemyIndex, selectedAttackType);

        selectedAttackType = AttackType.None;

        selectedAction = PlayerAction.None;
    }

     public void ButtonReset(bool state)
    {
        for (int i = 0; i < actionButtons.Length; i++)
        {
            actionButtons[i].interactable = state;
        }
        for (int i = 0; i < attackTypeButtons.Length; i++)
        {
            attackTypeButtons[i].interactable = state;
        }
        for (int i = 0; i < enemyButtons.Length; i++)
        {
            enemyButtons[i].interactable = state;
        }
        backButton.interactable = state;
    }

    public void UpdatePlayerText()
    {
        UpdatePlayerHealthText();
        UpdatePlayerArmourText();
        UpdatePlayerPoisonTurnsText();
    }

    public void UpdateEnemyText()
    {
        UpdateGoblinHealthText();
        UpdateGoblinArmourText();
        UpdateGoblinPoisonTurnsText();

        UpdateOrcHealthText();
        UpdateOrcArmourText();
        UpdateOrcPoisonTurnsText();

        UpdateHydraHealthText();
        UpdateHydraArmourText();
        UpdateHydraPoisonTurnsText();
    }

    public void UpdateCombatLogText(string message)
    {
        combatMessages.Add(message);
        {
            if ( combatMessages.Count > maxCombatMessages)
            {
                combatMessages.RemoveAt(0);
            }
        }

        CombatLogText.text = string.Join( "\n", combatMessages);
    }

    public void UpdateGameWinOrLoseText()
    {
        if (gameWon == true)
        {
            WinOrLoseText.text = "YOU WIN";
        }
        else if (playerAlive == false)
        {
            WinOrLoseText.text = "GAME OVER";
        }
    }   
    
    public void UpdatePlayerHealthText()
    {
        playerHealthText.text = "Player Health: " + playerHealth;
    }

    public void UpdatePlayerArmourText()
    {
        playerArmourText.text = "Player Armour: " + playerArmour;
    }

    public void UpdatePlayerPoisonTurnsText()
    {
        PlayerPoisonTurnsText.text = "Poison turns left: " + playerPoisonTurns;
    }

    public void UpdateGoblinHealthText ()
    {
        GoblinHealthText.text = "Health: " + enemyHealth[0];
    }

    public void UpdateOrcHealthText()
    {
        OrcHealthText.text = "Health: " + enemyHealth[1];
    }

    public void UpdateHydraHealthText()
    {
        HydraHealthText.text = "Health: " + enemyHealth[2];
    }

    public void UpdateGoblinArmourText()
    {
        GoblinArmourText.text = "Armour: " + enemyArmour[0];
    }

    public void UpdateOrcArmourText()
    {
        OrcArmourText.text = " Armour: " + enemyArmour[1];
    }

    public void UpdateHydraArmourText()
    {
        HydraArmourText.text = "Armour: " + enemyArmour[2];
    }

    public void UpdateGoblinPoisonTurnsText()
    {
        GoblinPoisonTurnsText.text = "Poison Turns Left: " + enemyPoisonTurns[0];
    }

    public void UpdateOrcPoisonTurnsText()
    {
        OrcPoisonTurnsText.text = "Poison Turns Left: " + enemyPoisonTurns[1];
    }

    public void UpdateHydraPoisonTurnsText()
    {
        HydraPoisonTurnsText.text =  "Poison Turns Left: " + enemyPoisonTurns[2];
    }

    void PlayerTurn(PlayerAction action, int enemyIndex = 0, AttackType type =  AttackType.None)
    {
        if (playerAlive == false || gameWon)
        {
            return;
        }

        if (playerTurnActive == false)
        {
            return;
        }

        if (action == PlayerAction.Attack)
        {
            PlayerAttack(enemyIndex, type);
        }

        if (action == PlayerAction.Defend)
        {
            if (playerArmour == playerMaxArmour)
            {
                UpdateCombatLogText("Player is at MAX ARMOUR");
                actionButtons[0].interactable = true;
                actionButtons[2].interactable = true;
                return;
            }
            PlayerDefend();
        }
        
        if (action == PlayerAction.Heal)
        {
            PlayerHeal();
        }
        
        UpdateEnemyText();

        WinCheck();

        if (gameWon == true)
        {
            ButtonReset(false);
            return;
        }

        PlayerPoisonCheck();

        UpdatePlayerText();

        WinCheck();
        if (playerAlive == false)
        {
            ButtonReset(false);
            return;
        }
      
        playerTurnActive = false;

        ButtonReset(false);
        SetEnemyButtonInteractable();

        StartCoroutine(EnemyTurn());

        WinCheck();

        UpdateEnemyText();
        UpdatePlayerText();
    }

    void PlayerAttack(int enemyIndex, AttackType type)
    {
        if (enemyAlive[enemyIndex] == false)
        {
            Debug.Log(enemyName[enemyIndex] + " is already dead...");
            return;
        }

        int totalPlayerDamage = PlayerAttackTypeCheck(enemyIndex, type, 0);

        UpdateCombatLogText("Player used " + type + " Attack on " + enemyName[enemyIndex]);
        if (type == AttackType.Poison)
        {
            UpdateCombatLogText(enemyName[enemyIndex] + " is now Poisoned for " + enemyPoisonTurns[enemyIndex] + " turns.");
        }

        EnemyDamageTaken(enemyIndex, totalPlayerDamage);
    }

    int PlayerAttackTypeCheck(int enemyIndex, AttackType type, int damage)
    {
        if (type == AttackType.Normal)
        {
            damage += playerDamage;
        }
        
        if (type == AttackType.Poison)
        {
            enemyPoisonTurns[enemyIndex] += poisonTurnsAmount;
            damage += playerDamage;

            if (enemyPoisonTurns[enemyIndex] > poisonMaxTurns)
            {
                enemyPoisonTurns[enemyIndex] = poisonMaxTurns;
            }
        }

        if (type == AttackType.Heavy)
        {
            damage = 2 * playerDamage;
        }

        return damage;
    }

    void PlayerDefend()
    {
        playerArmour += playerArmourAmount;

        if (playerArmour >= playerMaxArmour)
        {
            playerArmour = playerMaxArmour;
            UpdateCombatLogText("MAX ARMOUR Reached.");
            return;
        }

        else
        {
            UpdateCombatLogText("Player gained " + playerArmourAmount + " Armour.");
        }
    }

    void PlayerHeal()
    {
        playerHealth += playerHealAmount;

        if (playerHealth >= playerMaxHealth)
        {
            playerHealth = playerMaxHealth;
            UpdateCombatLogText("MAX Health Reached.");
            return;
        }

        else
        {
            UpdateCombatLogText("Player healed for " + playerHealAmount + " Health.");
        }
        
    }

    void EnemyDamageTaken(int enemyIndex, int damage)
    {
        if (enemyArmour[enemyIndex] > 0)
        {
            damage /= 2;
            enemyArmour[enemyIndex] -= damage;

            UpdateCombatLogText(enemyName[enemyIndex] + " took " + damage + " reduced Damage.");

            if (enemyArmour[enemyIndex] < 0)
            {
                enemyHealth[enemyIndex] -= enemyArmour[enemyIndex] * -1;
                enemyArmour[enemyIndex] = 0;
            }
        }

        else
        {
            enemyHealth[enemyIndex] -= damage;

            UpdateCombatLogText(enemyName[enemyIndex] + " took " + damage + " Damage.");
        }

        if (enemyHealth[enemyIndex] <= 0)
        {
            enemyAlive[enemyIndex] = false;
            enemyHealth[enemyIndex] = 0;
            UpdateCombatLogText(enemyName[enemyIndex] + " is DEAD...");
            return;
        }
    }

    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(1);

        SetActionButtonInteractable(actionButtons.Length);

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < enemyHealth.Length; i++)
        {
           if (enemyAlive[i] == true)
            {
                AttackType type = AttackType.Normal;

                if (i == 0)
                {
                    type = AttackType.Poison;
                    EnemyAttack(i, type);
                    if (type == AttackType.Poison)
                    {
                        UpdateCombatLogText("Player in now poisoned for " + playerPoisonTurns + " turns.");
                    }
                        
                }

                if (i == 1 )
                {
                    if (enemyArmour[i] > 0)
                    {
                        EnemyAttack(i, type);
                    }
                    else
                    {
                        EnemyDefend(i);
                    }
                }

                if (i == 2)
                {
                    
                    if (enemyHealth[i] < 100)
                    {
                        EnemyHeal(i);
                    }
                    else
                    {
                        EnemyAttack(i, type);
                    }
                }
                UpdatePlayerText();
                UpdateEnemyText();

                WinCheck();

                if (playerAlive == false)
                {
                    ButtonReset(false);
                    yield break;
                }

                EnemyPoisonCheck(i);

                UpdateEnemyText();

                WinCheck();

                if (gameWon == true)
                {
                    ButtonReset(false);
                    yield break;
                }

                SetEnemyButtonInteractable();

                yield return new WaitForSeconds(1);
            }
        }

        playerTurnActive = true;
        BackButtonPressed();
        backButton.interactable = true;
    }

    void EnemyAttack(int enemyIndex, AttackType type)
    {
        int totalEnemyDamage = EnemyAttackTypeCheck(enemyIndex, type, 0);

        UpdateCombatLogText(enemyName[enemyIndex] + " used " + type + " Attack.");

        PlayerDamageTaken(totalEnemyDamage);
    }

    void EnemyDefend(int enemyIndex)
    {
        enemyArmour[enemyIndex] += enemyArmourAmount[enemyIndex];

        if (enemyArmour[enemyIndex] > enemyMaxArmour[enemyIndex])
        {
            enemyArmour[enemyIndex] = enemyMaxArmour[enemyIndex];
        }


        UpdateCombatLogText(enemyName[enemyIndex] + " used Defend. Armour + " + enemyArmourAmount[enemyIndex] + ".");
    }

    void EnemyHeal(int enemyIndex)
    {
        enemyHealth[enemyIndex] += enemyHealAmount[enemyIndex];

        if (enemyHealth[enemyIndex] > enemyMaxHealth[enemyIndex])
        {
            enemyHealth[enemyIndex] = enemyMaxHealth[enemyIndex];
        }

        UpdateCombatLogText(enemyName[enemyIndex] + " healed for " + enemyHealAmount[enemyIndex] + " Health.");
    }

    int EnemyAttackTypeCheck(int enemyIndex, AttackType type, int damage)
    {
        if (type == AttackType.Normal)
        {
            damage += enemyDamage[enemyIndex];
        }

        if (type == AttackType.Poison)
        {
            playerPoisonTurns += poisonTurnsAmount;
            damage += enemyDamage[enemyIndex];

            if (playerPoisonTurns > poisonMaxTurns)
            {
                playerPoisonTurns = poisonMaxTurns;
            }
        }

        if (type == AttackType.Heavy)
        {
            damage = enemyDamage[enemyIndex] * 2;
        }
        return damage;
    }

    void PlayerDamageTaken(int damage)
    {
     
        if (playerArmour > 0)
        {
            damage /= 2;
            playerArmour -= damage;
            UpdateCombatLogText("Player took " + damage + " reduced damage.");

            if (playerArmour < 0)
            {
                playerHealth -= playerArmour * -1;
                playerArmour = 0;
            }
        }

        else
        {
            playerHealth -= damage;
            UpdateCombatLogText("Player took " + damage + " damage.");
        }

        if (playerHealth <= 0)
        {
            playerHealth = 0;
            UpdateCombatLogText("player is dead...");
            return;
        }
    }

    void PlayerPoisonCheck()
    {
        if (playerPoisonTurns > 0 && playerArmour > 0)
        {
            playerArmour -= poisonAmount / 2;
            playerPoisonTurns -= 1;
            UpdateCombatLogText("Player took " + poisonAmount / 2 + " reduced Poison Damage.");
            if (playerArmour < 0)
            {
                playerHealth -= playerArmour * -1;
                playerArmour = 0;
            }
        }

        else if (playerPoisonTurns > 0)
        {
            playerHealth -= poisonAmount;
            playerPoisonTurns -= 1;
            UpdateCombatLogText("Player took " + poisonAmount + " Poison Damage.");
        }
    }

    void EnemyPoisonCheck(int enemyIndex)
    {
        if (enemyArmour[enemyIndex] > 0 && enemyPoisonTurns[enemyIndex] > 0)
        {
            enemyArmour[enemyIndex] -= poisonAmount / 2;
            enemyPoisonTurns[enemyIndex] -= 1;
            UpdateCombatLogText(enemyName[enemyIndex] + " took " + poisonAmount / 2 + " reduced Poison Damage.");

            if (enemyArmour[enemyIndex] < 0)
            {
                enemyHealth[enemyIndex] -= enemyArmour[enemyIndex] * -1;
                enemyArmour[enemyIndex] = 0;
            }
        }

        else if (enemyPoisonTurns[enemyIndex] > 0)
        {
            enemyHealth[enemyIndex] -= poisonAmount;
            enemyPoisonTurns[enemyIndex] -= 1;
            UpdateCombatLogText(enemyName[enemyIndex] + " took " + poisonAmount + " Poison Damage.");
        }
    }
    void WinCheck()
    {
        if (playerHealth <= 0)
        {
            playerHealth = 0;
            playerAlive = false;
            UpdatePlayerText();
            UpdateGameWinOrLoseText();
            UpdateCombatLogText("Player was defeated... Better luck next time.");
            Debug.Log("GAME OVER");
            return;
        }

        int enemyDead = 0;

        for (int i = 0; i < enemyHealth.Length; i++)
        {

            if (enemyHealth[i] <= 0)
            {
                enemyAlive[i] = false;
                enemyHealth[i] = 0;
            }
            if (enemyAlive[i] == false)
            {
                enemyDead += 1; 
            }
        }

        if (enemyDead == enemyHealth.Length)
        {
            gameWon = true;
            UpdateGameWinOrLoseText();
            UpdateCombatLogText("Congratulations you've defeated all enemies!");
            Debug.Log("YOU WIN");
        }
    }

    void Update()
    {
        
    }
}

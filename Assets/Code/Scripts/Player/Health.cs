using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayerRoot
{
    public class Health
    {
        public int MaxHealth;
        public int CurrentHealth;
        private Player player;
        public void Initialize(Player player)
        {
            this.player = player;
            MaxHealth = player.health;
            CurrentHealth = MaxHealth;

            
            player.healthBar.Initialize(player.rowByHealth, MaxHealth, CurrentHealth);

        }
        public void TakeDamage(int damage)
        {
            CurrentHealth -= damage;
            player.healthBar.ChangeValue(CurrentHealth);
            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

        public void Die()
        { 
            player.Died = true;
            player.lostManager.gameObject.SetActive(true);
            player.lostManager.DiedPlayer(player);
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

}

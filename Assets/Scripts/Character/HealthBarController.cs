namespace Character
{
    using UnityEngine;
    using UI;

    public class HealthBarController : MonoBehaviour
    {
        [SerializeField] private HealthBar healthBar;
        private PlayerHealth playerHealth;

        void Start()
        {
            playerHealth = GetComponent<PlayerHealth>();
            
            if (playerHealth == null)
            {
                Debug.LogError("PlayerHealth component not found!");
                return;
            }

            
        }

        private void UpdateMaxHealth(int maxHealth)
        {
            healthBar.SetMaxHealth(maxHealth);
        }

        private void UpdateHealth(int currentHealth)
        {
            healthBar.SetHealth(currentHealth);
        }

        
    }
}
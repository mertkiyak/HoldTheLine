namespace Character
{
    using UnityEngine;

    public class PlayerHealth : MonoBehaviour
    {
        public int maxHealth;
        public int currentHealth;
        
        public UI.HealthBar healthBar;
        void Start()
        {
            currentHealth = maxHealth; 
            healthBar.SetMaxHealth(maxHealth);
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                TakeDamage(20);
            }
                
            
        }
        void OnCollisionEnter(Collision collision)
        {
            

            if (collision.gameObject.GetComponent<Heal>())
            {
                HealDamage(20);
                Destroy(collision.gameObject);
            }
        }

        public  void TakeDamage(int damage)
        {
                
            currentHealth -= damage;
            currentHealth = Mathf.Max(currentHealth, 0);
            healthBar.SetHealth(currentHealth);
        }
        void HealDamage(int damage)
        {
            currentHealth += damage;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            healthBar.SetHealth(currentHealth);
        }
    }
}

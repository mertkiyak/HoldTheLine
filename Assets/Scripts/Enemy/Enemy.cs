    namespace Enemy
    {
        using UnityEngine;

        public class Enemy : MonoBehaviour
        {
            [Header("Sağlık Ayarları")]
            [SerializeField] private float enemyHealth = 60f;

            [Header("Hasar Ayarları")]
            [SerializeField] private int contactDamage = 20; 
            [SerializeField] private float damageCooldown = 1f; 
            private float lastDamageTime = -999f; 

            [Header("Ölüm Efekti (Opsiyonel)")]
            [SerializeField] private GameObject deathEffectPrefab;
            [SerializeField] private float deathEffectDuration = 1f;

            void Start()
            {
                
                if (enemyHealth <= 0)
                {
                    enemyHealth = 60f;
                    Debug.LogWarning(gameObject.name + " canı 0'dı, 60'a ayarlandı!");
                }
            }

           
            
            public void TakeDamage(int damage)
            {
                enemyHealth -= damage;
                Debug.Log(gameObject.name + " hasar aldı! Kalan can: " + enemyHealth);

                
                if (enemyHealth <= 0)
                {
                    Die();
                }
            }

           
            private void Die()
            {
                Debug.Log(gameObject.name + " öldü!");
                Destroy(gameObject);
            }

            
            private void OnCollisionEnter(Collision collision)
            {
                
                Character.PlayerHealth player = collision.gameObject.GetComponent<Character.PlayerHealth>();

                if (player != null)
                {
                    
                    if (Time.time >= lastDamageTime + damageCooldown)
                    {
                        player.TakeDamage(contactDamage);
                        lastDamageTime = Time.time;
                        Debug.Log(gameObject.name + " karaktere hasar verdi!");
                    }
                }
            }

        
            private void OnDrawGizmos()
            {
                
                if (Application.isPlaying)
                {
                    Gizmos.color = enemyHealth > 30 ? Color.green : Color.red;
                    Gizmos.DrawWireSphere(transform.position + Vector3.up * 2f, 0.3f);
                }
            }
        }
    }
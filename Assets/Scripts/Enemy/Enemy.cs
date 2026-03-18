namespace Enemy
{
    using UnityEngine;
    using System.Collections;

    public class Enemy : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private float enemyHealth = 60f;
        
        [Header("Damage Settings")]
        [SerializeField] private int contactDamage = 20; 
        [SerializeField] private float damageCooldown = 1f; 
        private float lastDamageTime = -999f;
        
        [Header("Visual Feedback")]
        [SerializeField] private float damageFlashDuration = 0.2f;
        [SerializeField] private Color damageColor = Color.red;
        
        
        private Renderer enemyRenderer;
        private Color originalColor;
        private MaterialPropertyBlock propertyBlock;
        private bool isFlashing = false;
        
        void Start()
        {
            if (enemyHealth <= 0)
            {
                enemyHealth = 60f;
               
            }
            
            
            enemyRenderer = GetComponent<Renderer>();
            
            if (enemyRenderer != null)
            {
                propertyBlock = new MaterialPropertyBlock();
                
                enemyRenderer.GetPropertyBlock(propertyBlock);
                originalColor = enemyRenderer.material.color;
            }
            
        }

        public void TakeDamage(int damage)
        {
            enemyHealth -= damage;
            

          
            if (!isFlashing)
            {
                StartCoroutine(DamageFlashEffect());
            }
            
            if (enemyHealth <= 0)
            {
                Die();
            }
        }

        
        private IEnumerator DamageFlashEffect()
        {
            if (enemyRenderer == null) yield break;
            
            isFlashing = true;
            
            
            enemyRenderer.material.color = damageColor;
            
           
            yield return new WaitForSeconds(damageFlashDuration);
            
            
            enemyRenderer.material.color = originalColor;
            
            isFlashing = false;
        }

        private void Die()
        {
            Debug.Log($"{gameObject.name} died!");
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
                    Debug.Log($"{gameObject.name} dealt {contactDamage} damage to player");
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
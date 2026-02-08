namespace Character
{
    using UnityEngine;

    public class PlayerAttack : MonoBehaviour
    {
        [Header("Attack Settings")]
        [SerializeField] private float attackRadius = 5f; 
        [SerializeField] private int attackDamage = 20;
        [SerializeField] private int maxEnemiesInRange = 10;

        private Collider[] hitColliders;
        
        private void Start()
        {
            hitColliders = new Collider[maxEnemiesInRange];
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PerformAreaAttack();
            }
        }

        private void PerformAreaAttack()
        {
            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, attackRadius, hitColliders);

            for (int i = 0; i < hitCount; i++)
            {
                if (hitColliders[i].TryGetComponent(out Enemy.Enemy enemyScript))
                {
                    enemyScript.TakeDamage(attackDamage);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRadius);
        }
    }
}
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    private Animator animator;
    private string currentState;

    private void Start()
    {
        animator = GetComponent<Animator>();
        currentState = "Idle";
        animator.CrossFade("Idle", 0f);
    }

    void Update()
    {
        if (animator == null) return;

        Vector3 direction = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) direction += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) direction += Vector3.back;
        if (Input.GetKey(KeyCode.A)) direction += Vector3.left;
        if (Input.GetKey(KeyCode.D)) direction += Vector3.right;

        direction.Normalize();
        transform.position += direction * speed * Time.deltaTime;

        bool isMoving = direction.magnitude > 0.1f;

        // Karakteri hareket yönüne çevir
        if (isMoving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            ChangeAnimationState("Walk");
        }
        else
        {
            ChangeAnimationState("Idle");
        }
    }

    private void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;
        animator.CrossFade(newState, 0f);
        currentState = newState;
    }
}
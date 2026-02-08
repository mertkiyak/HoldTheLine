namespace Character
{
    using UnityEngine;

    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float speed = 5f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float directionSmoothTime = 0.1f;

        private Animator animator;
        private string currentState;
        private Rigidbody rb;
        private Vector3 direction;
        private Vector3 currentDirection;
        private Vector3 directionVelocity;
        
        private void Start()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;

            currentState = "Idle";
            animator.CrossFade("Idle", 0f);
        }

        void Update()
        {
            HandleMovementInput();
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }

        private void HandleMovementInput()
        {
            direction = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) direction += Vector3.forward;
            if (Input.GetKey(KeyCode.S)) direction += Vector3.back;
            if (Input.GetKey(KeyCode.A)) direction += Vector3.left;
            if (Input.GetKey(KeyCode.D)) direction += Vector3.right;

            direction.Normalize();

            currentDirection = Vector3.SmoothDamp(
                currentDirection,
                direction,
                ref directionVelocity,
                directionSmoothTime
            );

            bool isMoving = currentDirection.magnitude > 0.1f;

            if (isMoving)
            {
                RotatePlayer();
                ChangeAnimationState("Walk");
            }
            else
            {
                ChangeAnimationState("Idle");
            }
        }

        private void MovePlayer()
        {
            Vector3 newPosition = rb.position + currentDirection * (speed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);
        }

        private void RotatePlayer()
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        private void ChangeAnimationState(string newState)
        {
            if (currentState == newState) return;
            animator.CrossFade(newState, 0f);
            currentState = newState;
        }
    }
}
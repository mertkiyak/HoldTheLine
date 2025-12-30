using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float cameraDistance = 5f;
    [SerializeField] private float cameraHeight = 2f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private float groundCheckDistance = 0.1f;

    private Vector3 direction;
    private Animator animator;
    private string currentAnimation = "Idle";
    private float cameraRotationX = 20f;
    private float cameraRotationY = 0f;
    private Transform cameraTransform;
    private Rigidbody rb;
    private bool isGrounded;
    private Vector3 groundCheckOffset;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // Rigidbody yoksa ekle
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Rigidbody ayarları
        rb.mass = 1f;
        rb.linearDamping = 0f;
        rb.angularDamping = 0.05f;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Collider'ın alt merkezini bul
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            groundCheckOffset = new Vector3(0, col.bounds.extents.y, 0);
        }
        else
        {
            groundCheckOffset = new Vector3(0, 0.5f, 0);
        }

        // Kamera oluştur veya bul
        Camera cam = GetComponentInChildren<Camera>();
        if (cam == null)
        {
            GameObject cameraObj = new GameObject("PlayerCamera");
            cam = cameraObj.AddComponent<Camera>();
            cameraObj.tag = "MainCamera";
        }
        cameraTransform = cam.transform;
        cameraTransform.SetParent(null);

        // Mouse imlecini kilitle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        // ESC ile mouse kilidini aç/kapa
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        // Mouse ile kamera rotasyonu
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        cameraRotationY += mouseX;
        cameraRotationX -= mouseY;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -20f, 80f);

        // Kamera pozisyonunu karaktere göre ayarla
        Quaternion rotation = Quaternion.Euler(cameraRotationX, cameraRotationY, 0f);
        Vector3 offset = rotation * new Vector3(0f, cameraHeight, -cameraDistance);

        cameraTransform.position = transform.position + offset;
        cameraTransform.LookAt(transform.position + Vector3.up * cameraHeight);
    }

    void Update()
    {
        // Gelişmiş yere değme kontrolü - SphereCast kullanarak
        Vector3 checkPosition = transform.position - groundCheckOffset + Vector3.up * groundCheckRadius;
        isGrounded = Physics.SphereCast(checkPosition, groundCheckRadius, Vector3.down, out RaycastHit hit, groundCheckDistance + groundCheckRadius);

        // Debug görselleştirmesi
        Debug.DrawRay(checkPosition, Vector3.down * (groundCheckDistance + groundCheckRadius), isGrounded ? Color.green : Color.red);

        // Console'a yazdır (test için)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space basıldı! isGrounded: " + isGrounded + " | Y Velocity: " + rb.linearVelocity.y);
        }

        // Zıplama kontrolü
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Debug.Log("Zıplıyor!");
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // Y hızını sıfırla
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Önce E ve F tuşlarını kontrol et
        if (Input.GetKey(KeyCode.E))
        {
            PlayAnimation("Wave", true);
            direction = Vector3.zero;
            return;
        }
        if (Input.GetKey(KeyCode.F))
        {
            PlayAnimation("Attack", true);
            direction = Vector3.zero;
            return;
        }

        // Hareket kontrolü - Kameranın baktığı yöne göre
        direction = Vector3.zero;

        if (cameraTransform != null)
        {
            Vector3 forward = cameraTransform.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 right = cameraTransform.right;
            right.y = 0f;
            right.Normalize();

            if (Input.GetKey(KeyCode.W)) direction += forward;
            if (Input.GetKey(KeyCode.S)) direction -= forward;
            if (Input.GetKey(KeyCode.A)) direction -= right;
            if (Input.GetKey(KeyCode.D)) direction += right;
        }

        bool isMoving = direction.magnitude > 0.1f;

        if (isMoving)
        {
            // Karakteri hareket yönüne döndür
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            PlayAnimation("Walk", true);
        }
        else
        {
            PlayAnimation("Idle", true);
        }
    }

    void FixedUpdate()
    {
        // Hareket - sadece yatay eksende
        Vector3 moveVelocity = direction.normalized * speed;

        // Mevcut Y hızını koru (yerçekimi ve zıplama için)
        rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);
    }

    private void PlayAnimation(string animationName, bool forceRestart = false)
    {
        if (currentAnimation != animationName)
        {
            animator.Play(animationName, 0, 0f);
            currentAnimation = animationName;
        }
        else if (forceRestart)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.normalizedTime >= 1.0f && !animator.IsInTransition(0))
            {
                animator.Play(animationName, 0, 0f);
            }
        }
    }

    // Görsel debug için (Scene view'da görünür)
    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying && rb != null)
        {
            Vector3 checkPosition = transform.position - groundCheckOffset + Vector3.up * groundCheckRadius;
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(checkPosition, groundCheckRadius);
            Gizmos.DrawLine(checkPosition, checkPosition + Vector3.down * (groundCheckDistance + groundCheckRadius));
        }
    }
}
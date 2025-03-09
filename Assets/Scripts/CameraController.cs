using System;
using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] [Range(0.1f, 10)] private float movementSpeed = 0.5f;
    [SerializeField] [Range(1, 5)] private float mouseSpeed = 2f;
    [SerializeField] [Range(1, 10)] private float turboSpeed = 2f;

    [SerializeField] bool useMouseLook = true;
    [SerializeField] int health = 3;
    [SerializeField] float secondsOfImmortality = 1.5f;
    [SerializeField] CursorLockMode useLockState = CursorLockMode.Locked;
    [SerializeField] private TextMeshProUGUI healthText;
    private AudioSource _audioSource;
    
    private GameManager _gameManager;

    private float _turbo;
    private float _h;
    private float _v;
    private float _mouseX;
    private Rigidbody _rb;
    private Vector3 _moveDirection;
    private float _lastHitTime;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _gameManager = GetComponent<GameManager>();
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        _rb.linearDamping = 5;
        _rb.angularDamping = 1;
    }

    void Start()
    {
        healthText.text = "Health: " + health;
        if (useMouseLook)
        {
            Cursor.lockState = useLockState;
        }
    }

    void Update()
    {
        _turbo = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            ? turboSpeed
            : 1;

        _mouseX = Input.GetAxis("Mouse X");
        _h = Input.GetAxis("Horizontal");
        _v = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        _moveDirection = (transform.forward * _v + transform.right * _h).normalized;
        _rb.linearVelocity = _moveDirection * (movementSpeed * _turbo);

        Quaternion deltaRotation = Quaternion.Euler(0, mouseSpeed * 500 * Time.fixedDeltaTime * _mouseX, 0);
        _rb.MoveRotation(_rb.rotation * deltaRotation);
    }

    public void TakeDamage()
    {
        if (health > 0 && Time.time - _lastHitTime >= secondsOfImmortality )
        {
            _lastHitTime = Time.time;
            health--;
            _audioSource.Play();
            healthText.text = "Health: " + health;
            if (health <= 0)
            {
                enabled = false;
                _gameManager.GameOver();
            }
        }
    }
}

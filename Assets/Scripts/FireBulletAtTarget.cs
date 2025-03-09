using UnityEngine;

public class FireBulletsAtTarget : MonoBehaviour
{
    [SerializeField] GameObject activationMark;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject head;
    [SerializeField] Transform[] firePositions;
    [SerializeField][Range(1f, 100f)] private float minBulletSpeed;
    [SerializeField][Range(1f, 100f)] private float maxBulletSpeed;
    [SerializeField][Range(1f, 180f)] private float rotateSpeed = 45;
    [SerializeField][Range(0, 1)] private float fireAngle = 0.9f;

    private Transform _target;

    private float _fireRate = 1;
    private float _fireDistance = 100;
    private float _nextFire;

    [SerializeField] private bool debugDetectoinAngle = true;
    [SerializeField] private bool debugArea = true;

    internal void Configure(float fireRate, float fireDistance, Transform trackTarget)
    {
        _target = trackTarget;
        _fireRate = fireRate;
        _fireDistance = fireDistance;
    }

    private void Start()
    {
        activationMark.SetActive(false);
    }

    void Update()
    {
        if (!_target) return;

        if (Vector3.Distance(transform.position, _target.position) > _fireDistance)
        {
            head.transform.rotation = Quaternion.RotateTowards(head.transform.rotation, Quaternion.identity, rotateSpeed * Time.deltaTime);
            activationMark.SetActive(false);
            return;
        }

        activationMark.SetActive(true);

        Vector3 directionStart = firePositions[0].forward;
        Vector3 directionEnd = (_target.position - firePositions[0].position).normalized;

        float angle = Vector3.Angle(directionStart, directionEnd);

        float dot = Vector3.Dot(directionStart, directionEnd);

        if (debugDetectoinAngle) Debug.LogWarning($"{gameObject.name}:{angle} dot:{dot}", gameObject);

        var q = Quaternion.LookRotation(_target.position - head.transform.position);
        head.transform.rotation = Quaternion.RotateTowards(head.transform.rotation, q, rotateSpeed * Time.deltaTime);

        if (dot < 0 || fireAngle > dot) return;

        _nextFire += Time.deltaTime;

        if (_nextFire >= _fireRate)
        {
            foreach (Transform firePosition in firePositions)
            {
                GameObject bullet = Instantiate(bulletPrefab, firePosition.position, Quaternion.Euler(90, 0, 0));
                bullet.GetComponent<MoveBullets>().Configure(Random.Range(minBulletSpeed, maxBulletSpeed));
                bullet.GetComponent<DestroyBulletOnCollide>().Owner = gameObject;

                bullet.transform.up = _target.position - firePosition.position;

            }
            
            _nextFire = 0f;
        }

    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || !debugArea) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _fireDistance);
    }
}
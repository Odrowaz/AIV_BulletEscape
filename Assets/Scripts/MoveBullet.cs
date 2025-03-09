using UnityEngine;

public class MoveBullets : MonoBehaviour
{
    private float _speed = 100;
    private bool _inverted = false;
    public void Configure(float speed)
    {
        GetComponent<AudioSource>().Play();
        _speed = speed;
        Destroy(gameObject, 3);
    }

    public void invertSpeed()
    {
        if (!_inverted)
        {
            _speed = -_speed;
            _inverted = true;
        }
    }

    void Update()
    {
        transform.Translate(Vector3.up * (Time.deltaTime * _speed), Space.Self);
    }
}

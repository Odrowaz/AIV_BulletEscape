using System;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class DestroyOnMultipleHit : MonoBehaviour
{
    [SerializeField] int maxHitCount = 10;
    public int MaxHitCount => maxHitCount;
    [SerializeField] private bool randomHitCount = true;

    Material _material;
    private float _destroyStepsPercent = 1;

    GameManager _gameManager;

    public GameManager GameManager
    {
        set => _gameManager = value;
    }

    private AudioSource _audioSource;

    private void Awake()
    {
        if (randomHitCount)
        {
            maxHitCount = Random.Range(2, maxHitCount);
        }

        Configure(maxHitCount);
    }

    void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer)
        {
            _material = meshRenderer.material;
        }

        _audioSource = GetComponent<AudioSource>();
        _audioSource.volume = 0.1f;
    }

    public void Configure(int maxHitCountValue)
    {
        maxHitCount = maxHitCountValue;
        _destroyStepsPercent = 1f / maxHitCount;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (maxHitCount > 0)
        {
            if (!other.gameObject.GetComponent<MoveBullets>()) return;

            maxHitCount -= 1;
            if (_material)
            {
                _material.color -= new Color(0, 0, 0, _destroyStepsPercent);
            }

            //Debug.Log($"{_destroyStepsPercent} -> alpha = {_material.color.a}");

            if (maxHitCount <= 0)
            {
                _gameManager.CheckDestroyedElements(gameObject);

                FireBulletsAtTarget fireBulletsAtTarget = gameObject.GetComponent<FireBulletsAtTarget>();
                if (fireBulletsAtTarget)
                {
                    fireBulletsAtTarget.enabled = false;
                    foreach (Transform child in transform)
                    {
                        child.gameObject.SetActive(false);
                    }
                }
                else // a wall
                {
                    GetComponent<MeshRenderer>().enabled = false;
                    GetComponent<Collider>().enabled = false;
                }


                if (_audioSource.clip)
                {
                    _audioSource.Play();
                    Invoke(nameof(DestroyMe), _audioSource.clip.length);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private void DestroyMe()
    {
        _gameManager = null;
        Destroy(gameObject);
    }
}

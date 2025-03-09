using UnityEngine;

public class RotateAnimation : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 50;
    void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }
}
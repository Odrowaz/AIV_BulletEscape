using UnityEngine;

public class DestroyBulletOnCollide : MonoBehaviour
{
    private GameObject _owner;

    public GameObject Owner
    {
        set { _owner = value; }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject == _owner)
        {
            Destroy(gameObject);
            return;
        }
        
        if (other.gameObject.GetComponent<FireBulletsAtTarget>()) return;
        var cameraController = other.gameObject.GetComponent<CameraController>();
        
        if (cameraController)
        {
            cameraController.TakeDamage();
            Destroy(gameObject);
            return;
        }
        
        gameObject.GetComponent<MoveBullets>().invertSpeed();
        

    }
}

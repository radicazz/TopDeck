using UnityEngine;

public class ProjectileCleanup : MonoBehaviour
{
    [SerializeField] float _life = 3f;
    float _t;
    void Update()
    {
        _t += Time.deltaTime;
        if (_t >= _life) Destroy(gameObject);
    }
}

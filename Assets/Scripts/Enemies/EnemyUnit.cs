using UnityEngine;

public class EnemyUnit : MonoBehaviour
{
    [SerializeField] float _speed = 2f;
    [SerializeField] int _health = 10;
    [SerializeField] int _damage = 1;
    [SerializeField] Color _tint = Color.red;

    Transform _target;

void Start(){ _target = null; var sr = GetComponentInChildren<SpriteRenderer>(); if(sr==null) sr = gameObject.AddComponent<SpriteRenderer>(); if(sr.sprite==null){ var builtin = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd"); if(builtin!=null) sr.sprite = builtin; } sr.color = _tint; }

    void Update()
    {
        Vector3 dest = _target != null ? _target.position : Vector3.zero;
        Vector3 dir = (dest - transform.position).normalized;
        transform.position += dir * _speed * Time.deltaTime;
    }

    public void TakeDamage(int amount)
    {
        _health -= amount;
        if (_health <= 0) Destroy(gameObject);
    }


public void ApplyVariant(EnemyVariantGenerator.Variant v){ _health = Mathf.Max(1, Mathf.RoundToInt(_health * v.healthMultiplier)); _speed *= v.speedMultiplier; var sr = GetComponentInChildren<SpriteRenderer>(); if(sr!=null) sr.color = v.tint; if(v.sizeMultiplier > 0.01f) transform.localScale *= v.sizeMultiplier; }
}

using Player.Combat;
using UnityEngine;

public class SimpleProjectile : MonoBehaviour
{
    [SerializeField] private int damage = 5;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private LayerMask hitMask;
    private GameObject _owner;
    private Rigidbody2D _rb;

    private void Awake() => _rb = GetComponent<Rigidbody2D>();

    public void Launch(GameObject owner, Vector2 dir, float speed, int dmg, float life)
    {
        _owner = owner;
        damage = dmg;
        lifeTime = life;
        if (_rb) _rb.linearVelocity = dir.normalized * speed;
        Destroy(gameObject, lifeTime);
    }

    // kompatibilnost sa SendMessage iz ability-ja
    public void SetOwner(GameObject o) => _owner = o;
    public void SetDamage(int d) => damage = d;

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Ignoriši vlasnika
        if (_owner && col.attachedRigidbody && col.attachedRigidbody.gameObject == _owner)
            return;

        // Pokušaj da naneseš štetu
        var dmgTarget = col.GetComponent<IDamageable>();
        if (dmgTarget != null)
        {
            Vector2 hitPoint = transform.position;
            Vector2 normal = -_rb.linearVelocity.normalized;
            dmgTarget.TakeDamage(damage, hitPoint, normal);
            Destroy(gameObject);
            return;
        }

        // Ako je kolizija sa “čvrstim” slojem / groundom – samo nestani
        if (((1 << col.gameObject.layer) & hitMask.value) != 0)
        {
            Destroy(gameObject);
        }
    }
}

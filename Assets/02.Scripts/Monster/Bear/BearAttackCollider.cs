using UnityEngine;

public class BearAttackCollider : MonoBehaviour
{
    private Bear _owner;

    private void Awake()
    {
        _owner = GetComponentInParent<Bear>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        IDamageable damageableObject = other.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            Vector3 dir = (_owner.transform.position - other.transform.position).normalized;
            Vector3 vfxPosition = other.transform.position + dir * _owner.VFXOffset + new Vector3(0, 1f, 0);
            Instantiate(_owner.HitVFX, vfxPosition, Quaternion.identity);

            _owner.Hit(other);
        }
    }
}

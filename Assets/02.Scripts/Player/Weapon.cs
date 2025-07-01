using UnityEngine;

public class Weapon : MonoBehaviour
{
    public ParticleSystem HitVFX;
    public float VFXOffset = 0.5f;

    private PlayerAttackAbility _attackAbility;


    private void Start()
    {
        _attackAbility = GetComponentInParent<PlayerAttackAbility>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == _attackAbility.transform)
        {
            return;
        }
        
        IDamageable damageableObject = other.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            Vector3 dir = (_attackAbility.transform.position - other.transform.position).normalized;
            Vector3 vfxPosition = other.transform.position + dir * VFXOffset + new Vector3(0, 1f, 0);
            Instantiate(HitVFX, vfxPosition, Quaternion.identity);

            _attackAbility.Hit(other);
        }
    }
}

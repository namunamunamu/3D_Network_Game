using System;
using Photon.Pun;
using UnityEngine;


[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public abstract class ItemObject : MonoBehaviourPun
{
    protected Player _player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _player = other.GetComponent<Player>();
            ItemEffect();
            ItemObjectFactory.Instnace.RequestDelete(photonView.ViewID);
        }
    }

    protected virtual void ItemEffect()
    {
        if (_player == null)
        {
            throw new Exception($"{gameObject.name} :: 플레이어를 찾을 수 없습니다.");
        }
    }
}

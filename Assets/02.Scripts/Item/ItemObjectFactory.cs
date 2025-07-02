using Photon.Pun;
using UnityEngine;


public enum EItemType
{
    Score,
    Health,
    Stamina
}

[RequireComponent(typeof(PhotonView))]
public class ItemObjectFactory : MonoBehaviourPun
{
    private static ItemObjectFactory _intance;
    public static ItemObjectFactory Instnace => _intance;

    private void Awake()
    {
        _intance = this;
    }

    public void RequestCreate(EItemType itemType, Vector3 dropPosition)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Create(itemType, dropPosition);
        }
        else
        {
            photonView.RPC(nameof(Create), RpcTarget.MasterClient, itemType, dropPosition);
        }
    }

    public void RequestDelete(int viewID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Delete(viewID);
        }
        else
        {
            photonView.RPC(nameof(Delete), RpcTarget.MasterClient, viewID);
        }
    }

    [PunRPC]
    private void Create(EItemType itemType, Vector3 dropPosition)
    {
        PhotonNetwork.InstantiateRoomObject($"{itemType}Item", dropPosition, Quaternion.identity);
    }

    [PunRPC]
    private void Delete(int viewID)
    {
        GameObject objectToDelete = PhotonView.Find(viewID).gameObject;
        if (objectToDelete == null) return;

        PhotonNetwork.Destroy(objectToDelete);
    }
}

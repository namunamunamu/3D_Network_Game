using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public float CameraHeight = 12f;
    private Player _player;
    private Vector3 _cameraPosition;

    private void Start()
    {
        EventManger.AddListener<PlayerSpawnEvent>(OnFindPlayer);
    }

    private void OnFindPlayer(PlayerSpawnEvent spawnEvent)
    {
        _player = spawnEvent.Player;
    }

    private void Update()
    {
        if (_player == null)
        {
            return;
        }

        _cameraPosition = _player.transform.position + new Vector3(0, CameraHeight, 0);
        transform.position = _cameraPosition;

        float yRotation = Camera.main.transform.eulerAngles.y;
        transform.eulerAngles = new Vector3(90, yRotation, 0);
    }
}

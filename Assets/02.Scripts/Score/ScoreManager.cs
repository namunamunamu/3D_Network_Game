using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;


public class ScoreManager : MonoBehaviourPunCallbacks
{
    public static ScoreManager Instance { get; private set; }


    private Dictionary<string, int> _scores = new Dictionary<string, int>();
    public Dictionary<string, int> Scores => _scores;

    private int _killcount = 0;
    public int Killcount => _killcount;

    public int KillScore = 500;

    public event Action<List<KeyValuePair<string, int>>> OnDataChanged;
    public event Action<int> OnScoreRewarded;

    private int _score = 0;
    public int Score => _score;


    private void Awake()
    {
        Instance = this;
    }

    public override void OnJoinedRoom()
    {
        Refresh();
    }

    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            Refresh();
        }
    }

    public void Refresh()
    {
        Hashtable hashtable = new Hashtable();
        hashtable.Add("Score", Killcount * KillScore + _score);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
    }

    public void AddScore(int addedScore)
    {
        _score += addedScore;
        Refresh();

        OnScoreRewarded?.Invoke(_score/100);
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        var roomPlayers = PhotonNetwork.PlayerList;
        foreach (Photon.Realtime.Player player in roomPlayers)
        {
            if (player.CustomProperties.ContainsKey("Score"))
            {
                _scores[$"{player.NickName}_{player.ActorNumber}"] = (int)player.CustomProperties["Score"];
            }
        }

        var sortedScores = _scores.ToList().OrderByDescending(x => x.Value).ToList();

        OnDataChanged?.Invoke(sortedScores);
    }

    public int GetLoseScore()
    {
        int lostScore = (_score - (_killcount * KillScore)) / 2;
        AddScore(-lostScore);

        return lostScore;
    }

    public void AddKillCount()
    {
        ++_killcount;
        AddScore(KillScore);
    }
}

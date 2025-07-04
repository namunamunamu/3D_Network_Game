using TMPro;
using UnityEngine;

public class UI_ScoreSlot : MonoBehaviour
{
    public TextMeshProUGUI RankTextUI;
    public TextMeshProUGUI NicknameTextUI;
    public TextMeshProUGUI ScoreTextUI;


    public void Set(string rank, string nickName, int score)
    {
        RankTextUI.text = rank;
        NicknameTextUI.text = nickName;
        ScoreTextUI.text = score.ToString("N0");
    }
}

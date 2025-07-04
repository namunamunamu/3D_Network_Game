using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_Score : MonoBehaviour
{
    public List<UI_ScoreSlot> Slots;
    public UI_ScoreSlot MySlot;

    private void Start()
    {
        ScoreManager.Instance.OnDataChanged += Refresh;
    }

    private void Refresh(List<KeyValuePair<string, int>> sortedScores)
    {
        for (int i = 0; i < Slots.Count; i++)
        {
            if (i < sortedScores.Count)
            {
                Slots[i].gameObject.SetActive(true);
                Slots[i].Set($"{i + 1}", sortedScores[i].Key, sortedScores[i].Value);
            }
            else
            {
                Slots[i].gameObject.SetActive(false);
            }
        }

        // 내점수 등록 과제

        // MySlot.Set()
    }
}

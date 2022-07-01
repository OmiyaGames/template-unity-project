using System;
using UnityEngine;
using UnityEngine.UI;
using OmiyaGames.Saves;

namespace OmiyaGames.Menus
{
    public class TopRecordRow : MonoBehaviour
    {
        [SerializeField]
        TMPro.TextMeshProUGUI placeLabel;
        [SerializeField]
        TMPro.TextMeshProUGUI nameLabel;
        [SerializeField]
        TMPro.TextMeshProUGUI scoreLabel;

        public void UpdateRecord(int placement, IRecord<int> score)
        {
            UpdateRecordHelper(placement, score);
            scoreLabel.text = score.Record.ToString();
        }

        private void UpdateRecordHelper<T>(int placement, IRecord<T> record) where T : IComparable<T>
        {
            placeLabel.text = placement.ToString();
            nameLabel.text = record.Name;
        }
    }
}

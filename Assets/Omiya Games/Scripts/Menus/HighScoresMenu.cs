using UnityEngine;
using UnityEngine.UI;
using System;
using OmiyaGames.Saves;
using OmiyaGames.Global;

namespace OmiyaGames.Menus
{
    public class HighScoresMenu : IMenu
    {
        [Header("High Scores Info")]
        [SerializeField]
        Selectable defaultUi;
        [SerializeField]
        TopRecordRow bestScoreRow;
        [SerializeField]
        TMPro.TextMeshProUGUI noRecordsRow;

        TopRecordRow[] allBestScoreEntries = null;

        public TopRecordRow[] AllBestScoreEntries
        {
            get
            {
                if(allBestScoreEntries == null)
                {
                    allBestScoreEntries = CreateAllRows(bestScoreRow, Singleton.Get<GameSettings>().HighScores);
                }
                return allBestScoreEntries;
            }
        }

        public override Selectable DefaultUi
        {
            get
            {
                return defaultUi;
            }
        }

        public override Type MenuType
        {
            get
            {
                return Type.ManagedMenu;
            }
        }

        public override BackgroundMenu.BackgroundType Background
        {
            get
            {
                return BackgroundMenu.BackgroundType.SolidColor;
            }
        }

        public override string TitleTranslationKey
        {
            get
            {
                return null;
            }
        }

        protected override void OnStateChanged(VisibilityState from, VisibilityState to)
        {
            base.OnStateChanged(from, to);

            // Check if the menu is now visible
            if(to == VisibilityState.Visible)
            {
                SetupRecordsVisibility();
            }
        }

        private void SetupRecordsVisibility()
        {
            // Go through all records
            ISortedRecords<int> allScores = Singleton.Get<GameSettings>().HighScores;
            for (int index = 0; index < AllBestScoreEntries.Length; ++index)
            {
                if (index < allScores.Count)
                {
                    AllBestScoreEntries[index].gameObject.SetActive(true);
                    AllBestScoreEntries[index].UpdateRecord((index + 1), allScores[index]);
                }
                else
                {
                    AllBestScoreEntries[index].gameObject.SetActive(false);
                }
            }

            // Determine whether to show no-records label
            noRecordsRow.gameObject.SetActive(allScores.Count <= 0);
        }

        private static TopRecordRow[] CreateAllRows<T>(TopRecordRow baseRow, ISortedRecords<T> records) where T : IComparable<T>
        {
            TopRecordRow[] returnList = new TopRecordRow[records.MaxCapacity];
            returnList[0] = baseRow;

            GameObject clone = null;
            for (int i = 1; i < returnList.Length; ++i)
            {
                clone = Instantiate(baseRow.gameObject, baseRow.transform.parent);
                clone.transform.localPosition = Vector3.zero;
                clone.transform.localRotation = Quaternion.identity;
                clone.transform.localScale = Vector3.one;
                clone.transform.SetAsLastSibling();

                returnList[i] = clone.GetComponent<TopRecordRow>();
            }
            return returnList;
        }
    }
}

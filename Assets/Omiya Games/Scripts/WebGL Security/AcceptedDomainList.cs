using UnityEngine;

namespace OmiyaGames
{
    public class AcceptedDomainList : ScriptableObject
    {
        [SerializeField]
        string[] domains = null;

        public string[] AllDomains
        {
            get
            {
                return domains;
            }
            set
            {
                domains = value;
            }
        }
    }
}

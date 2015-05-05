using UnityEngine;
using System.Collections;

namespace OmiyaGames
{
    public class SceneTransitionMenu : IMenu
    {
        public override Type MenuType
        {
            get
            {
                return Type.UnmanagedMenu;
            }
        }

        public override GameObject DefaultUi
        {
            get
            {
                return null;
            }
        }

        // FIXME: start implementing these functions
        // FIXME: create animation events on the animator

        public void OnSceneTransitionInEnd()
        {

        }

        public void OnSceneTransitionOutEnd()
        {

        }
    }
}

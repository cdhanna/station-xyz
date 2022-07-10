using UnityEngine;
using UnityEngine.UI;

namespace Components.Hud
{
    public class ActionPointBehaviour : MonoBehaviour
    {
        public RawImage Image;

        public void SetSpent(in bool isSpent)
        {
            Image.color = isSpent ? Color.cyan : Color.grey;
        }
    }
}
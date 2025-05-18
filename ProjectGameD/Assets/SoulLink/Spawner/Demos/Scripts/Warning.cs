using UnityEngine;
using UnityEngine.UI;

namespace Magique.SoulLink
{
    public class Warning : MonoBehaviour
    {
        public Text warningText;

        void Start()
        {
#if SOULLINK_USE_AINAV
        warningText.enabled = false;
#endif
        }
    }
} // namespace Magique.SoulLink
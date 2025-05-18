using UnityEngine;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class GameManager : MonoBehaviour
    {
        static public GameManager Instance { get; private set; }

        [SerializeField]
        private TMPro.TextMeshProUGUI _scoreText;

        [SerializeField]
        private TMPro.TextMeshProUGUI _gameOverText;

        [SerializeField]
        private Transform _popupTextPrefab;

        [SerializeField]
        private Color _popupTextColor;

        private int _points = 0;

        private void Awake()
        {
            Instance = this;
        } // Awake()

        public void GameOver()
        {
            _gameOverText.enabled = true;
        } // GameOver()

        public void AddPoints(int value, bool popupText = true, Transform popupPos = null)
        {
            _points += value;

            if (_scoreText == null) return;

            _scoreText.text = string.Format("{0:000000}", _points);

            if (popupText && _popupTextPrefab != null)
            {
                var obj = SoulLinkGlobal.Instance.Spawn(_popupTextPrefab, popupPos.transform.position, Quaternion.identity, null);
                var text = obj.GetComponent<PopupText>();
                if (text != null)
                {
                    text.SetText(value.ToString(), _popupTextColor);
                } // if
            } // if
        } // AddPoints()
    } // class GameManager
} // namespace Magique.SoulLink
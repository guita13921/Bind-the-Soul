using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// (c)2022 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class SpawnTrackerManager : MonoBehaviour
    {
        [SerializeField]
        private Text _systemMessage;

        [SerializeField]
        private Text _dialogueText;

        [SerializeField]
        private Text _spawnCountText;

        [SerializeField]
        private Text _scoreText;

        [SerializeField]
        private Text _repeatCounterText;

        [SerializeField]
        private Text _interactMessage;

        [SerializeField]
        private Image _interactIcon;

        [SerializeField]
        private Sprite _emptyIcon;


        static private SpawnTrackerManager _instance;

        private readonly object countLock = new object();

        public int SpawnCount
        {
            get { return _spawnCount; }
            set
            {
                lock (countLock)
                {
                    _spawnCount = value;
                    if (_spawnCountText != null)
                    {
                        _spawnCountText.text = _spawnCount.ToString();
                    } // if
                } // lock
            } // set
        }

        private int _spawnCount;

        public int Score
        {
            get { return _score; }
            set 
            {
                _score = value;
                if (_scoreText != null)
                {
                    _scoreText.text = _score.ToString();
                } // if
            }
        }
        private int _score = 0;

        public int RepeatCounter
        {
            get { return _repeatCounter; }
            set
            {
                _repeatCounter = value;
                if (_repeatCounterText != null)
                {
                    _repeatCounterText.text = _repeatCounter.ToString();
                } // if
            }
        }
        private int _repeatCounter = 0;

        static public SpawnTrackerManager Instance
        { get { return _instance; } }

        private GameObject _instructionsShowing = null;

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this);
            }
            else
            {
                _instance = this;
            }
        } // Awake()

        private void Start()
        {
            SetInteractUI(null, "");
        } // Start()

        private void OnEnable()
        {
        } // OnEnable()

        private void OnDisable()
        {
        } // OnDisable()

        public void SetInteractUI(Sprite icon, string text)
        {
            if (_interactMessage != null)
            {
                _interactMessage.text = text;
            } // if

            _interactIcon.sprite = icon != null ? icon : _emptyIcon;
        } // SetInteractUI()

        public void SetSystemMessage(string someText, SoundData soundData = null, float duration = 5.0f)
        {
            if (_instructionsShowing != null)
            {
                _instructionsShowing.SetActive(false);
                _instructionsShowing = null;
            }

            _systemMessage.text = someText;

            if (soundData != null)
            {
                SoulLinkGlobal.Instance.PlaySound(soundData, Vector3.zero);
            }

            StartCoroutine(RemoveSystemMessage(duration));
        } // SetMessageText()

        IEnumerator RemoveSystemMessage(float duration)
        {
            yield return new WaitForSeconds(duration);
            _systemMessage.text = "";
        } // RemoveSystemMessage()

        public void SetDialogueText(string someText, float duration = 5.0f)
        {
            _dialogueText.text = someText;

            StartCoroutine(RemoveDialogueText(duration));
        } // SetDialogueText()

        IEnumerator RemoveDialogueText(float duration)
        {
            yield return new WaitForSeconds(duration);
            _dialogueText.text = "";
        } // RemoveDialogueText()

        public void ShowInstructions(GameObject someObject, float duration)
        {
            SetSystemMessage("");
            SetDialogueText("");
            _instructionsShowing = someObject;

            someObject.SetActive(true);
            StartCoroutine(HideInstructions(someObject, duration));
        } // ShowInstructions()

        IEnumerator HideInstructions(GameObject someObject, float duration)
        {
            yield return new WaitForSeconds(duration);

            someObject.SetActive(false);
            _instructionsShowing = null;
        } // HideInstructions()
    } // class SpawnTrackerManager
} // namespace Magique.SoulLink
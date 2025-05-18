using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public class Dialogue : MonoBehaviour
    {
        [SerializeField]
        private float _delayStart = 5f;

        [SerializeField]
        private float _delayBetween = 1f;

        [SerializeField]
        private List<DialogueData> _dialogueData = new List<DialogueData>();

        [SerializeField]
        private UnityEvent _onDialogCompleteEventHandler;

        void Start()
        {
            StartCoroutine(ShowDialogue());
        } // Start()

        IEnumerator ShowDialogue()
        {
            yield return new WaitForSeconds(_delayStart);

            foreach (var dialogueItem in _dialogueData)
            {
                dialogueItem.sprite.SetActive(true);
                dialogueItem.dialogueTextObject.gameObject.SetActive(true);
                dialogueItem.dialogueTextObject.text = dialogueItem.dialogueText;
                SoulLink.SoulLinkGlobal.Instance.PlaySound(new SoulLink.SoundData() { audioClip = dialogueItem.dialogueAudioClip, soundIs3D = false }, Vector3.zero);

                yield return new WaitForSeconds(dialogueItem.delayNext);

                dialogueItem.sprite.SetActive(false);
                dialogueItem.dialogueTextObject.gameObject.SetActive(false);
                dialogueItem.dialogueTextObject.text = "";

                yield return new WaitForSeconds(_delayBetween);
            } // foreach

            if (_onDialogCompleteEventHandler != null)
            {
                _onDialogCompleteEventHandler.Invoke();
            } // if
        } // ShowDialogue()
    } // class Dialogue
} // namespace Magique.SoulLink
#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Events;

namespace DM
{
    public class AudioManager : MonoBehaviour
    {
        [Space(20)]
        [Header("Auto add audio to every button that is child of GO")]
        
        [SerializeField] GameObject _go;
        [SerializeField] AudioSource _audioSource;
        private Button[] _buttons;

        public void AddAudio() {
            _buttons = _go.GetComponentsInChildren<Button>(true);
            foreach (var btn in _buttons)
            {
                UnityEventTools.AddPersistentListener(btn.onClick, _audioSource.Play);
            }
        }

        public void RemoveAudio() {
            _buttons = _go.GetComponentsInChildren<Button>(true);
            foreach (var btn in _buttons)
            {
                UnityEventTools.RemovePersistentListener(btn.onClick, _audioSource.Play);
            }
        }
    }
}
#endif

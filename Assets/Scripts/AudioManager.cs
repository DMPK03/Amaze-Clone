using UnityEngine;
using UnityEngine.UI;

namespace DM
{
    public class AudioManager : MonoBehaviour
    {
        [Space(20)]
        [Header("Auto add audio to every button, so i dont have to do it manualy.")]
        
        [SerializeField] GameObject _canvas;
        [SerializeField] AudioSource _audioSource;
        private Button[] _buttons;

        private void Start() {
            _buttons = _canvas.GetComponentsInChildren<Button>(true);
            foreach (var btn in _buttons)
            {
                btn.onClick.AddListener(_audioSource.Play);
            }
        }
    }
}

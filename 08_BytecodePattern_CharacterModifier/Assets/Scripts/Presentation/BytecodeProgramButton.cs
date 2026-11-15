using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterModifierBytecodeDemo.Presentation
{
    [RequireComponent(typeof(Button))]
    public sealed class BytecodeProgramButton : MonoBehaviour
    {
        [SerializeField] private CharacterModifierController _controller;
        [SerializeField] private string _programFileName;
        [SerializeField] private TMP_Text _label;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(ExecuteProgram);
        }

        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(ExecuteProgram);
            }
        }

        public void Configure(
            CharacterModifierController controller,
            string programFileName,
            TMP_Text label,
            string displayName)
        {
            _controller = controller;
            _programFileName = programFileName;
            _label = label;

            if (_label != null)
            {
                _label.text = displayName;
            }
        }

        private void ExecuteProgram()
        {
            if (_controller == null)
            {
                Debug.LogError("BytecodeProgramButton requires a CharacterModifierController.");
                return;
            }

            _controller.ExecuteProgram(_programFileName);
        }
    }
}

using System;
using System.Collections;
using CharacterModifierBytecodeDemo.Bytecode;
using CharacterModifierBytecodeDemo.Character;
using TMPro;
using UnityEngine;

namespace CharacterModifierBytecodeDemo.Presentation
{
    public sealed class CharacterModifierController : MonoBehaviour
    {
        private const string ProgramFolderName = "BytecodePrograms";

        [SerializeField] private CharacterModifierTarget _target;
        [SerializeField] private TMP_Text _stateText;
        [SerializeField] private TMP_Text _programText;
        [SerializeField] private TMP_Text _sourceText;
        [SerializeField] private TMP_Text _pathText;

        private readonly BytecodeVm _vm = new BytecodeVm();
        private readonly BytecodeCompiler _compiler = new BytecodeCompiler();
        private readonly CharacterBytecodeSymbolTable _symbolTable = new CharacterBytecodeSymbolTable();

        private BytecodeProgramLoader _loader;
        private CharacterBytecodeContext _context;
        private bool _isExecuting;

        private void Awake()
        {
            if (_target == null)
            {
                Debug.LogError("CharacterModifierController requires a CharacterModifierTarget.");
                enabled = false;
                return;
            }

            _loader = new BytecodeProgramLoader(ProgramFolderName);
            _context = new CharacterBytecodeContext(_target);

            RefreshStateText();

            if (_programText != null)
            {
                _programText.text = "Last program: none";
            }

            if (_sourceText != null)
            {
                _sourceText.text =
                    "External program source will appear here.\n\n" +
                    "Edit a .bcasm file in StreamingAssets, save it, then click its button again.";
            }

            if (_pathText != null)
            {
                _pathText.text = $"Program folder:\n{_loader.GetDisplayPath(string.Empty)}";
            }
        }

        private void Update()
        {
            RefreshStateText();
        }

        public void ExecuteProgram(string fileName)
        {
            if (_isExecuting)
            {
                return;
            }

            StartCoroutine(LoadCompileAndExecute(fileName));
        }

        public void Configure(
            CharacterModifierTarget target,
            TMP_Text stateText,
            TMP_Text programText,
            TMP_Text sourceText,
            TMP_Text pathText)
        {
            _target = target;
            _stateText = stateText;
            _programText = programText;
            _sourceText = sourceText;
            _pathText = pathText;
        }

        private IEnumerator LoadCompileAndExecute(string fileName)
        {
            _isExecuting = true;

            string source = null;
            string loadError = null;

            yield return _loader.LoadSource(
                fileName,
                loadedSource => source = loadedSource,
                error => loadError = error);

            if (loadError != null)
            {
                ShowError(loadError);
                _isExecuting = false;
                yield break;
            }

            if (_sourceText != null)
            {
                _sourceText.text = source;
            }

            try
            {
                BytecodeProgram program = _compiler.Compile(fileName, source, _symbolTable);
                _vm.Execute(program, _context);

                if (_programText != null)
                {
                    _programText.text =
                        $"Executed: {fileName}\n" +
                        $"Compiled bytecode size: {program.Code.Length} bytes";
                }
            }
            catch (Exception exception)
            {
                ShowError(exception.Message);
            }
            finally
            {
                _isExecuting = false;
            }
        }

        private void RefreshStateText()
        {
            if (_stateText == null || _target == null)
            {
                return;
            }

            _stateText.text =
                $"Scale: {_target.Scale:F2}\n" +
                $"Move Speed: {_target.MoveSpeed:F2}\n" +
                $"Health: {_target.Health:F0}";
        }

        private void ShowError(string message)
        {
            Debug.LogError(message);

            if (_programText != null)
            {
                _programText.text = $"ERROR\n{message}";
            }
        }
    }
}

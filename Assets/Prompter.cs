using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace PromptLink {

public sealed class Prompter : MonoBehaviour
{
    #region Project assets

    [SerializeField] TextAsset[] _sources = null;

    #endregion

    #region Scene objects

    [field:SerializeField] public TMP_Text _targetUI { get; set; } = null;

    #endregion

    #region Key assign

    [field:SerializeField] public InputAction _backAction { get; set; } = null;
    [field:SerializeField] public InputAction _nextAction { get; set; } = null;

    #endregion

    #region Text and reading position

    List<string[]> _lines = new List<string[]>();
    (int chapter, int line) _position;

    #endregion

    #region Reader methods

    int ChapterCount => _lines.Count;
    int ChapterLineCount => _lines[_position.chapter].Length;

    void ReadCurrentLine()
      => _targetUI.text = _lines[_position.chapter][_position.line];

    void StepBackward()
    {
        if (_position.line > 0)
        {
            _position.line--;
        }
        else if (_position.chapter > 0)
        {
            _position.chapter--;
            _position.line = ChapterLineCount - 1;
        }
    }

    void StepForward()
    {
        if (_position.line < ChapterLineCount - 1)
        {
            _position.line++;
        }
        else if (_position.chapter < ChapterCount - 1)
        {
            _position.chapter++;
            _position.line = 0;
        }
    }

    #endregion

    #region Input system

    static Key GetChapterKey(int index)
    {
        if (index < 9) return Key.Digit1 + index;
        return Key.A + (index - 9);
    }

    void OnBackPressed(InputAction.CallbackContext _)
    {
        StepBackward();
        ReadCurrentLine();
    }

    void OnNextPressed(InputAction.CallbackContext _)
    {
        StepForward();
        ReadCurrentLine();
    }

    #endregion

    #region MonoBehaviour implementation

    void OnEnable()
    {
        _backAction.performed += OnBackPressed;
        _nextAction.performed += OnNextPressed;
        _backAction.Enable();
        _nextAction.Enable();
    }

    void OnDisable()
    {
        _backAction.performed -= OnBackPressed;
        _nextAction.performed -= OnNextPressed;
        _backAction.Disable();
        _nextAction.Disable();
    }

    void Start()
    {
        foreach (var chapter in _sources)
            _lines.Add(chapter.text.Split("\n").SkipLast(1).ToArray());
        ReadCurrentLine();
    }

    void Update()
    {
        var keys = Keyboard.current;
        for (var i = 0; i < _lines.Count; i++)
        {
            if (keys[GetChapterKey(i)].wasPressedThisFrame)
            {
                _position = (i, 0);
                ReadCurrentLine();
                break;
            }
        }
    }

    #endregion
}

} // namespace PromptLink

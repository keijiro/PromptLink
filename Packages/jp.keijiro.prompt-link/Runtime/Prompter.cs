using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace PromptLink {

public sealed class Prompter : MonoBehaviour
{
    #region Project assets

    [SerializeField] TextAsset[] _sources = null;

    #endregion

    #region Key assign

    [field:SerializeField] public InputAction BackAction { get; set; } = null;
    [field:SerializeField] public InputAction NextAction { get; set; } = null;

    #endregion

    #region Text and reading position

    List<string[]> _lines = new List<string[]>();
    (int chapter, int line) _position;

    #endregion

    #region Reader methods

    int ChapterCount => _lines.Count;
    int ChapterLineCount => _lines[_position.chapter].Length;

    void ReadCurrentLine()
      => GetComponent<UIDocument>().rootVisualElement.Q<Label>("text").text
           = _lines[_position.chapter][_position.line];

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
        BackAction.performed += OnBackPressed;
        NextAction.performed += OnNextPressed;
        BackAction.Enable();
        NextAction.Enable();
    }

    void OnDisable()
    {
        BackAction.performed -= OnBackPressed;
        NextAction.performed -= OnNextPressed;
        BackAction.Disable();
        NextAction.Disable();
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

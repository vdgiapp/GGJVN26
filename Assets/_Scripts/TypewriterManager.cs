using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypewriterManager : MonoBehaviour
{
    [SerializeField] private Text _messageText;
    [SerializeField] private float _characterDelay = 0.05f;
    
    private Coroutine _typingCoroutine;
    private bool _isTyping;
    private bool _canContinue;
    
    private string _currentFullMessage;
    private List<string> _currentMessages;
    private int _currentMessageIndex;
    
    private Action _onDialogueFinished;

    public bool IsTyping() => _isTyping;
    public bool CanContinue() => _canContinue;
    public bool IsInDialogue() => _currentMessages != null;

    public void StartDialogue(List<string> messages, Action onFinished = null)
    {
        if (messages == null || messages.Count == 0)
        {
            ShowMessageInstant("...");
            _currentMessages = null;
            _onDialogueFinished?.Invoke();
            return;
        }
        _currentMessages = messages;
        _currentMessageIndex = 0;
        _onDialogueFinished = onFinished;
        StartTypingMessage(_currentMessages[_currentMessageIndex]);
    }
    
    private void StartTypingMessage(string message)
    {
        _currentFullMessage = message;
        _canContinue = false;

        if (_typingCoroutine != null)
            StopCoroutine(_typingCoroutine);

        _typingCoroutine = StartCoroutine(TypeMessageCoroutine(message));
    }
    
    private IEnumerator TypeMessageCoroutine(string message)
    {
        _isTyping = true;
        _messageText.text = "";

        for (int i = 0; i < message.Length; i++)
        {
            _messageText.text += message[i];
            yield return new WaitForSeconds(_characterDelay);
        }

        _isTyping = false;
        _canContinue = true;
    }
    
    private void ShowMessageInstant(string message)
    {
        if (_typingCoroutine != null)
            StopCoroutine(_typingCoroutine);

        _isTyping = false;
        _canContinue = true;

        _currentFullMessage = message;
        _messageText.text = message;
    }
    
    public void SkipTypingText()
    {
        if (!_isTyping) return;
        ShowMessageInstant(_currentFullMessage);
    }
    
    public void ContinueMessageText()
    {
        if (!_canContinue) return;

        _canContinue = false;

        if (_currentMessages == null)
        {
            _messageText.text = "";
            return;
        }

        _currentMessageIndex++;

        // hết hội thoại
        if (_currentMessageIndex >= _currentMessages.Count)
        {
            EndDialogue();
            return;
        }

        StartTypingMessage(_currentMessages[_currentMessageIndex]);
    }
    
    private void EndDialogue()
    {
        _messageText.text = "";
        _currentMessages = null;
        Action callback = _onDialogueFinished;
        _onDialogueFinished = null;
        callback?.Invoke();
    }
    
    public void ForceStopDialogue()
    {
        if (_typingCoroutine != null)
            StopCoroutine(_typingCoroutine);

        _isTyping = false;
        _canContinue = false;

        _messageText.text = "";
        _currentMessages = null;
        _onDialogueFinished = null;
    }
}
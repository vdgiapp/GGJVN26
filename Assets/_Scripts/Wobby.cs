using System;
using UnityEngine;

public class Wobby : MonoBehaviour
{
    [SerializeField] private float strength = 1f;
    [SerializeField] private float frequency = 1f;
    [SerializeField] private bool reverse = false;
    
    private float _original;
    private float _timer;

    private int _state = 0; // 0 = original + strength, 1 = original - strength
    
    private void Awake()
    {
        _original = gameObject.transform.rotation.eulerAngles.z;
    }

    private void Update()
    {
        _timer += Time.unscaledDeltaTime;

        if (_timer >= 1f / frequency)
        {
            _timer = 0f;

            float dir = reverse ? -1f : 1f; // <-- thêm dòng này

            float targetZ = (_state == 0)
                ? _original + strength * dir
                : _original - strength * dir;

            transform.localRotation = Quaternion.Euler(0f, 0f, targetZ);

            _state = 1 - _state;
        }
    }
}
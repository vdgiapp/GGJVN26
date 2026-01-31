using System;
using UnityEngine;

public class UpNDown : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.25f;
    [SerializeField] private float frequency = 1f;
    
    private Vector3 _startPos;

    private void Awake()
    {
        _startPos = transform.localPosition;
    }
    
    private void Update()
    {
        float t = Time.time;

        // sin chạy từ -1 đến 1
        float yOffset = Mathf.Sin(t * frequency) * amplitude;

        transform.localPosition = _startPos + new Vector3(0f, yOffset, 0f);
    }
}
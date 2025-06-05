using System;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public int Value;
    private Material _instanceMaterial;

    [Header("Color Lerp Settings")]
    [SerializeField] private Color[] _valueColors; // List of colors (e.g., green, yellow, orange, red)
    [SerializeField] private int _minValue = 0;
    [SerializeField] private int _maxValue = 100;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;
        other.GetComponentInChildren<PlayerHandler>().PickUpGem(this.gameObject);
    }

    private void Awake()
    {
        _instanceMaterial = GetComponent<Renderer>().material;
    }

    public void Initialize(int newValue)
    {
        Value = newValue;

        int colorCount = _valueColors.Length - 1;
        if (colorCount < 1) return;

        float lerpValue = Mathf.InverseLerp(_minValue, _maxValue, Value);
        float scaledLerpValue = lerpValue * colorCount;
        int colorIndex = Mathf.FloorToInt(scaledLerpValue);

        if (colorIndex >= colorCount)
            colorIndex = colorCount - 1;

        float lerpBetweenCurrentColors = scaledLerpValue - colorIndex;

        Color color = Color.Lerp(_valueColors[colorIndex], _valueColors[colorIndex + 1], lerpBetweenCurrentColors);
        _instanceMaterial.color = color;
    }
}

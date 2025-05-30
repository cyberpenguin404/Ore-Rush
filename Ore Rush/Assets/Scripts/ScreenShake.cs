using System.Collections;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    private bool _shaking;

    private Vector3 _originalPosition;
    private void Start()
    {
        _originalPosition = transform.position;
    }

    public void Shake(float duration, float magnitude)
    {
        if (_shaking)
        {
            return;
        }
        StartCoroutine(ShakeCoroutine(duration, magnitude));

    }
    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = _originalPosition + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = _originalPosition;
    }
}

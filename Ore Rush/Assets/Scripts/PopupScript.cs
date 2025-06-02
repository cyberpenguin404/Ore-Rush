using TMPro;
using UnityEngine;

public class PopupScript : MonoBehaviour
{
    public TextMeshProUGUI Text;
    [SerializeField]
    private RectTransform _rectTransform;
    [SerializeField]
    private float _verticalMovement;
    [SerializeField]
    private float _alphaFade;
    private float _alpha = 1;
    void Update()
    {
        _rectTransform.anchoredPosition += new Vector2(0, _verticalMovement * Time.deltaTime);
        Text.color = new Color(Text.color.r,Text.color.g,Text.color.b, _alpha);
        _alpha -= _alphaFade * Time.deltaTime;

        if (_alpha <= 0 )
        {
            Destroy(gameObject);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace VirtualTour
{
    [RequireComponent(typeof(Text))]
    public class TextController : MonoBehaviour
    {
        private Text _text;

        void Start()
        {
            _text = GetComponent<Text>();
            // ������������� �����
            int len = _text.text.Length;
            if (len > 100)
                _text.GetComponent<RectTransform>().sizeDelta = new Vector2(_text.GetComponent<RectTransform>().sizeDelta.x, len / 2);


            // ����������� �������� ��� �������� ������
            _text.horizontalOverflow = HorizontalWrapMode.Wrap; // ��������� ������� �� ����� ������
            _text.verticalOverflow = VerticalWrapMode.Truncate; // ���������� ���������� �����
            
        }
    }
}
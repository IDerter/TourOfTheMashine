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
            // Устанавливаем текст
            int len = _text.text.Length;
            if (len > 100)
                _text.GetComponent<RectTransform>().sizeDelta = new Vector2(_text.GetComponent<RectTransform>().sizeDelta.x, len / 2);


            // Настраиваем свойства для переноса текста
            _text.horizontalOverflow = HorizontalWrapMode.Wrap; // Разрешить перенос на новую строку
            _text.verticalOverflow = VerticalWrapMode.Truncate; // Ограничить количество строк
            
        }
    }
}
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;

namespace VirtualTour
{

    [RequireComponent(typeof(Button))]
    public class SaveFileText : MonoBehaviour, IPointerDownHandler
    {
        //public Text output;
        private Button _button;

        // Sample text data
        private string _data = "";

        [SerializeField] private TextScrollTxt _textScrollTxt;

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

    // Broser plugin should be called in OnPointerDown.
    public void OnPointerDown(PointerEventData eventData) {
        var bytes = Encoding.UTF8.GetBytes(_data);
        DownloadFile(gameObject.name, "OnFileDownload", "sample.txt", bytes, bytes.Length);
    }

    // Called from browser
    public void OnFileDownload() {
        output.text = "File Successfully Downloaded";
    }
#else
        //
        // Standalone platforms & editor
        //
        public void OnPointerDown(PointerEventData eventData) { }

        // Listen OnClick event in standlone builds
        void Start()
        {

        }

        private void OnEnable()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        public void OnClick()
        {
            var path = StandaloneFileBrowser.SaveFilePanel("Title", "", "sample", "txt");
            if (!string.IsNullOrEmpty(path))
            {
                File.WriteAllText(path, _data);
            }
        }

        public void SaveData(string path, string data)
        {
            if (!string.IsNullOrEmpty(path))
            {
                string allData = File.ReadAllText(path);
                File.WriteAllText(path, allData);
                allData = allData + "\n" + data;
                // Debug.Log(_textScrollTxt.ParseNameInPath(path));
                TextStorage.SaveTextResult(allData, _textScrollTxt.ParseNameInPath(path));
                _textScrollTxt.TextShowInMachine(_textScrollTxt.ParseNameInPath(path));

              //  Debug.Log("Save Data to " + path);
                Debug.Log(allData);
            }
        }
#endif
    }
}
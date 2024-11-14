using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace VirtualTour
{
    public class TextScrollTxt : SingletonBase<TextScrollTxt>
    {
        [SerializeField] private Transform _contentWindow;
        [SerializeField] private GameObject _recallTextObject;

        private void Start()
        {
            
        }

        public void TextShowInMachine(string nameFile)
        {

            string readFromFilePath = Application.streamingAssetsPath + "/Recall_Chat/" + nameFile;
            Debug.Log(readFromFilePath);
            Debug.Log(nameFile);
            //File.Rea
            List<string> fileLines = File.ReadAllLines(readFromFilePath).ToList();

            foreach (var line in fileLines)
            {
                SpawnRecallObject(line);
            }
        }

        private void SpawnRecallObject(string line)
        {
            Instantiate(_recallTextObject, _contentWindow);
            _recallTextObject.GetComponent<Text>().text = line;
        }

        public void ShowLoadData(string data)
        {
            char delimiter = '\n'; // символ '\' записываем в переменную типа char

            string[] parts = data.Split(delimiter);

            foreach (var line in parts)
            {
                SpawnRecallObject(line);
            }
        }

        public string ParseNameInPath(string path)
        {
            char delimiter = '/'; // символ '\' записываем в переменную типа char

            string[] parts = path.Split(delimiter);

            return parts[parts.Length - 1];
        }

    }
}
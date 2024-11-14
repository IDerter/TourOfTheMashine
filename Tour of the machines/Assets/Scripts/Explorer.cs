using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace VirtualTour {
    public class Explorer : MonoBehaviour
    {
        public Text eText;
        public Button openExplorerButton;

        public string finalPath;

        private bool readText = false;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

 

        private string ParseNameFileInPath(string path)
        {
            string nameFile;

            string[] words = path.Split('\\');

            nameFile = words[words.Length - 1];

            return nameFile;
        }

        void ReadText(string path)
        {
            eText.text = File.ReadAllText(path);

            Debug.Log(path);
            Debug.Log(ParseNameFileInPath(path));
            TextStorage.SaveTextResult(eText.text, ParseNameFileInPath(path));
        }

    }
}
using UnityEngine.SceneManagement;
using UnityEngine;

using System;

using UnityEngine.Events;
using System.Collections.Generic;
namespace VirtualTour
{
    public class TextStorage : SingletonBase<TextStorage>
    {

        private const string _fileName = "savetexts.dat";
        public string FileName => _fileName;

        [Serializable]
        private class TextSave
        {
            [SerializeField] private string _textToSave;
            [SerializeField] private string _textNameTXT;
            public string TextToSave { get => _textToSave; set => _textToSave = value; }
            public string TextNameText { get => _textNameTXT; set => _textNameTXT = value; }

        }

        [SerializeField] private TextSave[] _completionData;
        [SerializeField] private TextSave[] _completionDataUnity;

        //[SerializeField] private StorageEpisode _storageEpisode;



        private new void Awake()
        {
            base.Awake();
            SaveNewInUnityProgress();
        }

        private void Start()
        {
            LoadData();
        }

        public virtual void LoadData()
        {
            bool flag = Saver<TextSave[]>.TryLoad(_fileName, ref _completionData);
            Debug.Log(flag + " произошло сохранение!");
        }

        public static string CheckSaveData(string name)
        {
            //bool flag = Saver<TextSave[]>.TryLoad(_fileName, ref _completionData);
            if (Instance)
            {

                foreach (var item in Instance._completionDataUnity)
                {
                    if (item.TextNameText == name)
                    {

                        return item.TextToSave;
                    }
                }

            }
            return "";


        }

        public void SaveNewInUnityProgress()
        {
            bool flag = Saver<TextSave[]>.TryLoad(_fileName, ref _completionData);

            Debug.Log(flag + " произошло попытка получени€ сохранение!");

            if (flag)
            {
                for (int i = 0; i < _completionData.Length; i++)
                {
                    _completionDataUnity[i] = _completionData[i];

                }
            }
            else
            {
                ResetTextResult();
            }
        }

        public static void SaveTextResult(string newText, string nameTextToSave)
        {
            Debug.Log("StartSave");

            if (Instance)
            {

                foreach (var item in Instance._completionDataUnity)
                {
                    if (item.TextNameText == nameTextToSave)
                    {
                        item.TextToSave = newText;
                        break;
                    }

                    if (item.TextNameText == "" && item.TextToSave == "")
                    {
                        item.TextNameText = nameTextToSave;
                        item.TextToSave = newText;
                        break;
                    }


                }
                Saver<TextSave[]>.Save(_fileName, Instance._completionDataUnity);
            }

            if (Instance)
            {

                foreach (var item in Instance._completionData)
                {


                    if (item.TextNameText == nameTextToSave)
                    {
                        item.TextToSave = newText;
                        break;
                    }

                    if (item.TextNameText == "" && item.TextToSave == "")
                    {
                        item.TextNameText = nameTextToSave;
                        item.TextToSave = newText;
                        break;
                    }

                }
                Saver<TextSave[]>.Save(_fileName, Instance._completionData);
            }


        }



        public static void ResetTextResult()
        {
            if (Instance)
            {
                foreach (var item in Instance._completionDataUnity)
                {

                        item.TextToSave = "";

                        Saver<TextSave[]>.Save(_fileName, Instance._completionDataUnity);
                        Debug.Log("ѕроизошел ResetEpisdoeResult");
                }
            }

            

            if (Instance)
            {
                foreach (var item in Instance._completionData)
                {

                     item.TextToSave = "";
                     Saver<TextSave[]>.Save(_fileName, Instance._completionData);
                     Debug.Log("ѕроизошел ResetTextResult");
                }
            
            }
        }

    }
}

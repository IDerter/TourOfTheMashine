using System;
using System.IO;
using UnityEngine;

namespace VirtualTour
{
    [Serializable]
    public class Saver<T> // класс является обобщенным параметр T в угловых скобках еще называется универсальным параметром, так как вместо него можно подставить любой тип.
    {
        public T _data;
        //public T Data => _data;

        public static bool TryLoad(string fileName, ref T data)
        {
            var path = FileHandler.Path(fileName);
            if (File.Exists(path))
            {
                Debug.Log($"loading from {path}");
                var dataString = File.ReadAllText(path);
                var saver = JsonUtility.FromJson<Saver<T>>(dataString);
                data = saver._data;
                return true;
            }
            else
            {
                Debug.Log($"not exists {path}");
                return false;
            }
        }

        public static void Save(string fileName, T data)
        {
            var wrapper = new Saver<T> { _data = data };

            var dataString = JsonUtility.ToJson(wrapper);
            File.WriteAllText(FileHandler.Path(fileName), dataString);
            Debug.Log("Произошло сохранение!" + FileHandler.Path(fileName) + dataString);
        }
    }

    public static class FileHandler
    {
        public static string Path(string fileName)
        {

            return $"{Application.persistentDataPath}/{fileName}";
        }

        public static void Reset(string fileName)
        {
            var path = Path(fileName);
            Debug.Log(path + "Try to delete!");
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log("Delete path");
            }
        }

        public static bool HasFile(string fileName)
        {
            return File.Exists(Path(fileName));
        }
    }
}
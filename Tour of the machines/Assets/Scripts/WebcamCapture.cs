using UnityEngine;
using UnityEngine.Networking;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ObjdetectModule;
using OpenCVForUnity.VideoioModule;
using OpenCVForUnity.UnityUtils;
using System.Collections;

public class WebcamCapture : MonoBehaviour
{
    private Mat frame;

    private string videoURL = "http://192.168.0.109:8080"; // Укажите URL вашей камеры
    [SerializeField]

    void Start()
    {
        frame = new Mat();
        StartCoroutine(GetVideoStream());
    }

    private IEnumerator GetVideoStream()
    {
        while (true)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(videoURL))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error: " + www.error);
                }
                else
                {
                    Debug.Log("Success");
                    // Обработка данных, если запрос успешный.
                    // Важно: проверьте, возможно ли, что данные, которые вы получаете, могут быть текстурой
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(www.downloadHandler.data); // Загрузите данные в текстуру

                    // Преобразование текстуры в Mat для использования с OpenCV
                    Mat mat = new Mat(texture.height, texture.width, CvType.CV_8UC4);
                    Utils.texture2DToMat(texture, mat);

                    // Здесь добавьте обработку с помощью OpenCV
                }
            }
            yield return new WaitForSeconds(0.1f); // Четкая задержка для предотвращения перегрузки
        }
    }

    private void OnApplicationQuit()
    {
        if (frame != null)
            frame.Dispose();
    }
}
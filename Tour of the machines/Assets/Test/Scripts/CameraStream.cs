using OpenCVForUnity.CoreModule;
using OpenCVForUnity.DnnModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CameraStream : MonoBehaviour
{
    public RawImage rawImage;
    private WebCamTexture webCamTexture;
    private Mat frame;
    private Texture2D texture;

    private Net net;
    [SerializeField] private List<string> classNames;

    void Start()
    {
        // Инициализация камеры
        webCamTexture = new WebCamTexture();
        rawImage.texture = webCamTexture;
        webCamTexture.Play();

        frame = new Mat(webCamTexture.height, webCamTexture.width, CvType.CV_8UC4);
        texture = new Texture2D(frame.cols(), frame.rows(), TextureFormat.RGBA32, false);

        net = Dnn.readNetFromDarknet("yolov3.cfg", "yolov3.weights");
        classNames = File.ReadAllLines(Application.streamingAssetsPath + "/coco.names").ToList();
    }

    void Update()
    {
        if (webCamTexture.didUpdateThisFrame && webCamTexture.isPlaying)
        {
            Utils.webCamTextureToMat(webCamTexture, frame);

            // Подготовка blob для DNN
            Mat blob = Dnn.blobFromImage(frame, 1 / 255.0, new Size(416, 416), new Scalar(0, 0, 0), true, false);
            net.setInput(blob);

            // Получение результатов
            Mat output = net.forward();

            // Обработка результатов (пример для YOLO)
            for (int i = 0; i < output.rows(); i++)
            {
                Mat row = output.row(i);
                Mat scores = row.colRange(5, output.cols());
                Core.MinMaxLocResult result = Core.minMaxLoc(scores);

                if (result.maxVal > 0.5)
                { // Порог уверенности
                    int centerX = (int)(row.get(0, 0)[0] * frame.width());
                    int centerY = (int)(row.get(0, 1)[0] * frame.height());
                    int width = (int)(row.get(0, 2)[0] * frame.width());
                    int height = (int)(row.get(0, 3)[0] * frame.height());

                    // Отрисовка
                    Imgproc.rectangle(frame,
                        new Point(centerX - width / 2, centerY - height / 2),
                        new Point(centerX + width / 2, centerY + height / 2),
                        new Scalar(0, 255, 0), 2);

                    // Подпись класса
                    Imgproc.putText(frame, classNames[(int)result.maxLoc.x],
                        new Point(centerX - width / 2, centerY - height / 2 - 5),
                        Imgproc.FONT_HERSHEY_SIMPLEX, 0.5, new Scalar(0, 0, 255), 1);
                }
            }

            Utils.matToTexture2D(frame, texture);
            rawImage.texture = texture;
        }
    }
}

#if !UNITY_WSA_10_0

using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgcodecsModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.UnityUtils.Helper;
using OpenCVForUnityExample.DnnModel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OpenCVForUnityExample
{
    /// <summary>
    /// Object Detection YOLOv4 Example
    /// An example of using OpenCV dnn module with YOLOv4 Object Detection.
    /// Referring to https://github.com/AlexeyAB/darknet.
    /// https://gist.github.com/YashasSamaga/48bdb167303e10f4d07b754888ddbdcf
    /// 
    /// [Tested Models]
    /// yolov4-tiny https://github.com/AlexeyAB/darknet/releases/download/yolov4/yolov4-tiny.weights, https://raw.githubusercontent.com/AlexeyAB/darknet/0faed3e60e52f742bbef43b83f6be51dd30f373e/cfg/yolov4-tiny.cfg
    /// yolov4 https://github.com/AlexeyAB/darknet/releases/download/yolov4/yolov4.weights, https://raw.githubusercontent.com/AlexeyAB/darknet/0faed3e60e52f742bbef43b83f6be51dd30f373e/cfg/yolov4.cfg
    /// </summary>
    [RequireComponent(typeof(WebCamTextureToMatHelper))]
    public class ObjectDetectionYOLOv4Example : MonoBehaviour
    {
        [TooltipAttribute("Path to a binary file of model contains trained weights. It could be a file with extensions .caffemodel (Caffe), .pb (TensorFlow), .t7 or .net (Torch), .weights (Darknet).")]
        public string model = "yolov4-tiny.weights";

        [TooltipAttribute("Path to a text file of model contains network configuration. It could be a file with extensions .prototxt (Caffe), .pbtxt (TensorFlow), .cfg (Darknet).")]
        public string config = "yolov4-tiny.cfg";

        [TooltipAttribute("Optional path to a text file with names of classes to label detected objects.")]
        public string classes = "coco.names";

        [TooltipAttribute("Confidence threshold.")]
        public float confThreshold = 0.25f;

        [TooltipAttribute("Non-maximum suppression threshold.")]
        public float nmsThreshold = 0.45f;

        //[TooltipAttribute("Maximum detections per image.")]
        //public int topK = 1000;

        [TooltipAttribute("Preprocess input image by resizing to a specific width.")]
        public int inpWidth = 416;

        [TooltipAttribute("Preprocess input image by resizing to a specific height.")]
        public int inpHeight = 416;


        [Header("TEST")]

        [TooltipAttribute("Path to test input image.")]
        public string testInputImage;


        protected string classes_filepath;
        protected string config_filepath;
        protected string model_filepath;


        /// <summary>
        /// The texture.
        /// </summary>
        Texture2D texture;

        /// <summary>
        /// The webcam texture to mat helper.
        /// </summary>
        WebCamTextureToMatHelper webCamTextureToMatHelper;

        /// <summary>
        /// The bgr mat.
        /// </summary>
        Mat bgrMat;

        /// <summary>
        /// The YOLOv4 ObjectDetector.
        /// </summary>
        YOLOv4ObjectDetector objectDetector;

        /// <summary>
        /// The FPS monitor.
        /// </summary>
        FpsMonitor fpsMonitor;

        public GameObject personPrefab; // Префаб для визуализации человека
        private CameraToWorldMapper coordinateMapper;
        private GameObject trackedPerson;

        [Header("Kalman Filter Settings")]
        public float kalmanSmoothness = 0.1f;

        [Header("Tracking Settings")]
        public float maxInvisibleTime = 0.1f;
        private Dictionary<int, TrackedPerson> activePersons = new Dictionary<int, TrackedPerson>();
        private Dictionary<int, float> invisibleTimers = new Dictionary<int, float>();
        private int nextId = 1;

#if UNITY_WEBGL
        IEnumerator getFilePath_Coroutine;
#endif

        // Use this for initialization
        void Start()
        {
            fpsMonitor = GetComponent<FpsMonitor>();

            webCamTextureToMatHelper = gameObject.GetComponent<WebCamTextureToMatHelper>();

#if UNITY_WEBGL
            getFilePath_Coroutine = GetFilePath();
            StartCoroutine(getFilePath_Coroutine);
#else
            if (!string.IsNullOrEmpty(classes))
            {
                classes_filepath = Utils.getFilePath("OpenCVForUnity/dnn/" + classes);
                if (string.IsNullOrEmpty(classes_filepath)) Debug.Log("The file:" + classes + " did not exist in the folder “Assets/StreamingAssets/OpenCVForUnity/dnn”.");
            }
            if (!string.IsNullOrEmpty(config))
            {
                config_filepath = Utils.getFilePath("OpenCVForUnity/dnn/" + config);
                if (string.IsNullOrEmpty(config_filepath)) Debug.Log("The file:" + config + " did not exist in the folder “Assets/StreamingAssets/OpenCVForUnity/dnn”.");
            }
            if (!string.IsNullOrEmpty(model))
            {
                model_filepath = Utils.getFilePath("OpenCVForUnity/dnn/" + model);
                if (string.IsNullOrEmpty(model_filepath)) Debug.Log("The file:" + model + " did not exist in the folder “Assets/StreamingAssets/OpenCVForUnity/dnn”.");
            }
            coordinateMapper = gameObject.AddComponent<CameraToWorldMapper>();
            coordinateMapper.unityCamera = Camera.main;

            trackedPerson = Instantiate(personPrefab);
            trackedPerson.SetActive(false);

            Run();
#endif
        }

#if UNITY_WEBGL
        private IEnumerator GetFilePath()
        {
            if (!string.IsNullOrEmpty(classes))
            {
                var getFilePathAsync_0_Coroutine = Utils.getFilePathAsync("OpenCVForUnity/dnn/" + classes, (result) =>
                {
                    classes_filepath = result;
                });
                yield return getFilePathAsync_0_Coroutine;

                if (string.IsNullOrEmpty(classes_filepath)) Debug.Log("The file:" + classes + " did not exist in the folder “Assets/StreamingAssets/OpenCVForUnity/dnn”.");
            }

            if (!string.IsNullOrEmpty(config))
            {
                var getFilePathAsync_1_Coroutine = Utils.getFilePathAsync("OpenCVForUnity/dnn/" + config, (result) =>
                {
                    config_filepath = result;
                });
                yield return getFilePathAsync_1_Coroutine;

                if (string.IsNullOrEmpty(config_filepath)) Debug.Log("The file:" + config + " did not exist in the folder “Assets/StreamingAssets/OpenCVForUnity/dnn”.");
            }

            if (!string.IsNullOrEmpty(model))
            {
                var getFilePathAsync_2_Coroutine = Utils.getFilePathAsync("OpenCVForUnity/dnn/" + model, (result) =>
                {
                    model_filepath = result;
                });
                yield return getFilePathAsync_2_Coroutine;

                if (string.IsNullOrEmpty(model_filepath)) Debug.Log("The file:" + model + " did not exist in the folder “Assets/StreamingAssets/OpenCVForUnity/dnn”.");
            }

            getFilePath_Coroutine = null;

            Run();
        }
#endif

        // Use this for initialization
        void Run()
        {
            //if true, The error log of the Native side OpenCV will be displayed on the Unity Editor Console.
            Utils.setDebugMode(true);


            if (string.IsNullOrEmpty(model_filepath))
            {
                Debug.LogError("model: " + model + " or " + "config: " + config + " or " + "classes: " + classes + " is not loaded.");
            }
            else
            {
                objectDetector = new YOLOv4ObjectDetector(model_filepath, config_filepath, classes_filepath, new Size(inpWidth, inpHeight), confThreshold, nmsThreshold/*, topK*/);
            }

            if (string.IsNullOrEmpty(testInputImage))
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                // Avoids the front camera low light issue that occurs in only some Android devices (e.g. Google Pixel, Pixel2).
                webCamTextureToMatHelper.avoidAndroidFrontCameraLowLightIssue = true;
#endif
                webCamTextureToMatHelper.Initialize();
            }
            else
            {
                /////////////////////
                // TEST

                var getFilePathAsync_0_Coroutine = Utils.getFilePathAsync("OpenCVForUnity/dnn/" + testInputImage, (result) =>
                {
                    string test_input_image_filepath = result;
                    if (string.IsNullOrEmpty(test_input_image_filepath)) Debug.Log("The file:" + testInputImage + " did not exist in the folder “Assets/StreamingAssets/OpenCVForUnity/dnn”.");

                    Mat img = Imgcodecs.imread(test_input_image_filepath);
                    if (img.empty())
                    {
                        img = new Mat(424, 640, CvType.CV_8UC3, new Scalar(0, 0, 0));
                        Imgproc.putText(img, testInputImage + " is not loaded.", new Point(5, img.rows() - 30), Imgproc.FONT_HERSHEY_SIMPLEX, 0.7, new Scalar(255, 255, 255, 255), 2, Imgproc.LINE_AA, false);
                        Imgproc.putText(img, "Please read console message.", new Point(5, img.rows() - 10), Imgproc.FONT_HERSHEY_SIMPLEX, 0.7, new Scalar(255, 255, 255, 255), 2, Imgproc.LINE_AA, false);
                    }
                    else
                    {
                        TickMeter tm = new TickMeter();
                        tm.start();

                        Mat results = objectDetector.infer(img);

                        tm.stop();
                        Debug.Log("YOLOv4ObjectDetector Inference time (preprocess + infer + postprocess), ms: " + tm.getTimeMilli());

                        objectDetector.visualize(img, results, true, false);
                    }

                    gameObject.transform.localScale = new Vector3(img.width(), img.height(), 1);
                    float imageWidth = img.width();
                    float imageHeight = img.height();
                    float widthScale = (float)Screen.width / imageWidth;
                    float heightScale = (float)Screen.height / imageHeight;
                    if (widthScale < heightScale)
                    {
                        Camera.main.orthographicSize = (imageWidth * (float)Screen.height / (float)Screen.width) / 2;
                    }
                    else
                    {
                        Camera.main.orthographicSize = imageHeight / 2;
                    }

                    Imgproc.cvtColor(img, img, Imgproc.COLOR_BGR2RGB);
                    Texture2D texture = new Texture2D(img.cols(), img.rows(), TextureFormat.RGB24, false);
                    Utils.matToTexture2D(img, texture);
                    gameObject.GetComponent<Renderer>().material.mainTexture = texture;

                });
                StartCoroutine(getFilePathAsync_0_Coroutine);

                /////////////////////
            }
        }

        /// <summary>
        /// Raises the webcam texture to mat helper initialized event.
        /// </summary>
        public void OnWebCamTextureToMatHelperInitialized()
        {
            Debug.Log("OnWebCamTextureToMatHelperInitialized");

            Mat webCamTextureMat = webCamTextureToMatHelper.GetMat();

            texture = new Texture2D(webCamTextureMat.cols(), webCamTextureMat.rows(), TextureFormat.RGBA32, false);
            Utils.matToTexture2D(webCamTextureMat, texture);

            gameObject.GetComponent<Renderer>().material.mainTexture = texture;

            gameObject.transform.localScale = new Vector3(webCamTextureMat.cols(), webCamTextureMat.rows(), 1);
            Debug.Log("Screen.width " + Screen.width + " Screen.height " + Screen.height + " Screen.orientation " + Screen.orientation);

            if (fpsMonitor != null)
            {
                fpsMonitor.Add("width", webCamTextureMat.width().ToString());
                fpsMonitor.Add("height", webCamTextureMat.height().ToString());
                fpsMonitor.Add("orientation", Screen.orientation.ToString());
            }


            float width = webCamTextureMat.width();
            float height = webCamTextureMat.height();

            float widthScale = (float)Screen.width / width;
            float heightScale = (float)Screen.height / height;
            if (widthScale < heightScale)
            {
                Camera.main.orthographicSize = (width * (float)Screen.height / (float)Screen.width) / 2;
            }
            else
            {
                Camera.main.orthographicSize = height / 2;
            }

            bgrMat = new Mat(webCamTextureMat.rows(), webCamTextureMat.cols(), CvType.CV_8UC3);
        }

        /// <summary>
        /// Raises the webcam texture to mat helper disposed event.
        /// </summary>
        public void OnWebCamTextureToMatHelperDisposed()
        {
            Debug.Log("OnWebCamTextureToMatHelperDisposed");

            if (bgrMat != null)
                bgrMat.Dispose();

            if (texture != null)
            {
                Texture2D.Destroy(texture);
                texture = null;
            }
        }

        /// <summary>
        /// Raises the webcam texture to mat helper error occurred event.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        public void OnWebCamTextureToMatHelperErrorOccurred(WebCamTextureToMatHelper.ErrorCode errorCode)
        {
            Debug.Log("OnWebCamTextureToMatHelperErrorOccurred " + errorCode);
        }

        // Update is called once per frame
        void Update()
        {

            if (webCamTextureToMatHelper.IsPlaying() && webCamTextureToMatHelper.DidUpdateThisFrame())
            {

                Mat rgbaMat = webCamTextureToMatHelper.GetMat();

                if (objectDetector == null)
                {
                    Imgproc.putText(rgbaMat, "model file is not loaded.", new Point(5, rgbaMat.rows() - 30), Imgproc.FONT_HERSHEY_SIMPLEX, 0.7, new Scalar(255, 255, 255, 255), 2, Imgproc.LINE_AA, false);
                    Imgproc.putText(rgbaMat, "Please read console message.", new Point(5, rgbaMat.rows() - 10), Imgproc.FONT_HERSHEY_SIMPLEX, 0.7, new Scalar(255, 255, 255, 255), 2, Imgproc.LINE_AA, false);
                }
                else
                {
                    Imgproc.cvtColor(rgbaMat, bgrMat, Imgproc.COLOR_RGBA2BGR);

                    //TickMeter tm = new TickMeter();
                    //tm.start();

                    Mat results = objectDetector.infer(bgrMat);

                    List<DetectedPerson> currentDetections = new List<DetectedPerson>();

                    if (results.rows() > 0)
                    {
                        for (int i = 0; i < results.rows(); i++)
                        {
                            float[] box = new float[4];
                            results.get(i, 0, box);
                            float[] cls = new float[1];
                            results.get(i, 5, cls);

                            if ((int)cls[0] == 0) // person class
                            {
                                float centerX = (box[0] + box[2]) / 2f;
                                float centerY = (box[1] + box[3]) / 2f;
                                float boxHeight = box[3] - box[1];

                                Vector3 worldPos = coordinateMapper.MapToWorld(
                                    new Vector2(centerX, centerY),
                                    boxHeight,
                                    rgbaMat.rows()
                                );

                                if (IsValidPosition(worldPos))
                                {
                                    currentDetections.Add(new DetectedPerson(worldPos));
                                    Debug.LogWarning(currentDetections.Count + " кол-во currentDetections");
                                }
                            }
                        }
                    }


                    // Обновляем треки
                    UpdateTracks(currentDetections);
                    CleanupInvisiblePersons();
                    //tm.stop();
                    //Debug.Log("YOLOv4ObjectDetector Inference time (preprocess + infer + postprocess), ms: " + tm.getTimeMilli());

                    Imgproc.cvtColor(bgrMat, rgbaMat, Imgproc.COLOR_BGR2RGBA);

                    objectDetector.visualize(rgbaMat, results, false, true);
                }

                Utils.matToTexture2D(rgbaMat, texture);
            }
        }

        private void UpdateTracks(List<DetectedPerson> currentDetections)
        {
            Debug.Log($"Updating tracks. Current detections count: {currentDetections.Count}");

            // 1. Помечаем все существующие треки как необнаруженные
            foreach (var pair in activePersons)
            {
                pair.Value.wasDetected = false;
            }

            // 2. Для каждого обнаружения находим ближайший существующий трек
            foreach (var detection in currentDetections)
            {
                int closestTrackId = -1;
                float minDistance = float.MaxValue;
                float maxAllowedDistance = 3.0f; // Максимальное расстояние для сопоставления

                foreach (var pair in activePersons)
                {
                    if (pair.Value.wasDetected) continue;

                    float distance = Vector3.Distance(
                        detection.position,
                        pair.Value.filter.GetCurrentPosition()
                    );
                    Debug.Log(distance + " дистанция");

                    if (distance < minDistance && distance < maxAllowedDistance)
                    {
                        minDistance = distance;
                        closestTrackId = pair.Key;
                        Debug.Log(closestTrackId + " closeTrackId");
                    }
                }

                // 3. Если нашли подходящий трек - обновляем, иначе создаем новый
                if (closestTrackId != -1)
                {
                    activePersons[closestTrackId].UpdatePosition(detection.position);
                    activePersons[closestTrackId].wasDetected = true;
                    Debug.Log($"Updated track ID: {closestTrackId}");
                }
                else
                {
                    int newId = nextId++;
                    activePersons[newId] = new TrackedPerson(
                        personPrefab,
                        newId,
                        kalmanSmoothness,
                        detection.position
                    );
                    Debug.Log($"Created new track ID: {newId}");
                }
            }

            // 4. Удаляем треки, которые не были обнаружены
            CleanupInvisiblePersons();
        }

        private void CleanupInvisiblePersons()
        {
            List<int> tracksToRemove = new List<int>();
            float currentTime = Time.time;

            foreach (var pair in activePersons)
            {
                // Если трек не был обнаружен в текущем кадре
                if (!pair.Value.wasDetected)
                {
                    // Если таймер еще не установлен - устанавливаем текущее время
                    if (!invisibleTimers.ContainsKey(pair.Key))
                    {
                        invisibleTimers[pair.Key] = currentTime;
                    }
                    // Если трек не появлялся дольше maxInvisibleTime - помечаем на удаление
                    else if (currentTime - invisibleTimers[pair.Key] > maxInvisibleTime)
                    {
                        tracksToRemove.Add(pair.Key);
                    }
                }
                else
                {
                    // Если трек обнаружен - сбрасываем таймер
                    if (invisibleTimers.ContainsKey(pair.Key))
                    {
                        invisibleTimers.Remove(pair.Key);
                    }
                }
            }

            // Удаляем помеченные треки
            foreach (int trackId in tracksToRemove)
            {
                if (activePersons.TryGetValue(trackId, out TrackedPerson person))
                {
                    // Уничтожаем GameObject
                    if (person.obj != null)
                    {
                        UnityEngine.Object.Destroy(person.obj);
                    }

                    // Удаляем из словарей
                    activePersons.Remove(trackId);
                    invisibleTimers.Remove(trackId);

                    Debug.Log($"Removed track ID: {trackId}");
                }
            }
        }



        private class DetectedPerson
        {
            public Vector3 position;

            public DetectedPerson(Vector3 position)
            {
                this.position = position;
            }
        }


        private bool IsValidPosition(Vector3 pos)
        {
            return !float.IsNaN(pos.x) && !float.IsInfinity(pos.x) &&
                   !float.IsNaN(pos.z) && !float.IsInfinity(pos.z);
        }




        /// <summary>
        /// Raises the destroy event.
        /// </summary>
        void OnDestroy()
        {
            webCamTextureToMatHelper.Dispose();

            if (objectDetector != null)
                objectDetector.dispose();

            Utils.setDebugMode(false);

#if UNITY_WEBGL
            if (getFilePath_Coroutine != null)
            {
                StopCoroutine(getFilePath_Coroutine);
                ((IDisposable)getFilePath_Coroutine).Dispose();
            }
#endif
        }

        /// <summary>
        /// Raises the back button click event.
        /// </summary>
        public void OnBackButtonClick()
        {
            SceneManager.LoadScene("OpenCVForUnityExample");
        }

        /// <summary>
        /// Raises the play button click event.
        /// </summary>
        public void OnPlayButtonClick()
        {
            webCamTextureToMatHelper.Play();
        }

        /// <summary>
        /// Raises the pause button click event.
        /// </summary>
        public void OnPauseButtonClick()
        {
            webCamTextureToMatHelper.Pause();
        }

        /// <summary>
        /// Raises the stop button click event.
        /// </summary>
        public void OnStopButtonClick()
        {
            webCamTextureToMatHelper.Stop();
        }

        /// <summary>
        /// Raises the change camera button click event.
        /// </summary>
        public void OnChangeCameraButtonClick()
        {
            webCamTextureToMatHelper.requestedIsFrontFacing = !webCamTextureToMatHelper.requestedIsFrontFacing;
        }
    }
}

#endif
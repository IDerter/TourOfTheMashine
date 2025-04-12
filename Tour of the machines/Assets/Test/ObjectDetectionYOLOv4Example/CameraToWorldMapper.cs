using UnityEngine;
using OpenCVForUnity.CoreModule;

public class CameraToWorldMapper : MonoBehaviour
{
    public Camera unityCamera; // Основная камера Unity
    public float floorY = 0;   // Y-координата "пола" в Unity
    public float objectHeight = 1.8f; // Предполагаемая высота человека (в метрах)

    public Vector3 MapToWorld(Vector2 screenPoint)
    {
        // Нормализованные координаты (0-1) с инверсией Y
        Vector2 normalizedPoint = new Vector2(
            screenPoint.x / Screen.width,
            1f - (screenPoint.y / Screen.height)
        );

        // Преобразование в мировые координаты
        Ray ray = unityCamera.ViewportPointToRay(new Vector3(normalizedPoint.x, normalizedPoint.y, 0));
        float distance = (floorY - ray.origin.y) / ray.direction.y;
        Vector3 worldPos = ray.GetPoint(distance);

        // Корректировка высоты
        worldPos.y = floorY + objectHeight / 2f;

        return worldPos;
    }
}
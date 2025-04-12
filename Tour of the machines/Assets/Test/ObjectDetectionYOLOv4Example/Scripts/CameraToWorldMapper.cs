using UnityEngine;

public class CameraToWorldMapper : MonoBehaviour
{
    public Camera unityCamera;
    public float floorY = 0f;
    public float referenceHeight = 1.8f;
    public float minDistance = 0.5f;
    public float maxDistance = 20f;
    public float fovCompensation = 1.2f;

    public Vector3 MapToWorld(Vector2 imagePoint, float boundingBoxHeight, float imageHeight)
    {
        // Расстояние на основе относительного размера объекта
        float distance = (referenceHeight * imageHeight) / (boundingBoxHeight * fovCompensation);
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        // Преобразование координат
        Vector3 screenPos = new Vector3(
            imagePoint.x,
            Screen.height - imagePoint.y,
            distance
        );

        Vector3 worldPos = unityCamera.ScreenToWorldPoint(screenPos);
        worldPos.y = floorY + referenceHeight / 2f;

        return worldPos;
    }
}
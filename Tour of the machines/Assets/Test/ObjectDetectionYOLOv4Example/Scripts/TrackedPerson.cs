using UnityEngine;

public class TrackedPerson
{
    public GameObject obj;
    public KalmanFilter3D filter;
    public int id;
    public Color color;
    public bool wasDetected = true;

    public TrackedPerson(GameObject prefab, int id, float smoothness, Vector3 initialPosition)
    {
        this.id = id;
        this.obj = Object.Instantiate(prefab, initialPosition, Quaternion.identity);
        this.filter = new KalmanFilter3D(smoothness);
        this.color = Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.8f, 1f);

        var renderer = obj.GetComponent<Renderer>();
        if (renderer != null) renderer.material.color = color;

        var text = obj.GetComponentInChildren<TextMesh>();
        if (text != null) text.text = $"ID: {id}";
    }

    public void UpdatePosition(Vector3 newPosition)
    {
        Vector3 filteredPos = filter.Update(newPosition);
        obj.transform.position = filteredPos;
    }
}
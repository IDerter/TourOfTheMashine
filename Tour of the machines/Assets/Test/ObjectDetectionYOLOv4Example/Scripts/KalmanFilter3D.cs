using UnityEngine;

public class KalmanFilter3D
{
    private KalmanFilter[] filters;
    private Vector3 currentPosition;

    public KalmanFilter3D(float smoothness = 0.1f)
    {
        filters = new KalmanFilter[3];
        for (int i = 0; i < 3; i++)
        {
            filters[i] = new KalmanFilter();
            filters[i].Setup(smoothness);
        }
        currentPosition = Vector3.zero;
    }

    public Vector3 Update(Vector3 position)
    {
        currentPosition = new Vector3(
            filters[0].Update(position.x),
            filters[1].Update(position.y),
            filters[2].Update(position.z)
        );
        return currentPosition;
    }

    public Vector3 GetCurrentPosition()
    {
        return currentPosition;
    }

    private class KalmanFilter
    {
        private float Q = 0.0001f; // Process noise
        private float R = 0.01f;   // Measurement noise
        private float P = 1.0f;    // Estimation error
        private float X = 0f;      // Value
        private float K;           // Kalman gain

        public void Setup(float smoothness)
        {
            Q = smoothness;
            R = smoothness * 10f;
        }

        public float Update(float measurement)
        {
            // Prediction phase
            P = P + Q;

            // Measurement update
            K = P / (P + R);
            X = X + K * (measurement - X);
            P = (1 - K) * P;

            return X;
        }

        public float GetCurrentValue()
        {
            return X;
        }
    }
}
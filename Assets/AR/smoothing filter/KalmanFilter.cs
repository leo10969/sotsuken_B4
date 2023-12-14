using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MatrixExtensions
{
    public static Matrix4x4 Multiply(this Matrix4x4 matrix, float scalar)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                matrix[i, j] *= scalar;
            }
        }
        return matrix;
    }

    public static Matrix4x4 Add(this Matrix4x4 matrix, float scalar)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                matrix[i, j] += scalar;
            }
        }
        return matrix;
    }
}

public class KalmanFilter
{
    private Vector3 _lastEstimate = new Vector3(-0.1f, -0.20f, 0.02f);
    private float _processNoise; // プロセスノイズ
    private float _measurementNoise; // 観測ノイズ
    private Matrix4x4 _errorCovariance; // 誤差共分散行列

    public KalmanFilter()
    {
        _errorCovariance = Matrix4x4.identity;
    }

    public void SetKalmanParameter(float noise, float process)
    {
        _measurementNoise = noise;
        _processNoise = process;
    }

    public Vector3 UpdateEstimate(Vector3 measurement)
    {
        // 予測
        Vector3 prediction = _lastEstimate;
        Matrix4x4 predictionErrorCovariance = MatrixExtensions.Add(_errorCovariance, _processNoise);

        // 更新
        float kalmanGain = predictionErrorCovariance.m00 / (predictionErrorCovariance.m00 + _measurementNoise);
        _lastEstimate = prediction + kalmanGain * (measurement - prediction);
        _errorCovariance = MatrixExtensions.Multiply(predictionErrorCovariance, (1 - kalmanGain));

        return _lastEstimate;
    }
}

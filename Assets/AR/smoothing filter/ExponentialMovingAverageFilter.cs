using UnityEngine;

public class ExponentialMovingAverageFilter
{
    private Vector3 _lastEMA;
    private float _alpha;

    public void SetEMAParameter(float alpha)
    {
        _alpha = alpha;
    }
    public ExponentialMovingAverageFilter(float alpha = 0.5f)
    {
        _alpha = alpha;
        _lastEMA = new Vector3(-0.1f, -0.20f, 0.02f);
    }

    public Vector3 UpdateEstimate(Vector3 measurement)
    {
        _lastEMA = (1 - _alpha) * _lastEMA + _alpha * measurement;
        // Debug.Log("lastEMA: " + _lastEMA);
        return _lastEMA;
    }
}


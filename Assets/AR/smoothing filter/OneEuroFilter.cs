using UnityEngine;

public class OneEuroFilter
{
    private Vector3 _lastValue;
    private Vector3 _lastVelocity;
    private float _beta;
    private float _fc_min;

    public void SetOneEuroParameters(float beta, float fc_min)
    {
        _beta = beta;
        _fc_min = fc_min;
    }

    public OneEuroFilter(float beta, float fc_min)
    {
        _beta = beta;
        _fc_min = fc_min;
        _lastValue = new Vector3(-0.1f, -0.5f, 0.02f);
        _lastVelocity = Vector3.zero;
    }

    public Vector3 UpdateEstimate(Vector3 measurement)
    {
        Vector3 velocity = (measurement - _lastValue) / Time.deltaTime;
        float magnitude = velocity.magnitude;

        float fc = _fc_min + _beta * magnitude;
        float alpha = 1.0f / (1.0f + (1.0f / (2.0f * Mathf.PI * fc)));

        Vector3 filteredValue = alpha * measurement + (1.0f - alpha) * _lastValue;

        _lastValue = filteredValue;
        _lastVelocity = velocity;

        return filteredValue;
    }
}

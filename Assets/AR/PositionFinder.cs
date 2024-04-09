using UnityEngine;

public class PositionFinder : MonoBehaviour
{
    // 座標を外部から参照できるようにプロパティを用意
    public Vector3 QTopLeft { get; private set; }
    public Vector3 PRightEdge { get; private set; }
    public Vector3 MBottomEdge { get; private set; }

    public void QPMPositionFinder()
    {
        // "Q"タグのオブジェクトの左上の座標を取得し、プロパティに格納
        GameObject qObject = GameObject.FindWithTag("Q");
        if (qObject != null)
        {
            QTopLeft = GetTopLeft(qObject);
            Debug.Log("Q Object Top Left: " + QTopLeft);
        }

        // "P"タグのオブジェクトの右端の座標を取得し、プロパティに格納
        GameObject pObject = GameObject.FindWithTag("P");
        if (pObject != null)
        {
            PRightEdge = GetRightEdge(pObject);
            Debug.Log("P Object Right Edge: " + PRightEdge);
        }

        // "M"タグのオブジェクトの下端の座標を取得し、プロパティに格納
        GameObject mObject = GameObject.FindWithTag("M");
        if (mObject != null)
        {
            MBottomEdge = GetBottomEdge(mObject);
            Debug.Log("M Object Bottom Edge: " + MBottomEdge);
        }
    }

    Vector3 GetTopLeft(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        return renderer.bounds.min + new Vector3(0, renderer.bounds.size.y, 0);
    }

    Vector3 GetRightEdge(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        return renderer.bounds.max;
    }

    Vector3 GetBottomEdge(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        return renderer.bounds.min;
    }
}

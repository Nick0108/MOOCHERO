using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

public class SplineReader : MonoBehaviour {

    public GameObject trailEditorCanvas;            //宝物车轨迹绘制提示画布
    public SplineTrailRenderer trailReference;      //保护车轨迹计算和保存
    private Vector3[] knotsPosition;                
    private Vector3 offset = new Vector3(0.0f, 0.1f, 0.0f);

    void Start () {
        trailEditorCanvas.SetActive(false);
        ReadKnotsData();
        trailReference.spline.knots.Clear();
        trailReference.emit = false;
        //第一个点和最后一个点省去，防止退回的现象：第一个点和最后一个点非严格定义的控制点。
        for (int i = 1; i < knotsPosition.Length-1; i++)
        {
            trailReference.AddKnots(knotsPosition[i]);
        }
        transform.position = transform.position + offset;
    }

    //读取曲线的节点信息
    void ReadKnotsData()
    {
        //存到字节数组里 
        string trail = Resources.Load("Trail").ToString();
        string[] arr = trail.Split('\n');
        string line;
        line = arr[0];
        int length = int.Parse(line);
        knotsPosition = new Vector3[length];
        for (int i = 0; i < length; i++)
        {
            line = arr[i+1];
            knotsPosition[i] = StringToVector3(line);
        }
        //sr.Close();
    }

    Vector3 StringToVector3(string s)
    {
        s = s.Substring(1, s.Length - 3);//2个括号加一个分隔符
        char[] delimiterChars = {','};
        string[] numbers = s.Split(delimiterChars);
        return new Vector3(float.Parse(numbers[0]), float.Parse(numbers[1]), float.Parse(numbers[2]));
    }
}

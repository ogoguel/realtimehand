using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTHand;

public class Line
{
    public LineRenderer lineRenderer;
    public JointName[] joints;
}

public class RealtimeHandSample : MonoBehaviour
{
    [SerializeField]
    public GameObject jointPrefab;
    [SerializeField]
    public GameObject linePrefab;
    [SerializeField]
    public RealtimeHandManager handManager;

    Dictionary<JointName, GameObject> joints;
    List<Line> lines;
    GameObject root;

    protected void Awake()
    {
        joints = new Dictionary<JointName, GameObject>();
        root = new GameObject("hand");
        root.transform.parent = gameObject.transform;
        foreach (JointName joint in Enum.GetValues(typeof(JointName)))
        {
            GameObject go = GameObject.Instantiate(jointPrefab);
            go.name = joint.ToString();
            go.transform.SetParent(root.transform);
            joints.Add(joint, go);
            go.SetActive(true);
        }
        jointPrefab.SetActive(false);
        linePrefab.SetActive(false);
        root.SetActive(false);

        lines = new List<Line>();
        AddLine(JointName.wrist, JointName.indexMCP, JointName.indexPIP, JointName.indexDIP, JointName.indexTip);
        AddLine(JointName.wrist, JointName.littleMCP, JointName.littlePIP, JointName.littleDIP, JointName.littleTip);
        AddLine(JointName.wrist, JointName.middleMCP, JointName.middlePIP, JointName.middleDIP, JointName.middleTip);
        AddLine(JointName.wrist, JointName.ringMCP, JointName.ringPIP, JointName.ringDIP, JointName.ringTip);
        AddLine(JointName.wrist, JointName.thumbCMC, JointName.thumbMP, JointName.thumbIP, JointName.thumbTip);
    }

    protected void Start()
    {
        handManager.HandUpdated += OnHandUpdated;
    }

    protected void OnDestroy()
    {
        if (handManager != null)
        {
            handManager.HandUpdated -= OnHandUpdated;
        }
    }

    void AddLine(params JointName[] _joints)
    {
        Line line = new Line();
        GameObject lineRendererGO = GameObject.Instantiate(linePrefab);
        lineRendererGO.name = "line_" + _joints[_joints.Length - 1];
        lineRendererGO.transform.SetParent(root.transform);
        lineRendererGO.SetActive(true);
        line.lineRenderer = lineRendererGO.GetComponent<LineRenderer>();
        line.lineRenderer.positionCount = _joints.Length;
        line.lineRenderer.startWidth = 0.001f;
        line.lineRenderer.endWidth = 0.001f;
        line.joints = _joints;
        lines.Add(line);
    }

    private void OnHandUpdated(RealtimeHand _realtimeHand)
    {
        root.SetActive(_realtimeHand.IsVisible);
        if (!_realtimeHand.IsVisible)
        {
            return;
        }

        var handJoints = _realtimeHand.Joints;
        foreach (var kv in joints)
        {
            kv.Value.gameObject.transform.position = handJoints[kv.Key].worldPos;
        }

        foreach (var line in lines)
        {
            for (int i = 0; i < line.joints.Length; i++)
            {
                line.lineRenderer.SetPosition(i, joints[line.joints[i]].transform.position);
            }
        }
    }

}

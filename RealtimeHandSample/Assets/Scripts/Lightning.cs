
using UnityEngine;
using RTHand;

public class Lightning : MonoBehaviour
{
    [SerializeField]
    public LineRenderer lineRenderer;
  
    [SerializeField]
    public RealtimeHandManager handManager;

  
    protected void Start()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        handManager.HandUpdated += OnHandUpdated;
    }

    protected void OnDestroy()
    {
        if (handManager != null)
        {
            handManager.HandUpdated -= OnHandUpdated;
        }
    }

    private void OnHandUpdated(RealtimeHand _realtimeHand)
    {
        lineRenderer.enabled = _realtimeHand.IsVisible;

        if (_realtimeHand.IsVisible)
        {
            var thumbWorldPos = _realtimeHand.Joints[JointName.thumbTip].worldPos;
            var indexWorldPos = _realtimeHand.Joints[JointName.indexTip].worldPos;
            lineRenderer.SetPosition(0, thumbWorldPos);
            lineRenderer.SetPosition(1, indexWorldPos);
        }
    }

}

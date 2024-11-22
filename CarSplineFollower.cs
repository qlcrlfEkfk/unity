using UnityEngine;

public class CarSplineFollower : MonoBehaviour
{
    public Transform[] splinePoints; // ���ö��� ����
    public float speed = 5f;         // �ڵ��� �ӵ�
    private float t = 0f;            // ��� �󿡼� �ڵ����� ��ġ

    private void Update()
    {
        if (splinePoints == null || splinePoints.Length < 4)
        {
            UnityEngine.Debug.LogError("���ö��� ���� �ּ� 4�� �̻� �ʿ��մϴ�."); // UnityEngine.Debug ��������� ���
            return;
        }

        // t ���� �ð��� ���� ������Ű�� ��θ� ���� �̵�
        t += speed * Time.deltaTime / splinePoints.Length;
        t = Mathf.Repeat(t, 1f); // t ���� 0~1 ���̸� �ݺ�

        // Hermite ���ö��� ������� ��ġ �� ���� ���
        Vector3 targetPosition = GetPositionOnSpline(t);
        Vector3 direction = GetDirectionOnSpline(t);

        // �ڵ��� ��ġ �� ȸ�� ������Ʈ
        transform.position = targetPosition;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    // Hermite Spline ��� ��� ���� ��ġ ���
    private Vector3 GetPositionOnSpline(float t)
    {
        int numPoints = splinePoints.Length;
        int p0Index = Mathf.FloorToInt(t * numPoints) % numPoints;
        int p1Index = (p0Index + 1) % numPoints;
        int p2Index = (p0Index + 2) % numPoints;
        int p3Index = (p0Index + 3) % numPoints;

        float localT = (t * numPoints) - p0Index;

        return GetHermiteInternal(
            splinePoints[p0Index].position,
            splinePoints[p1Index].position,
            splinePoints[p2Index].position,
            splinePoints[p3Index].position,
            localT
        );
    }

    // Hermite Spline ��� ��� ���� ���� ���
    private Vector3 GetDirectionOnSpline(float t)
    {
        float delta = 0.001f; // ���� ���� ����Ͽ� ���� ���� ���
        Vector3 position1 = GetPositionOnSpline(t);
        Vector3 position2 = GetPositionOnSpline(Mathf.Repeat(t + delta, 1f));

        return (position2 - position1).normalized;
    }

    // Hermite ���ö��� ��� �޼���
    private Vector3 GetHermiteInternal(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        float h00 = 2f * t3 - 3f * t2 + 1f; // Hermite basis function h00
        float h10 = t3 - 2f * t2 + t;      // Hermite basis function h10
        float h01 = -2f * t3 + 3f * t2;    // Hermite basis function h01
        float h11 = t3 - t2;               // Hermite basis function h11

        Vector3 m0 = 0.5f * (p2 - p0);     // Tangent at p1
        Vector3 m1 = 0.5f * (p3 - p1);     // Tangent at p2

        return h00 * p1 + h10 * m0 + h01 * p2 + h11 * m1;
    }
}
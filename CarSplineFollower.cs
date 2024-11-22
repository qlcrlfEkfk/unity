using UnityEngine;

public class CarSplineFollower : MonoBehaviour
{
    public Transform[] splinePoints; // 스플라인 점들
    public float speed = 5f;         // 자동차 속도
    private float t = 0f;            // 경로 상에서 자동차의 위치

    private void Update()
    {
        if (splinePoints == null || splinePoints.Length < 4)
        {
            UnityEngine.Debug.LogError("스플라인 점은 최소 4개 이상 필요합니다."); // UnityEngine.Debug 명시적으로 사용
            return;
        }

        // t 값을 시간에 따라 증가시키며 경로를 따라 이동
        t += speed * Time.deltaTime / splinePoints.Length;
        t = Mathf.Repeat(t, 1f); // t 값이 0~1 사이를 반복

        // Hermite 스플라인 기반으로 위치 및 방향 계산
        Vector3 targetPosition = GetPositionOnSpline(t);
        Vector3 direction = GetDirectionOnSpline(t);

        // 자동차 위치 및 회전 업데이트
        transform.position = targetPosition;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    // Hermite Spline 기반 경로 상의 위치 계산
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

    // Hermite Spline 기반 경로 상의 방향 계산
    private Vector3 GetDirectionOnSpline(float t)
    {
        float delta = 0.001f; // 작은 값을 사용하여 방향 벡터 계산
        Vector3 position1 = GetPositionOnSpline(t);
        Vector3 position2 = GetPositionOnSpline(Mathf.Repeat(t + delta, 1f));

        return (position2 - position1).normalized;
    }

    // Hermite 스플라인 계산 메서드
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
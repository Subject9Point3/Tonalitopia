using UnityEngine;

public class CameraPositionAngleCalculator : MonoBehaviour
{
    public bool lockHeight, lockLength, lockAngle;
    public float height, length, angle;
    private float previousHeight, previousLength, previousAngle;

    private void OnValidate()
    {
        if (height != previousHeight)
        {
            previousHeight = height;
            
            if (lockAngle && lockLength) return;
            if (lockAngle) SetSide(ref length, height, HypotenuseOppositeAngle(height, angle));
            if (lockLength) SetAngle();
        }

        if (length != previousLength)
        {
            previousLength = length;
            
            if (lockAngle && lockHeight) return;
            if (lockHeight) SetAngle();
            if (lockAngle) SetSide(ref height, length, HypotenuseAdjacentAngle(length, angle));
        }

        if (angle != previousAngle)
        {
            previousAngle = angle;
            
            if (lockHeight && lockLength) return;
            if (lockHeight) SetSide(ref length, height, HypotenuseOppositeAngle(height, angle));
            if (lockLength) SetSide(ref height, length, HypotenuseAdjacentAngle(length, angle));
        }
    }

    private void SetAngle()
    {
        angle = Mathf.Asin(height / HypotenuseOppositeAdjacent(height, length)) * Mathf.Rad2Deg;
    }

    private void SetSide(ref float side, float known, float hypotenuse)
    {
        side = Mathf.Sqrt(Mathf.Pow(hypotenuse, 2) - Mathf.Pow(known, 2));
    }

    private float HypotenuseOppositeAdjacent(float a, float b) => Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(b, 2));
    private float HypotenuseOppositeAngle(float opposite, float theta) => opposite / Mathf.Sin(Mathf.Deg2Rad * theta);
    private float HypotenuseAdjacentAngle(float adjacent, float theta) => adjacent / Mathf.Cos(Mathf.Deg2Rad * theta);
}
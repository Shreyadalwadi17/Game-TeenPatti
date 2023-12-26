using UnityEngine;

public class ArrangeChildren : MonoBehaviour
{
    [Header("General")]
    public Transform targetTransform;
    public float spacing = 0.2f;
    public ArrangementMode mode;

    [Header("Oval")]
    public float xRadius = 2.0f;
    public float yRadius = 1.0f;
    public float angleOffset = 0.0f;
    public float tiltAngle = 45.0f;


    public enum ArrangementMode
    {
        None,
        Line,
        Arc,
        Oval
    }

    void Start()
    {
        ArrangePlayers();
    }

    void ArrangePlayers()
    {
        switch (mode)
        {
            case ArrangementMode.Line:
                ArrangeChildrenInLine();
                break;
            case ArrangementMode.Arc:
                ArrangeChildrenInArc();
                break;
            case ArrangementMode.Oval:
                ArrangeChildrenInOval();
                break;
        }

        FaceTargetObject();
    }


    private void ArrangeChildrenInOval()
    {
        int childCount = transform.childCount;
        float angleStep = 360.0f / childCount;
        float angle = 0.0f;
        float x, y;

        for (int i = 0; i < childCount; i++)
        {
            angle = angleOffset + i * angleStep;
            x = Mathf.Cos(angle * Mathf.Deg2Rad) * xRadius;
            y = Mathf.Sin(angle * Mathf.Deg2Rad) * yRadius;
            transform.GetChild(i).localPosition = new Vector3(x, y, 0.0f);
        }
    }





    [Header("Arc")]
    public float radius = 1f; // The radius of the arc
    public float startAngle = -90f; // The starting angle of the arc, in degrees
    public float endAngle = 90f; // The ending angle of the arc, in degrees
    public bool clockwise = true; // Whether to arrange the children clockwise or counterclockwise


    private void ArrangeChildrenInArc()
    {


        int numChildren = transform.childCount;

        if (numChildren == 0) // If there are no children, do nothing
            return;

        float angleRange = endAngle - startAngle; // The range of angles for the arc
        float angleIncrement = angleRange / (numChildren - 1); // The increment between each child's angle
        Quaternion rotation = Quaternion.Euler(0f, 0f, clockwise ? -startAngle : -endAngle); // The rotation for the children

        for (int i = 0; i < numChildren; i++)
        {
            Transform child = transform.GetChild(i);
            float angle = startAngle + i * angleIncrement; // The angle for the child's position
            float x = radius * Mathf.Cos(Mathf.Deg2Rad * angle); // The x position for the child's position
            float y = radius * Mathf.Sin(Mathf.Deg2Rad * angle); // The y position for the child's position
            Vector3 localPosition = new Vector3(x, y, 0f);
            child.localPosition = localPosition;
            child.localRotation = rotation;
        }
    }

    private void ArrangeChildrenInLine()
    {

        int numChildren = transform.childCount;

        if (numChildren == 0) // If there are no children, do nothing
            return;

        for (int i = 0; i < numChildren; i++)
        {
            Transform child = transform.GetChild(i);
            float offset = i * spacing; // The offset from the first child's position
            Vector3 positionOffset = targetTransform.right * offset; // The position offset for the child
            Vector3 localPosition = positionOffset;
            child.localPosition = localPosition;

        }
    }


    private void FaceTargetObject()
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Vector3 direction = targetTransform.position - transform.GetChild(i).position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.GetChild(i).rotation = Quaternion.Euler(0.0f, 0.0f, angle - 90.0f);
        }
    }


#if UNITY_EDITOR
    void OnValidate()
    {
        ArrangePlayers();
    }
#endif
}
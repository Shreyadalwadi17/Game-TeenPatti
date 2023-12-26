using UnityEngine;

public class RoundTableArrangement : MonoBehaviour
{
    public float arcRadius = 2f;
    public float arcAngle = 180f;



    void Start()
    {
        //ArrangePlayers();
    }

    void ArrangePlayers()
    {
        int numSprites = transform.childCount;
        Vector2 centerPos = Vector2.zero;



        for (int i = 0; i < numSprites; i++)
        {
            float angle = (i / (float)(numSprites - 1)) * arcAngle - (arcAngle / 2f);
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * arcRadius;
            float y = Mathf.Sin(angle * Mathf.Deg2Rad) * arcRadius;
            Vector2 spritePos = new Vector2(x, y);

            Transform spriteTransform = transform.GetChild(i);
            spriteTransform.localPosition = spritePos;

            Vector2 dir = centerPos - spritePos;
            float angleToRotate = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            spriteTransform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, angleToRotate));
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (isActiveAndEnabled)
        {
            ArrangePlayers();
        }
    }
#endif
}

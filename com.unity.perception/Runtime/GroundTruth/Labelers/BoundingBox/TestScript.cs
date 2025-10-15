using UnityEngine;

public class TestScript : MonoBehaviour
{
    Texture m_BoundingBoxTexture;
    Texture m_LabelTexture;
    GUIStyle m_Style;

    Vector2 m_OriginalScreenSize = Vector2.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_OriginalScreenSize = new Vector2(Screen.width, Screen.height);

        m_BoundingBoxTexture = Resources.Load<Texture>("outline_box");
        m_LabelTexture = Resources.Load<Texture>("solid_white");

        m_Style = new GUIStyle();
        m_Style.normal.textColor = Color.black;
        m_Style.fontSize = 16;
        m_Style.padding = new RectOffset(4, 4, 4, 4);
        m_Style.contentOffset = new Vector2(4, 0);
        m_Style.alignment = TextAnchor.MiddleLeft;
    }

    // Update is called once per frame
    void OnGUI()
    {
        Color redColor = new Color(1f, 0f, 0f);
        GUI.DrawTexture(new Rect(30, 30, 90, 90), m_BoundingBoxTexture, ScaleMode.StretchToFill, true, 0, redColor, 3, 0.25f);
        GUI.DrawTexture(new Rect(10, 10, 70, 70), m_LabelTexture, ScaleMode.StretchToFill, true, 0, redColor, 0, 0);
    }
}

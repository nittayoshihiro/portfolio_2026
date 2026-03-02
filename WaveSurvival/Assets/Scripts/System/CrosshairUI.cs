using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    Image _img;

    void Start()
    {
        _img = GetComponent<Image>();
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;

        bool inside =
            mousePos.x >= 0 &&
            mousePos.y >= 0 &&
            mousePos.x <= Screen.width &&
            mousePos.y <= Screen.height;

        if (inside)
        {
            // 画面内
            transform.position = mousePos;
            _img.enabled = true;      // 表示
            Cursor.visible = false;  // OSカーソル非表示

            if (Input.GetMouseButton(0))
            {
                _img.color = Color.red;
            }
            else
            {
                _img.color = Color.white;
            }
        }
        else
        {
            // 画面外
            _img.enabled = false;     // 非表示（SetActiveは使わない）
            Cursor.visible = true;   // OSカーソル表示
        }
    }
}

using TMPro;
using UnityEngine;
[RequireComponent(typeof(TMP_Text))]
public class FloatingText : MonoBehaviour
{
    TMP_Text txt;
    // Start is called before the first frame update
    void Awake()
    {
        Destroy(this.gameObject, 0.5f);
        txt = GetComponent<TMP_Text>();

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, 0.1f * Time.deltaTime, 0, Space.Self);
    }
    public void SetText(string t, Color c)
    {

        txt.text = t;
        txt.color = c;

    }
}

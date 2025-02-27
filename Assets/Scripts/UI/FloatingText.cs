using System;
using System.Threading;
using TMPro;
using UnityEngine;
[RequireComponent(typeof(TMP_Text))]
public class FloatingText : MonoBehaviour
{
    TMP_Text txt;
    float timer = 0;
    // Start is called before the first frame update
    void Awake()
    {
        Destroy(this.gameObject, 0.5f);
        txt = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(-0.1f * Time.deltaTime, Mathf.Sin(timer * 3 * (float) Math.PI) * 0.1f * Time.deltaTime, 0, Space.Self);
        timer += Time.deltaTime;
    }
    public void SetText(string t, Color c)
    {

        txt.text = t;
        txt.color = c;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    private float disappearSpeed = 5f;
    private float disappearTimer = 0.5f;
    private float ySpeed = 1.25f;
    private TextMeshPro textMesh;
    private Color textColor;

    public static DamageText Create(Vector3 position, int damage, bool friendly)
    {
        GameObject damageTextObject = Instantiate(Map.Instance.DamageTextPF, position + new Vector3(0, 0.8f, 0), Quaternion.identity);
        DamageText damageText = damageTextObject.GetComponent<DamageText>();
        damageText.Setup(damage, friendly);
        return damageText;
        
    }

    public void Setup(int damage, bool friendly)
    {
        textMesh.SetText(damage.ToString());
        if (!friendly)
            textColor = textMesh.color;
        else
        {
            textColor = Color.red;
            textMesh.color = textColor;

        }
    } 

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, ySpeed) * Time.deltaTime;
        disappearTimer -= Time.deltaTime;
        if (disappearTimer <= 0)
        {
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a <= 0) Destroy(gameObject);

        }
    }
}

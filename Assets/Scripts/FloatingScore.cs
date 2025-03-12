using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using TMPro;

public class FloatingScore : MonoBehaviour
{
    public float moveSpeed = 50f;
    public float fadeDuration = 1f;
    private TextMeshProUGUI scoreText;
    private Color initialColor;


    IEnumerator FadeOutAndMove()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(initialColor.a, 0, elapsed / fadeDuration);
            scoreText.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
            yield return null;
        }
       
        Destroy(gameObject);
    }

    public void Initialize(int score, Color color)
    {
        scoreText = GetComponent<TextMeshProUGUI>();
        scoreText.text = score.ToString();
        initialColor = color;
        StartCoroutine(FadeOutAndMove());
        StartCoroutine(UpdateScore(score));
    }

    IEnumerator UpdateScore(int score)
    {
        yield return new WaitForSeconds(.5f);
        GameManager.Instance.AddScore(score);
    }

}

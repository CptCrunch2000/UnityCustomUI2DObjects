using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FancyTypeWriterController : MonoBehaviour
{
    public TMP_Text TextComponent;

    [Header("Text Settings")]
    public AudioClip TypeWriterAudio;
    public AudioClip TypeWriterSpaceAudio;
    public float WaitForNextLetter = 1f;
    public float WaitForNextText = 5f;
    public TextAlignmentOptions TextAlign = TextAlignmentOptions.TopLeft;

    [Header("TypeWriter Text List")]
    public List<string> TextList;

    [Header("Sinewave Settings")]
    [Range(0f, 200f)]
    public float WaveHeight = 60f;
    [Range(-10f, 10f)]
    public float WaveSpeed = 2f;
    public WaveDirection WaveDirection;
    public WaveStyle WaveStyle;

    private TMP_TextInfo textInfo;
    private bool isBusy;
    private int currentTextIndex, currentLetterIndex, lineCount;
    private AudioSource audioSource;

    private FancyTextSinuswave fts;

    private void Start()
    {
        currentTextIndex = 0;
        currentLetterIndex = 0;
        lineCount = 1;
        if (TextList == null || TextList.Count == 0) { TextList.Add(TextComponent.text); }
        TextComponent.text = string.Empty;
        textInfo = TextComponent.textInfo;
        TextComponent.alignment = TextAlign;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = .25f;

        fts = new FancyTextSinuswave();
    }

    private void Update()
    {
        if (!isBusy)
        {
            isBusy = true;
            StartCoroutine(WriteLetter());
        }

        if (WaveHeight > 0f && WaveSpeed != 0f)
        {
            TextComponent.ForceMeshUpdate();
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                var chrInfo = textInfo.characterInfo[i];

                if (!chrInfo.isVisible) { continue; }

                var verts = textInfo.meshInfo[chrInfo.materialReferenceIndex].vertices;
                fts.SinewaveText(verts, chrInfo.vertexIndex, WaveHeight, WaveSpeed, WaveDirection, WaveStyle);
            }
            TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }
    }

    IEnumerator WriteLetter()
    {
        if (currentLetterIndex < TextList[currentTextIndex].Length)
        {
            var currentLetter = TextList[currentTextIndex][currentLetterIndex];
            if (currentLetter == (char)32)  // Space
            {
                audioSource.PlayOneShot(TypeWriterSpaceAudio);
            }
            else
            {
                audioSource.PlayOneShot(TypeWriterAudio);
            }

            TextComponent.text += currentLetter;
            currentLetterIndex++;

            if (textInfo.lineCount > lineCount)
            {
                lineCount = textInfo.lineCount;
                ScrollText();
            }

            yield return new WaitForSeconds(WaitForNextLetter);
            isBusy = false;
        }
        else
        {
            yield return new WaitForSeconds(WaitForNextText);
            TextComponent.text = string.Empty;
            TextComponent.transform.localPosition = new Vector3(0f, 0f, 0f);
            currentLetterIndex = 0;
            lineCount = 1;

            if (currentTextIndex + 1 < TextList.Count)
            {
                currentTextIndex++;
            }
            else
            {
                currentTextIndex = 0;
            }

            isBusy = false;
        }
    }

    private void ScrollText()
    {
        var dif = ((RectTransform)transform).rect.height - TextComponent.mesh.bounds.size.y;
        if (dif <= 0f)
        {
            TextComponent.transform.Translate(Vector2.up * TextComponent.textInfo.lineInfo[0].lineHeight);
        }
    }
}

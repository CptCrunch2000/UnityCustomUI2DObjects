using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FancyNewsTickerController : MonoBehaviour
{
    public TMP_Text TextComponent;

    [Header("Ticker Text List")]
    public List<string> MessageList;

    [Header("Movement Settings")]
    public Vector2 Direction = Vector2.right;
    [Range(0f, 100f)]
    public float Speed = 10f;
    public float WaitForNextMessage = 3f;

    [Header("Sinewave Settings")]
    [Range(0f, 200f)]
    public float WaveHeight = 60f;
    public WaveDirection WaveDirection;
    public WaveStyle WaveStyle;
    [Range(-10f, 10f)]
    public float WaveSpeed = 2f;

    private FancyTextSinuswave fts;
    private bool useMessageList;
    private int messageIndex;
    private Vector2 preferredSize;
    private float textOffsetX, textOffsetY;
    private TMP_TextInfo textInfo;
    private bool moveable;

    void Start()
    {
        fts = new FancyTextSinuswave();
        moveable = true;

        if (MessageList != null && MessageList.Count > 0)
        {
            useMessageList = true;
        }
        textInfo = TextComponent.textInfo;
        preferredSize = TextComponent.GetPreferredValues(TextComponent.text);
        textOffsetX = (((RectTransform)transform).rect.width - preferredSize.x) / 2f;
        textOffsetY = (((RectTransform)transform).rect.height - preferredSize.y);        
    }

    void Update()
    {        
        MoveText();

        if(WaveHeight > 0f && WaveSpeed != 0f)
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

    private void MoveText()
    {
        if (!moveable) return;

        // Check if right border has crossed.
        if (Direction == Vector2.right)
        {
            if (TextComponent.transform.localPosition.x + textOffsetX > ((RectTransform)transform).rect.width)
            {
                moveable = false;
                if (useMessageList)
                {
                    StartCoroutine(SetTickerText(false));
                }
                TextComponent.transform.localPosition = new Vector3(-preferredSize.x - textOffsetX, 0f, 0f);
            }
        }

        // Check if left border has crossed.
        if (Direction == Vector2.left)
        {
            if (TextComponent.transform.localPosition.x + preferredSize.x + textOffsetX < 0f)
            {
                moveable = false;
                if (useMessageList)
                {
                    StartCoroutine(SetTickerText(false));
                }
                TextComponent.transform.localPosition = new Vector3(((RectTransform)transform).rect.width - textOffsetX, 0f, 0f);
            }
        }

        // Check if top border has crossed.
        if (Direction == Vector2.up)
        {
            if (TextComponent.transform.localPosition.y + preferredSize.y - WaveHeight > ((RectTransform)transform).rect.height)
            {
                moveable = false;
                if (useMessageList)
                {
                    StartCoroutine(SetTickerText(true));
                }
                TextComponent.transform.localPosition = new Vector3(0f, (-preferredSize.y / 2f) - textOffsetY - WaveHeight, 0f);
            }
        }

        // Check if bottom border has crossed.
        if (Direction == Vector2.down)
        {
            if (TextComponent.transform.localPosition.y + textOffsetY + WaveHeight < 0f)
            {
                moveable = false;
                if (useMessageList)
                {
                    StartCoroutine(SetTickerText(true));
                }
                TextComponent.transform.localPosition = new Vector3(0f, (preferredSize.y / 2f) + textOffsetY + WaveHeight, 0f);
            }
        }

        TextComponent.transform.Translate(Direction * Speed * Time.deltaTime);
    }

    IEnumerator SetTickerText(bool wordWrapping)
    {
        TextComponent.textWrappingMode = wordWrapping ? TextWrappingModes.Normal : TextWrappingModes.NoWrap;
        TextComponent.text = MessageList[messageIndex];
        preferredSize = TextComponent.GetPreferredValues(TextComponent.text);
        textOffsetX = (((RectTransform)transform).rect.width - preferredSize.x) / 2f;
        textOffsetY = ((RectTransform)transform).rect.height - preferredSize.y;
        messageIndex++;
        if (messageIndex >= MessageList.Count) messageIndex = 0;

        //Wait for N seconds
        yield return new WaitForSeconds(WaitForNextMessage);
        moveable = true;
    }
}

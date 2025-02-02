using UnityEngine;

public class FancyTextSinuswave
{
    public void SinewaveText(Vector3[] verts, int vertexIndex, float waveHeight, float waveSpeed, WaveDirection waveDirection, WaveStyle waveStyle)
    {
        for (int x = 0; x < 4; x++)
        {
            var orig = verts[vertexIndex + x];

            if (waveStyle == WaveStyle.Letter)
            {
                switch (waveDirection)
                {
                    case WaveDirection.Vertical:
                        verts[vertexIndex + x] = orig + new Vector3(Mathf.Sin(Time.time * waveSpeed + orig.x * 0.01f) * waveHeight, 0, 0f);
                        break;
                    case WaveDirection.Horizontal:
                        verts[vertexIndex + x] = orig + new Vector3(0, Mathf.Sin(Time.time * waveSpeed + orig.x * 0.01f) * waveHeight, 0f);
                        break;
                }
            }

            if(waveStyle == WaveStyle.Text)
            {
                switch (waveDirection)
                {
                    case WaveDirection.Vertical:
                        verts[vertexIndex + x] = orig + new Vector3(Mathf.Sin(waveSpeed * Time.time) * waveHeight, 0f, 0f);
                        break;
                    case WaveDirection.Horizontal:
                        verts[vertexIndex + x] = orig + new Vector3(0, Mathf.Sin(waveSpeed * Time.time) * waveHeight, 0f);         
                        break;
                }
            }
        }
    }
}

public enum WaveDirection
{
    Horizontal,
    Vertical
}

public enum WaveStyle
{
    Letter,
    Text
}
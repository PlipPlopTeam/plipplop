using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pencil
{
    public class OutOfUVException : System.Exception { public OutOfUVException(string message) : base(message) { } }

    static Color pencilColor = Color.white;

    public static void SetPencilColor(this Texture2D tex, Color c)
    {
        pencilColor = c;
    }

    public static void Point(this Texture2D tex, Vector2 UVOrigin){
        tex.Circle(UVOrigin, 1f);
    }

    public static void Circle(this Texture2D tex, Vector2 UVOrigin, float radius)
    {
        var origin = tex.UVToRasterCoordinate(UVOrigin);
        var iRadius = Mathf.RoundToInt(radius);

        for (int y = -iRadius; y <= iRadius; y++)
            for (int x = -iRadius; x <= iRadius; x++)
                if (x * x + y * y <= iRadius * iRadius + iRadius * 0.8)
                    tex.SetPixel(origin.x + x, origin.y + y, pencilColor);

    }

    public static void Line(this Texture2D tex, Vector2 UVP1, Vector2 UVP2, float width=1f)
    {
        var p1 = tex.UVToRasterCoordinate(UVP1);
        var p2 = tex.UVToRasterCoordinate(UVP2);

        // Dark magic found on the internet
        int w = p2.x - p1.x;
        int h = p2.y - p1.y;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
        if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
        if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
        int longest = Mathf.Abs(w);
        int shortest = Mathf.Abs(h);
        if (!(longest > shortest)) {
            longest = Mathf.Abs(h);
            shortest = Mathf.Abs(w);
            if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
            dx2 = 0;
        }
        int numerator = longest >> 1;
        for (int i = 0; i <= longest; i++) {
            tex.Circle(tex.RasterToUVCoordinates(p1), width);
            numerator += shortest;
            if (!(numerator < longest)) {
                numerator -= longest;
                p1 += new Vector2Int(dx1, dy1);
            }
            else {
                p1 += new Vector2Int(dx2, dy2);
            }
        }
    }

    public static void Polygon(this Texture2D tex, List<Vector2> UVs, float width = 1f)
    {
        tex.Lines(UVs, width);
        tex.Line(UVs[UVs.Count-1], UVs[0], width);
    }

    public static void Lines(this Texture2D tex, List<Vector2> UVs, float width = 1f)
    {
        for (int i = 1; i < UVs.Count; i++) {
            tex.Line(UVs[i - 1], UVs[i], width);
        }
    }

    public static void Fill(this Texture2D tex, Color color)
    {
        Fill(tex, Vector2.zero, Vector2.one, color);
    }


    public static void Fill(this Texture2D tex, Vector2 UV, Vector2 size, Color color)
    {
        for (int x = Mathf.FloorToInt(UV.x*tex.width); x < UV.x*tex.width+size.x*tex.width; x++) {
            for (int y = Mathf.FloorToInt(UV.y * tex.height); y < UV.y * tex.height + size.y * tex.height; y++) {
                tex.SetPixel(x, y, color);
            }
        }
    }
    /*
        public static void Lines(this Texture2D tex, Polygon UVSegments, float width = 1f)
        {
            foreach(var seg in UVSegments) {
                tex.Line(seg.a, seg.b, width);
            }
        }
        */
    public static void Number(this Texture2D tex, Vector2 UV, float number)
    {
        var str = number.ToString();
        float spacing = 5f;
        float width = 8f;
        float height = 16f;
        Vector2 unit = new Vector2(1f / tex.width, 1f / tex.height);
        float xOffset = -str.Length * (width + spacing) * unit.x ;
        foreach (char chr in str) {
            var uv = UV + new Vector2(xOffset, 0f);
            try {
                uv.CheckValue(new Vector2(width, height) * unit);
            }
            catch (OutOfUVException e) {
                //logger.Trace("This error can be safely ignored. " + e.Message);
                continue;
            }
            var left = uv.x - (width / 2) * unit.x;
            var right = uv.x + (width / 2) * unit.x;
            var bottom = uv.y - (height / 2) * unit.y;
            var top = uv.y + (height / 2) * unit.y;
            var middle = uv.y;
            var center = uv.x;

            #region switch(chars)
            switch (chr) {
                case ',': tex.Circle(new Vector2(center, bottom), 2f); break;

                case '0': tex.Polygon(new List<Vector2>() {
                    new Vector2(left, bottom),
                    new Vector2(right, bottom),
                    new Vector2(right, top),
                    new Vector2(left, top),
                    new Vector2(left, bottom),
                    new Vector2(right, top)
                }); break;

                case '1':tex.Lines(new List<Vector2>() {
                    new Vector2(left, bottom),
                    new Vector2(right, top),
                    new Vector2(right, bottom)
                }); break;

                case '2':
                    tex.Lines(new List<Vector2>() {
                    new Vector2(left, top),
                    new Vector2(right, top),
                    new Vector2(left, bottom),
                    new Vector2(right, bottom)
                }); break;

                case '3':
                    tex.Lines(new List<Vector2>() {
                    new Vector2(left, top),
                    new Vector2(right, top),
                    new Vector2(left, middle),
                    new Vector2(right, middle),
                    new Vector2(right, bottom),
                    new Vector2(left, bottom)
                }); break;

                case '4':
                    tex.Lines(new List<Vector2>() {
                    new Vector2(left, top),
                    new Vector2(left, middle),
                    new Vector2(right, middle),
                    new Vector2(right, top),
                    new Vector2(right, bottom)
                }); break;

                case '5':
                    tex.Lines(new List<Vector2>() {
                    new Vector2(right, top),
                    new Vector2(left, top),
                    new Vector2(right, bottom),
                    new Vector2(left, bottom)
                }); break;

                case '6':
                    tex.Lines(new List<Vector2>() {
                    new Vector2(right, top),
                    new Vector2(left, middle),
                    new Vector2(left, bottom),
                    new Vector2(right, bottom),
                    new Vector2(right, middle),
                    new Vector2(left, middle)
                }); break;

                case '7':
                    tex.Lines(new List<Vector2>() {
                    new Vector2(left, top),
                    new Vector2(right, top),
                    new Vector2(left, bottom)
                }); break;

                case '8':
                    tex.Lines(new List<Vector2>() {
                    new Vector2(left, top),
                    new Vector2(right, top),
                    new Vector2(right, bottom),
                    new Vector2(left, bottom),
                    new Vector2(left, top),
                    new Vector2(left, middle),
                    new Vector2(right, middle)
                }); break;

                case '9':
                    tex.Lines(new List<Vector2>() {
                    new Vector2(right, middle),
                    new Vector2(right, top),
                    new Vector2(left, top),
                    new Vector2(left, middle),
                    new Vector2(right, middle),
                    new Vector2(left, bottom)
                }); break;
            }
#endregion
            xOffset += (width+spacing) * unit.x;
        }
    }

    static Vector2Int UVToRasterCoordinate(this Texture2D tex, Vector2 UV)
    {
        UV.CheckValue();
        return new Vector2Int(
            Mathf.RoundToInt(UV.x * tex.width),
            Mathf.RoundToInt(UV.y * tex.height)
        );
    }

    static Vector2 RasterToUVCoordinates(this Texture2D tex, Vector2 rasterCoords)
    {
        return new Vector2(
            (rasterCoords.x / (float)tex.width),
            (rasterCoords.y / (float)tex.height)
        );
    }
    
    static bool OffTexture(this Texture2D tex, Vector2 rasterCoords)
    {
        return rasterCoords.x >= 0 && rasterCoords.y >= 0 && rasterCoords.x < tex.width && rasterCoords.y < tex.height;
    }

    static void CheckValue(this Vector2 UV)
    {
        if (UV.x < 0f || UV.y < 0f || UV.magnitude > Mathf.Sqrt(2f)) {
            throw (new OutOfUVException("Invalid UV (out of bounds?) : "+ UV +"->"+ UV.magnitude));
        }
    }

    static void CheckValue(this Vector2 UV, Vector2 variation)
    {
        CheckValue(UV + variation);
        CheckValue(UV - variation);
    }
}

using UnityEngine;
public class ColorFieldAttribute : PropertyAttribute {
    public Color c;
    public ColorFieldAttribute(float r, float g, float b)
    {
        this.c = new Color(r, g, b);
    }
}
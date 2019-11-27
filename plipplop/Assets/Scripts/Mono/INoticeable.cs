using UnityEngine;

public interface INoticeable
{
    void Notice();
    bool IsVisible();
    void SetVisible(bool value);
}

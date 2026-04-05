// Scripts/UI/Shared/PassThroughImage.cs
using UnityEngine;
using UnityEngine.UI;

public class PassThroughImage : Image
{
    private bool _isDragging = false;

    public void SetDragging(bool value) => _isDragging = value;

    // chỉ block raycast khi đang drag
    public override bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        return _isDragging;
    }
}
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UISelectButton : MonoBehaviour
{
    [SerializeField] private Image _bgImage;
    [SerializeField] private Text _text;
    [SerializeField] private Color _selectColor;
    [SerializeField] private Color _deselectColor;

    [ReadOnly] public int genderId;
    
    public UnityAction<int> Clicked;
    
    private bool _isSelected = false;
    
    public bool IsSelected => _isSelected;

    public void Select()
    {
        _isSelected = true;
        
        _bgImage.color = _selectColor;
        _text.color = _selectColor;
    }

    public void Deselect()
    {
        _isSelected = false;
        _bgImage.color = _deselectColor;
        _text.color = _deselectColor;
    }

    public void Click()
    {
        Clicked?.Invoke(genderId);
    }
}

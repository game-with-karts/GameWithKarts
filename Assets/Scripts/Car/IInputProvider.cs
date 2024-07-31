using System;
 
namespace GWK.Kart {
    public interface IInputProvider {
        event Action<float> HorizontalPerformed;
        event Action<float> VerticalPerformed;
        event Action<bool> Jump1;
        event Action<bool> Jump2;
        event Action Item;
        event Action<bool> BackCamera;
    }
}

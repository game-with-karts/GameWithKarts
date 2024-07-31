using GWK.Kart;

public interface IItemInteractable {
    public BaseCar parentCar { get; }
    public void OnItemBox();
}
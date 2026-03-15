namespace SmartCart.Application.Interfaces
{
    public interface ICouponFactory
    {
        ICouponStrategy? GetStrategy(string code);
    }
}

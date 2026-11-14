namespace Pizzeria.Domain.Core
{
    public enum ProductCategory
    {
        Pizza = 1,
        Beverage = 2,
        Side = 3,
        Dessert = 4
    }

    public enum OrderType
    {
        DineIn = 1,
        TakeOut = 2,
        Delivery = 3
    }

    public enum OrderStatus
    {
        Pending = 1,
        InPreparation = 2,
        Ready = 3,
        OnTheWay = 4,
        Delivered = 5,
        Cancelled = 6
    }

    public enum DeliveryStatus
    {
        Pending = 1,
        OnTheWay = 2,
        Delivered = 3,
        Cancelled = 4
    }
}

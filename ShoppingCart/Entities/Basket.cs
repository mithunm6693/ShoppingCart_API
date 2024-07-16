namespace ShoppingCart.Entities
{
    public class Basket
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }
        public List<BasketItem> Items { get; set; } = new List<BasketItem>();

        public void AddItem(Product product, int quantity)
        {
            if (Items.All(item => item.ProductId != product.Id))
            {
                Items.Add(new BasketItem { Quantity = quantity, Product = product });
            }

            var ExistingItem = Items.FirstOrDefault(x => x.ProductId == product.Id);
            if (ExistingItem != null)
            {
                ExistingItem.Quantity = quantity;
            }
        }

        public void RemoveItem(int productid, int quantity)
        {
            var ExistingItem = Items.FirstOrDefault(x => x.ProductId == productid);
            if (ExistingItem != null)
            {
                ExistingItem.Quantity -= quantity;
                if (ExistingItem.Quantity == 0)
                {
                    Items.Remove(ExistingItem);
                }
            }
        }




    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Data;
using ShoppingCart.DTO;
using ShoppingCart.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShoppingCart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly StoreContext _context;

        public BasketController(StoreContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<BasketDto>> GetBasket()
        {
            var basket = await RetriveBasket();
            if (basket != null)
            {
                return new BasketDto
                {
                    Id = basket.Id,
                    BuyerId = basket.BuyerId,
                    Items = basket.Items.Select(item => new BasketItemDto
                    {
                        Brand = item.Product.Brand,
                        Name = item.Product.Name,
                        PictureUrl = item.Product.PictureUrl,
                        Price = item.Product.Price,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Type = item.Product.Type

                    }).ToList(),
                };
            }
            else
            { return NotFound(); }
        }



        [HttpPost]
        public async Task<ActionResult> AddItemTobasket(int productId, int quantity)
        {
            //get basket || create basket

            var basket = await RetriveBasket();

            if (basket == null)
            {
                basket = CreateBakset();
            }

            //get product

            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            basket.AddItem(product, quantity);

            var results = await _context.SaveChangesAsync() > 0;
            if (results)
            {
                return Ok(201);
            }
            else
            {
                return BadRequest(new ProblemDetails { Title = "error in saving items to basket" });
            }



        }

        [HttpDelete]
        public async Task<ActionResult> RemovebasketItem(int productId, int quantity)
        {

            var basket = await RetriveBasket();
            if (basket == null)
            {
                return NotFound();
            }
            basket.RemoveItem(productId, quantity);
            if (await _context.SaveChangesAsync() > 0)
            {
                return Ok();
            }

            return BadRequest(new ProblemDetails { Title = "Error in removing items from the basket" });


        }

        private async Task<Basket> RetriveBasket()
        {
            return await _context.Baskets.Include(i => i.Items).ThenInclude(p => p.Product).FirstOrDefaultAsync(x => x.BuyerId == Request.Cookies["buyerid"]);
        }

        private Basket CreateBakset()
        {
            var buyerid = Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions { IsEssential = true, Expires = DateTime.Now.AddDays(30) };
            Response.Cookies.Append("buyerid", buyerid, cookieOptions);
            var basket = new Basket { BuyerId = buyerid };
            _context.Baskets.Add(basket);
            return basket;
        }


    }
}

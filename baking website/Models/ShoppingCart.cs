using baking_website.Models;
using Microsoft.EntityFrameworkCore;

namespace baking_website.Models
{
    public class ShoppingCart : IShoppingCart
    {
        private readonly BethanysPieShopDbContext _bethanysPieShopDbContext;

        private ShoppingCart(BethanysPieShopDbContext bethanysPieShopDbContext)
        {
            _bethanysPieShopDbContext = bethanysPieShopDbContext;
        }


        public string? ShoppingCartId { get; set; }

        public List<ShoppingCartItem> ShoppingCartItems { get; set; } = default!;


        public static ShoppingCart GetCart(IServiceProvider services)
        {

            //IServiceProvider is an in-built interface that allows you to interact/call actions/services to use within your project
            //this will be passed in from the Program.cs file
            //This method is called whenever an instnce of the IShoppingCart is created - configured in the Program.cs file 

            ISession? session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Session;
            //this is getting access to the session
            //services = IServiceProvider, which can access certain actions/services to use in the project...
            //Here is asking for information about the current session
            //is obtained through the IHttpContextAccessor service

            BethanysPieShopDbContext context = services.GetService<BethanysPieShopDbContext>() ?? throw new Exception("Error initializing");
            //getting access to the DbContext
            //GetService retrieves a service of type <T> from the DI container
            //Remember we register services in the Program.cs file... have registered DbContext as a service there 
         

            string cartId = session?.GetString("CartId") ?? Guid.NewGuid().ToString();
            //check, based on the session, if there is already a value for the key CartId for the incoming user - if not create a new guid
            //if there is one, then we will take that value from the session
            //behind the scenes ASP.NET uses a cookie to associate different requests from the same user (machine)
            //so this is just a string it creates and 'attaches' to the session - for every session, there is a 'CartId' key that can be retrieved


            session?.SetString("CartId", cartId);
            //set the value of the cart Id for the session
            //SetString sets a key-value pair for the session
            //Just a way to assign/store information for the session state 
            //if previous CartId key was found then that will be set for the session
            //if a new guid was made, then that will be set for the session 

            return new ShoppingCart(context) { ShoppingCartId = cartId };
            //we create a new ShoppingCart object for the session, where the id == session id
            //have to pass in the db context when creating a new instance of this object
            //So anytime a new session is started...a new shopping cart object will be created for it...
            //The id == the session id if there is one, or a new guid for the session
            //Whenever any updtes to the db are made from the below methods, it will pass through this id and update db accordingly
            //so this GetCart method is just to create a shopping cart object for each session... and allow shopping carts from the same
            //session persist through searching for the session's CartId
        }

        public void AddToCart(Pie pie)
        {
            var shoppingCartItem =
                    _bethanysPieShopDbContext.ShoppingCartItems.SingleOrDefault(
                        s => s.Pie.PieId == pie.PieId && s.ShoppingCartId == ShoppingCartId);
            //check the shopping cart items in the DB where the Pie's Id and the ShoppingCartId match
            //i.e. The shopping item will have a PieId and a ShoppingCartId... it can have lots of Items with the same
            //PieId, but they will all belong to different shopping carts
            //So here, we're saying get the Pie of this Id that is a part of this shopping cart (using its Id)

            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartId = ShoppingCartId,
                    Pie = pie,
                    Amount = 1
                };

                _bethanysPieShopDbContext.ShoppingCartItems.Add(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.Amount++;
            }
            _bethanysPieShopDbContext.SaveChanges();
        }

        //If a pie of this Id + shopping cart Id doesn't exist, it means we are adding the first pie of this type to the shopping cart
        //we create a new shopping cart item and add it to the db 
        //If one does exist, it means we are just adding an extra amount of this pie to the cart
        //if it exists, we just need to update this item with one more...


        public int RemoveFromCart(Pie pie)
        {
            var shoppingCartItem =
                    _bethanysPieShopDbContext.ShoppingCartItems.SingleOrDefault(
                        s => s.Pie.PieId == pie.PieId && s.ShoppingCartId == ShoppingCartId);

            var localAmount = 0;

            if (shoppingCartItem != null)
            {
                if (shoppingCartItem.Amount > 1)
                {
                    shoppingCartItem.Amount--;
                    localAmount = shoppingCartItem.Amount;
                }
                else
                {
                    _bethanysPieShopDbContext.ShoppingCartItems.Remove(shoppingCartItem);
                }
            }

            _bethanysPieShopDbContext.SaveChanges();

            return localAmount;
        }

        //take the Pie passed in, search for it in the SCItems table, where the SCId is the same as the session Id
        //If this exists (there is a Pie of this type for this sessions shopping cart)
        //If the amount for this item >1, then -1 from the amount, else, Remove this Item from the SCItem table
        //(and thus from this session's shopping cart) 

        public List<ShoppingCartItem> GetShoppingCartItems()
        {
            return ShoppingCartItems ??=
                       _bethanysPieShopDbContext.ShoppingCartItems.Where(c => c.ShoppingCartId == ShoppingCartId)
                           .Include(s => s.Pie)
                           .ToList();
        }
        //get all Items for this sessions shopping cart
        //only include the Pies for every item 

        public void ClearCart()
        {
            var cartItems = _bethanysPieShopDbContext
                .ShoppingCartItems
                .Where(cart => cart.ShoppingCartId == ShoppingCartId);

            _bethanysPieShopDbContext.ShoppingCartItems.RemoveRange(cartItems);

            _bethanysPieShopDbContext.SaveChanges();
        }

        //get all the items from this sessions shopping cart
        //remove all items in this range 

        public decimal GetShoppingCartTotal()
        {
            var total = _bethanysPieShopDbContext.ShoppingCartItems.Where(c => c.ShoppingCartId == ShoppingCartId)
                .Select(c => c.Pie.Price * c.Amount).AsEnumerable().Sum();
            return total;
        }

        //get all items from this sessions shopping cart, and do a calculation on selected properties
    }
}


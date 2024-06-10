using ECommerce.API.Models;

namespace ECommerce.API.DataAccess
{
    public interface IDataAccess
    {
        List<ProductCategory> GetProductCategories();
        List<Offer> GetOfferList();
        ProductCategory GetProductCategory(int id);
        Offer GetOffer(int id);
        List<Product> GetProducts(string category, string subcategory, int count); 
        Product GetProduct(int id);
        int InsertUser(User user);
        string IsUserPresent(string email, string password);
        void InsertReview(Review review);
        List<Review> GetProductReviews(int productId);
        User GetUser(int id);
        bool InsertCartItem(int userId, int productId);
        Cart GetActiveCartOfUser(int userid);
        Cart GetCart(int cartid);
        List<Cart> GetAllPreviousCartsOfUser(int userid);
        List<PaymentMethod> GetPaymentMethods();
        int InsertPayment(Payment payment);
        int InsertOrder(Order order);
        void InsertProduct(Product product);
        bool DeleteProduct(int id);
        bool DeleteCartItemByProductId(int productId);
        List<Product> GetProductsFromQuery();
        List<Admin> GetAllAdmin();
        List<Order> GetOrders();
        bool InsertAdmin(Admin admin);
        string IsAdminPresent(string email, string password);
        int UpdateUser(User user); 
    }
}

using ECommerce.API.Models;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace ECommerce.API.DataAccess
{
    public class DataAccess : IDataAccess
    {
        private readonly IConfiguration configuration;
        private readonly string dbconnection;
        private readonly string dateformat;

        MyDatabase _myDatabase;
        public DataAccess(IConfiguration configuration)
        {
            this.configuration = configuration;
            dbconnection = this.configuration["ConnectionStrings:DB"];
            dateformat = this.configuration["Constants:DateFormat"];
            _myDatabase = new MyDatabase(configuration);
        }
        #region Cart
        public Cart GetActiveCartOfUser(int userid)
        {
            var cart = new Cart();
            using (SqlConnection connection = new(dbconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };
                connection.Open();

                string query = "SELECT COUNT(*) From Carts WHERE UserId=" + userid + " AND Ordered='false';";
                command.CommandText = query;

                int count = (int)command.ExecuteScalar();
                if (count == 0)
                {
                    return cart;
                }

                query = "SELECT CartId From Carts WHERE UserId=" + userid + " AND Ordered='false';";
                command.CommandText = query;

                int cartid = (int)command.ExecuteScalar();

                query = "select * from CartItems where CartId=" + cartid + ";";
                command.CommandText = query;

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    CartItem item = new()
                    {
                        Id = (int)reader["CartItemId"],
                        Product = GetProduct((int)reader["ProductId"])
                    };
                    cart.CartItems.Add(item);
                }

                cart.Id = cartid;
                cart.User = GetUser(userid);
                cart.Ordered = false;
                cart.OrderedOn = "";
            }
            return cart;
        }

        public List<Cart> GetAllPreviousCartsOfUser(int userid)
        {
            var carts = new List<Cart>();
            using (SqlConnection connection = new(dbconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };
                string query = "SELECT CartId FROM Carts WHERE UserId=" + userid + " AND Ordered='true';";
                command.CommandText = query;
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var cartid = (int)reader["CartId"];
                    carts.Add(GetCart(cartid));
                }
            }
            return carts;
        }

        public Cart GetCart(int cartid)
        {
            var cart = new Cart();
            using (SqlConnection connection = new(dbconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };
                connection.Open();

                string query = "SELECT * FROM CartItems WHERE CartId=" + cartid + ";";
                command.CommandText = query;

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    CartItem item = new()
                    {
                        Id = (int)reader["CartItemId"],
                        Product = GetProduct((int)reader["ProductId"])
                    };
                    cart.CartItems.Add(item);
                }
                reader.Close();

                query = "SELECT * FROM Carts WHERE CartId=" + cartid + ";";
                command.CommandText = query;
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    cart.Id = cartid;
                    cart.User = GetUser((int)reader["UserId"]);
                    cart.Ordered = bool.Parse((string)reader["Ordered"]);
                    cart.OrderedOn = (string)reader["OrderedOn"];
                }
                reader.Close();
            }
            return cart;
        }

        public bool DeleteCartItemByProductId(int id)
        {
            using (SqlConnection connection = new SqlConnection(dbconnection))
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;

                // Xóa tất cả các mục sản phẩm có ProductId cụ thể
                string query = "DELETE FROM CartItems WHERE ProductId = @productId";
                command.CommandText = query;
                command.Parameters.Add("@productId", SqlDbType.Int).Value = id;

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }
        #endregion

        #region Offer
        public Offer GetOffer(int id)
        {
            var offer = new Offer();
            using (SqlConnection connection = new(dbconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };

                string query = "SELECT * FROM Offers WHERE OfferId=" + id + ";";
                command.CommandText = query;

                connection.Open();
                SqlDataReader r = command.ExecuteReader();
                while (r.Read())
                {
                    offer.Id = (int)r["OfferId"];
                    offer.Title = (string)r["Title"];
                    offer.Discount = (int)r["Discount"];
                }
            }
            return offer;
        }
        public List<Offer> GetOfferList()
        {
            var offerlist = new List<Offer>();
            using (SqlConnection connection = new(dbconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };
                string query = "Select * from Offers;";
                command.CommandText = query;

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var offer = new Offer()
                    {
                        Id = (int)reader["OfferId"],
                        Title = (string)reader["Title"],
                        Discount = (int)reader["Discount"]
                    };
                    offerlist.Add(offer);
                }
            }
            return offerlist;
        }
        #endregion

        public List<PaymentMethod> GetPaymentMethods()
        {
            var result = new List<PaymentMethod>();
            using (SqlConnection connection = new(dbconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };

                string query = "SELECT * FROM PaymentMethods;";
                command.CommandText = query;

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    PaymentMethod paymentMethod = new()
                    {
                        Id = (int)reader["PaymentMethodId"],
                        Type = (string)reader["Type"],
                        Provider = (string)reader["Provider"],
                        Available = bool.Parse((string)reader["Available"]),
                        Reason = (string)reader["Reason"]
                    };
                    result.Add(paymentMethod);
                }
            }
            return result;
        }

        #region Product
        public Product GetProduct(int id)
        {
            var product = new Product();
            using (SqlConnection connection = new(dbconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };

                string query = "SELECT * FROM Products WHERE ProductId=" + id + ";";
                command.CommandText = query;

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    product.Id = (int)reader["ProductId"];
                    product.Title = (string)reader["Title"];
                    product.Description = (string)reader["Description"];
                    product.Price = (double)reader["Price"];
                    product.Quantity = (int)reader["Quantity"];
                    product.ImageName = (string)reader["ImageName"];

                    var categoryid = (int)reader["CategoryId"];
                    product.ProductCategory = GetProductCategory(categoryid);

                    var offerid = (int)reader["OfferId"];
                    product.Offer = GetOffer(offerid);
                }
            }
            return product;
        }

        public List<ProductCategory> GetProductCategories()
        {
            var productCategories = new List<ProductCategory>();
            using (SqlConnection connection = new(dbconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };
                string query = "SELECT * FROM ProductCategories;";
                command.CommandText = query;

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var category = new ProductCategory()
                    {
                        Id = (int)reader["CategoryId"],
                        Category = (string)reader["Category"],
                        SubCategory = (string)reader["SubCategory"]
                    };
                    productCategories.Add(category);
                }
            }
            return productCategories;
        }

        public ProductCategory GetProductCategory(int id)
        {
            var productCategory = new ProductCategory();

            using (SqlConnection connection = new(dbconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };

                string query = "SELECT * FROM ProductCategories WHERE CategoryId=" + id + ";";
                command.CommandText = query;

                connection.Open();
                SqlDataReader r = command.ExecuteReader();
                while (r.Read())
                {
                    productCategory.Id = (int)r["CategoryId"];
                    productCategory.Category = (string)r["Category"];
                    productCategory.SubCategory = (string)r["SubCategory"];
                }
            }

            return productCategory;
        }
        public List<Product> GetProducts(string category, string subcategory, int count)
        {
            var products = new List<Product>();
            using (SqlConnection connection = new(dbconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };

                string query = "SELECT TOP " + count + " * FROM Products WHERE CategoryId=(SELECT CategoryId FROM ProductCategories WHERE Category=@c AND SubCategory=@s) ORDER BY newid();";
                command.CommandText = query;
                command.Parameters.Add("@c", System.Data.SqlDbType.NVarChar).Value = category;
                command.Parameters.Add("@s", System.Data.SqlDbType.NVarChar).Value = subcategory;

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var product = new Product()
                    {
                        Id = (int)reader["ProductId"],
                        Title = (string)reader["Title"],
                        Description = (string)reader["Description"],
                        Price = (double)reader["Price"],
                        Quantity = (int)reader["Quantity"],
                        ImageName = (string)reader["ImageName"]
                    };

                    var categoryid = (int)reader["CategoryId"];
                    product.ProductCategory = GetProductCategory(categoryid);

                    var offerid = (int)reader["OfferId"];
                    product.Offer = GetOffer(offerid);

                    products.Add(product);
                }
            }
            return products;
        }
        public List<Product> GetProductsFromQuery()
        {
            List<Product> products = new List<Product>();

            using (SqlConnection connection = new SqlConnection(dbconnection))
            {
                SqlCommand command = new SqlCommand
                {
                    Connection = connection
                };

                string query = "SELECT p.*, pc.*, o.* " +
                               "FROM Products p " +
                               "INNER JOIN ProductCategories pc ON p.CategoryId = pc.CategoryId " +
                               "INNER JOIN Offers o ON p.OfferId = o.OfferId";

                command.CommandText = query;

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Product product = new Product
                    {
                        Id = (int)reader["ProductId"],
                        Title = (string)reader["Title"],
                        Description = (string)reader["Description"],
                        ProductCategory = new ProductCategory
                        {
                            Id = (int)reader["CategoryId"],
                            Category = (string)reader["Category"],
                            SubCategory = (string)reader["SubCategory"]
                        },
                        Offer = new Offer
                        {
                            Id = (int)reader["OfferId"],
                            Title = (string)reader["Title"],
                            Discount = (int)reader["Discount"]
                        },
                        Price = (double)reader["Price"],
                        Quantity = (int)reader["Quantity"],
                        ImageName = (string)reader["ImageName"]
                    };
                    products.Add(product);
                }
            }

            return products;
        }
        public void InsertProduct(Product product)
        {
            using SqlConnection connection = new(dbconnection);
            SqlCommand command = new()
            {
                Connection = connection
            };

            string query = "INSERT INTO Products (Title, Description, CategoryId, OfferId, Price, Quantity, ImageName) VALUES (@t, @d, @c, @o, @p, @q, @i)";
            command.CommandText = query;
            command.Parameters.Add("@t", SqlDbType.NVarChar).Value = product.Title;
            command.Parameters.Add("@d", SqlDbType.NVarChar).Value = product.Description;
            command.Parameters.Add("@c", SqlDbType.Int).Value = product.ProductCategory.Id;
            command.Parameters.Add("@o", SqlDbType.Int).Value = product.Offer.Id;
            command.Parameters.Add("@p", SqlDbType.Float).Value = product.Price;
            command.Parameters.Add("@q", SqlDbType.Int).Value = product.Quantity;
            command.Parameters.Add("@i", SqlDbType.NVarChar).Value = product.ImageName;

            connection.Open();
            command.ExecuteNonQuery();
        }
        public bool DeleteProduct(int id)
        {
            using (SqlConnection connection = new SqlConnection(dbconnection))
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;

                string deleteQuery = "DELETE FROM Products WHERE ProductId = @productId";
                command.CommandText = deleteQuery;
                command.Parameters.Add("@productId", SqlDbType.Int).Value = id;

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }
        #endregion


        public List<Review> GetProductReviews(int productId)
        {
            var reviews = new List<Review>();
            using (SqlConnection connection = new(dbconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };

                string query = "SELECT * FROM Reviews WHERE ProductId=" + productId + ";";
                command.CommandText = query;

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var review = new Review()
                    {
                        Id = (int)reader["ReviewId"],
                        Value = (string)reader["Review"],
                        CreatedAt = (string)reader["CreatedAt"]
                    };

                    var userid = (int)reader["UserId"];
                    review.User = GetUser(userid);

                    var productid = (int)reader["ProductId"];
                    review.Product = GetProduct(productid);

                    reviews.Add(review);
                }
            }
            return reviews;
        }

 

        public bool InsertCartItem(int userId, int productId)
        {
            using (SqlConnection connection = new(dbconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };

                connection.Open();
                string query = "SELECT COUNT(*) FROM Carts WHERE UserId=" + userId + " AND Ordered='false';";
                command.CommandText = query;
                int count = (int)command.ExecuteScalar();
                if (count == 0)
                {
                    query = "INSERT INTO Carts (UserId, Ordered, OrderedOn) VALUES (" + userId + ", 'false', '');";
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }

                query = "SELECT CartId FROM Carts WHERE UserId=" + userId + " AND Ordered='false';";
                command.CommandText = query;
                int cartId = (int)command.ExecuteScalar();


                query = "INSERT INTO CartItems (CartId, ProductId) VALUES (" + cartId + ", " + productId + ");";
                command.CommandText = query;
                command.ExecuteNonQuery();
                return true;
            }
        }
        public int InsertOrder(Order order)
        {
            int value = 0;

            using (SqlConnection connection = new(dbconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };

                string query = "INSERT INTO Orders (UserId, CartId, PaymentId, CreatedAt) values (@uid, @cid, @pid, @cat);";

                command.CommandText = query;
                command.Parameters.Add("@uid", System.Data.SqlDbType.Int).Value = order.User.Id;
                command.Parameters.Add("@cid", System.Data.SqlDbType.Int).Value = order.Cart.Id;
                command.Parameters.Add("@cat", System.Data.SqlDbType.NVarChar).Value = order.CreatedAt;
                command.Parameters.Add("@pid", System.Data.SqlDbType.Int).Value = order.Payment.Id;

                connection.Open();
                value = command.ExecuteNonQuery();

                if (value > 0)
                {
                    query = "UPDATE Carts SET Ordered='true', OrderedOn='" + DateTime.Now.ToString(dateformat) + "' WHERE CartId=" + order.Cart.Id + ";";
                    command.CommandText = query;
                    command.ExecuteNonQuery();

                    query = "SELECT TOP 1 Id FROM Orders ORDER BY Id DESC;";
                    command.CommandText = query;
                    value = (int)command.ExecuteScalar();
                }
                else
                {
                    value = 0;
                }
            }

            return value;
        }

        public int InsertPayment(Payment payment)
        {
            int value = 0;
            using (SqlConnection connection = new(dbconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };

                string query = @"INSERT INTO Payments (PaymentMethodId, UserId, TotalAmount, ShippingCharges, AmountReduced, AmountPaid, CreatedAt) 
                                VALUES (@pay, @uid, @ta, @sc, @ar, @ap, @cat);";

                command.CommandText = query;
                command.Parameters.Add("@pay", System.Data.SqlDbType.Int).Value = payment.PaymentMethod.Id;
                command.Parameters.Add("@uid", System.Data.SqlDbType.Int).Value = payment.User.Id;
                command.Parameters.Add("@ta", System.Data.SqlDbType.NVarChar).Value = payment.TotalAmount;
                command.Parameters.Add("@sc", System.Data.SqlDbType.NVarChar).Value = payment.ShipingCharges;
                command.Parameters.Add("@ar", System.Data.SqlDbType.NVarChar).Value = payment.AmountReduced;
                command.Parameters.Add("@ap", System.Data.SqlDbType.NVarChar).Value = payment.AmountPaid;
                command.Parameters.Add("@cat", System.Data.SqlDbType.NVarChar).Value = payment.CreatedAt;

                connection.Open();
                value = command.ExecuteNonQuery();

                if (value > 0)
                {
                    query = "SELECT TOP 1 Id FROM Payments ORDER BY Id DESC;";
                    command.CommandText = query;
                    value = (int)command.ExecuteScalar();
                }
                else
                {
                    value = 0;
                }
            }
            return value;
        }

        public void InsertReview(Review review)
        {
            using SqlConnection connection = new(dbconnection);
            SqlCommand command = new()
            {
                Connection = connection
            };

            string query = "INSERT INTO Reviews (UserId, ProductId, Review, CreatedAt) VALUES (@uid, @pid, @rv, @cat);";
            command.CommandText = query;
            command.Parameters.Add("@uid", System.Data.SqlDbType.Int).Value = review.User.Id;
            command.Parameters.Add("@pid", System.Data.SqlDbType.Int).Value = review.Product.Id;
            command.Parameters.Add("@rv", System.Data.SqlDbType.NVarChar).Value = review.Value;
            command.Parameters.Add("@cat", System.Data.SqlDbType.NVarChar).Value = review.CreatedAt;

            connection.Open();
            command.ExecuteNonQuery();
        }


        #region User
        // thêm user
        public int InsertUser(User user)
        {

            SqlParameter[] sqlParameters = new SqlParameter[]
             {
                    new SqlParameter("@UserId", user.Id),
                     new SqlParameter("@FirstName", user.FirstName),
                      new SqlParameter("@LastName", user.LastName),
                       new SqlParameter("@Address", user.Address),
                        new SqlParameter("@Mobile", user.Mobile),
                         new SqlParameter("@Email", user.Email),
                          new SqlParameter("@Password", user.Password),
                           new SqlParameter("@CreatedAt", user.CreatedAt),
                           new SqlParameter("@ModifiedAt", user.ModifiedAt)
             };
            return _myDatabase.MyExcuteNonQuery("PSP_User_UPdate", CommandType.StoredProcedure, sqlParameters);
        }

        // đăng nhập
        public string IsUserPresent(string email, string password)
        {
            User user = new();
            using (SqlConnection connection = new(dbconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };

                connection.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Email='" + email + "' AND Password='" + password + "';";
                command.CommandText = query;
                int count = (int)command.ExecuteScalar();
                if (count == 0)
                {
                    connection.Close();
                    return "";
                }

                query = "SELECT * FROM Users WHERE Email='" + email + "' AND Password='" + password + "';";
                command.CommandText = query;

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    user.Id = (int)reader["UserId"];
                    user.FirstName = (string)reader["FirstName"];
                    user.LastName = (string)reader["LastName"];
                    user.Email = (string)reader["Email"];
                    user.Address = (string)reader["Address"];
                    user.Mobile = (string)reader["Mobile"];
                    user.Password = (string)reader["Password"];
                    user.CreatedAt = (string)reader["CreatedAt"];
                    user.ModifiedAt = (string)reader["ModifiedAt"];
                }

                string key = "MNU66iBl3T5rh6H52i69";
                string duration = "60";
                var symmetrickey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(symmetrickey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("firstName", user.FirstName),
                    new Claim("lastName", user.LastName),
                    new Claim("address", user.Address),
                    new Claim("mobile", user.Mobile),
                    new Claim("email", user.Email),
                    new Claim("createdAt", user.CreatedAt),
                    new Claim("modifiedAt", user.ModifiedAt)
                };

                var jwtToken = new JwtSecurityToken(
                    issuer: "localhost",
                    audience: "localhost",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(Int32.Parse(duration)),
                    signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(jwtToken);
            }
            return "";
        }

        //Sửa User
        public int UpdateUser(User user)
        {
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                    new SqlParameter("@UserId", user.Id),
                     new SqlParameter("@FirstName", user.FirstName),
                      new SqlParameter("@LastName", user.LastName),
                       new SqlParameter("@Address", user.Address),
                        new SqlParameter("@Mobile", user.Mobile),
                         new SqlParameter("@Email", user.Email),
                          new SqlParameter("@Password", user.Password),
                           new SqlParameter("@CreatedAt", user.CreatedAt),
                           new SqlParameter("@ModifiedAt", user.ModifiedAt)
            };
            return _myDatabase.MyExcuteNonQuery("PSP_User_UPdate", CommandType.StoredProcedure, sqlParameters);

        }

        //lấy danh sách user
        public User GetUser(int id)
        {
            var user = new User();
            using (SqlConnection connection = new(dbconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };

                string query = "SELECT * FROM Users WHERE UserId=" + id + ";";
                command.CommandText = query;

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    user.Id = (int)reader["UserId"];
                    user.FirstName = (string)reader["FirstName"];
                    user.LastName = (string)reader["LastName"];
                    user.Email = (string)reader["Email"];
                    user.Address = (string)reader["Address"];
                    user.Mobile = (string)reader["Mobile"];
                    user.Password = (string)reader["Password"];
                    user.CreatedAt = (string)reader["CreatedAt"];
                    user.ModifiedAt = (string)reader["ModifiedAt"];
                }
            }
            return user;
        }
        #endregion

        #region Admin
        public bool InsertAdmin(Admin admin)
        {
            using (SqlConnection connection = new(dbconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };
                connection.Open();
                string query = "SELECT COUNT(*) FROM Admin WHERE Email='" + admin.Email + "';";
                command.CommandText = query;
                int count = (int)command.ExecuteScalar();
                if (count > 0)
                {
                    connection.Close();
                    return false;
                }
                query = "insert into dbo.admin(namead,phone,email,password,address) values(@name , @phone,@email,@password,@address)";
                command.CommandText = query;
                command.Parameters.Add("@name", System.Data.SqlDbType.NVarChar).Value = admin.Name;
                command.Parameters.Add("@phone", System.Data.SqlDbType.NVarChar).Value = admin.Phone;
                command.Parameters.Add("@email", System.Data.SqlDbType.NVarChar).Value = admin.Email;
                command.Parameters.Add("@password", System.Data.SqlDbType.NVarChar).Value = admin.Password;
                command.Parameters.Add("@address", System.Data.SqlDbType.NVarChar).Value = admin.Address;
                command.ExecuteNonQuery();
            }
            return true;
        }
        public string IsAdminPresent(string email, string password)
        {
            Admin admin = new Admin();
            using (SqlConnection connection = new(dbconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };

                connection.Open();
                string query = "SELECT COUNT(*) FROM admin WHERE Email='" + email + "' AND Password='" + password + "';";
                command.CommandText = query;
                int count = (int)command.ExecuteScalar();
                if (count == 0)
                {
                    connection.Close();
                    return "";
                }

                query = "SELECT * FROM admin WHERE Email='" + email + "' AND Password='" + password + "';";
                command.CommandText = query;

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    admin.Id = (int)reader["id"];
                    admin.Name = (string)reader["namead"];
                    admin.Email = (string)reader["Email"];
                    admin.Address = (string)reader["Address"];
                    admin.Phone = (string)reader["phone"];
                    admin.Password = (string)reader["Password"];
                }
                string key = "MNU66iBl3T5rh6H52i69";
                string duration = "60";
                var symmetrickey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(symmetrickey, SecurityAlgorithms.HmacSha256);
                var claims = new[]
{
                    new Claim("id", admin.Id.ToString()),
                    new Claim("namead", admin.Name),
                    new Claim("address", admin.Address),
                    new Claim("phone", admin.Phone),
                    new Claim("email", admin.Email),
                };

                var jwtToken = new JwtSecurityToken(
                    issuer: "localhost",
                    audience: "localhost",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(Int32.Parse(duration)),
                    signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(jwtToken);
            }
            return "";
        }
        public List<Admin> GetAllAdmin()
        {
            List<Admin> admins = new List<Admin>();
            using (SqlConnection connection = new SqlConnection(dbconnection))
            {
                SqlCommand command = new SqlCommand
                {
                    Connection = connection
                };
                string query = "select *from admin";
                command.CommandText = query;

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Admin admin = new Admin
                    {
                        Id = (int)reader["id"],
                        Name = (string)reader["namead"],
                        Phone = (string)reader["Phone"],
                        Email = (string)reader["Email"],
                        Password = (string)reader["Password"],
                        Address = (string)reader["Address"],

                    };
                    admins.Add(admin);

                }

            }
            return admins;
        }
        #endregion

        //danh sách order
        public List<Order> GetOrders()
        {
            List<Order> orders = new List<Order>();
            using (SqlConnection connection = new SqlConnection(dbconnection))
            {
                SqlCommand command = new SqlCommand
                {
                    Connection = connection
                };
                string query = "select o.Id,us.FirstName,us.LastName,us.Email,us.Mobile ,pay.AmountPaid,pm.Provider,pay.CreatedAt from Orders o inner join Users us on o.UserId = us.UserId inner join Payments pay on pay.Id = o.PaymentId inner join PaymentMethods pm on pm.PaymentMethodId = pay.PaymentMethodId";

                command.CommandText = query;

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Order order = new Order
                    {
                        Id = (int)reader["id"],
                        User = new User
                        {
                            FirstName = (string)reader["FirstName"],
                            LastName = (string)reader["LastName"],
                            Email = (string)reader["email"],
                            Mobile = (string)reader["Mobile"],
                        },
                        Payment = new Payment
                        {
                            AmountPaid = (int)reader["AmountPaid"],
                            PaymentMethod = new PaymentMethod
                            {
                                Provider = (string)reader["Provider"],
                            },
                            CreatedAt = (string)reader["CreatedAt"],
                        },
                    };
                    orders.Add(order);

                }
            }
            return orders;
        }
        SqlConnection connection;
        public int UpdateExcu(string sql,CommandType commandType, params SqlParameter[] parameters)
        {
            connection = new SqlConnection(dbconnection);
            try
            {

               if(connection.State==ConnectionState.Open)
                {
                    connection.Close();
                }
                connection.Open();
                SqlCommand command = new SqlCommand
                {
                    Connection = connection
                };
                command.CommandType = commandType;
                command.CommandText = sql;
                if(parameters != null)
                {
                    foreach(SqlParameter param in parameters)
                    {
                        command.Parameters.Add(param);
                    }
                }
                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                return -1;
            }
            finally
            {
                connection.Close();
            }
            return 0;

        }

    }
}

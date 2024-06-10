import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Category, Offer, Order, Payment, PaymentMethod, Product, User } from '../models/models';
import {map} from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class NavigationService {
  baseUrl = "https://localhost:7149/api/Shopping/";
  baseAdmin = "https://localhost:7149/api/Admin/";
  baseProduct = "https://localhost:7149/api/Product/";
  baseUsers = "https://localhost:7149/api/User/";
  baseOder ="https://localhost:7149/api/Order/";

  constructor(private http: HttpClient) { }
  getCategoryList() {
    let url = this.baseUrl + 'GetCategoryList';
    return this.http.get<any[]>(url).pipe(
      map((categories) =>
        categories.map((category) => {
          let mappedCategory: Category = {
            id: category.id,
            category: category.category,
            subCategory: category.subCategory,
          };
          return mappedCategory;
        })
      )
    );
  }
  getOfferList(){
    let url = this.baseUrl + 'GetOfferList';
    return this.http.get<any[]>(url).pipe(
      map((offers) =>
      offers.map((offer) => {
          let mappedOffer: Offer = {
            id: offer.id,
            title: offer.title,
            discount : offer.discount
          };
          return mappedOffer;
        })
      )
    );
  }
  getProducts(category: string, subCategory: string, count: number) {
    return this.http.get<any[]>(this.baseProduct + 'GetProducts', {
      params: new HttpParams()
        .set('category', category)
        .set('subcategory', subCategory)
        .set('count', count),
    });
  }
  getProduct(id: number) {
    let url = this.baseProduct + 'GetProduct/' + id;
    return this.http.get(url);
  }
  registerUser(user: User) {
    let url = this.baseUsers + 'RegisterUser';
    return this.http.post(url, user, { responseType: 'text' });
  }
  loginUser(email: string, password: string) {
    let url = this.baseUsers + 'LoginUser';
    return this.http.post(
      url,
      { Email: email, Password: password },
      { responseType: 'text' }
    );
  }
  submitReview(userid: number, productid: number, review: string) {
    let obj: any = {
      User: {
        Id: userid,
      },
      Product: {
        Id: productid,
      },
      Value: review,
    };

    let url = this.baseUrl + 'InsertReview';
    return this.http.post(url, obj, { responseType: 'text' });
  }
  getAllReviewsOfProduct(productId: number) {
    let url = this.baseUrl + 'GetProductReviews/' + productId;
    return this.http.get(url);
  }
  addToCart(userid: number, productid: number) {
    let url = this.baseUrl + 'InsertCartItem/' + userid + '/' + productid;
    return this.http.post(url, null, { responseType: 'text' });
  }
  getActiveCartOfUser(userid: number) {
    let url = this.baseUrl + 'GetActiveCartOfUser/' + userid;
    return this.http.get(url);
  }
  getAllPreviousCarts(userid: number) {
    let url = this.baseUrl + 'GetAllPreviousCartsOfUser/' + userid;
    return this.http.get(url);
  }
  getPaymentMethods() {
    let url = this.baseUrl + 'GetPaymentMethods';
    return this.http.get<PaymentMethod[]>(url);
  }
  insertPayment(payment: Payment) {
    return this.http.post(this.baseUrl + 'InsertPayment', payment, {
      responseType: 'text',
    });
  }
  insertOrder(order: Order) {
    return this.http.post(this.baseUrl + 'InsertOrder', order);
  }
  deleteCartItem(id : number){
    return this.http.delete(this.baseUrl+ 'DeleteCartItem/'+ id)
  }
  createProduct(product: Product) {
    return this.http.post(this.baseProduct + 'CreateProduct', product);
  }
  getallProduct(){
    return this.http.get<Product>(this.baseProduct+'GetProductsFromQuery')
  }
  deleteProduct(id : number){
    return this.http.delete(this.baseProduct+'DeleteProduct/'+id)
  }
}


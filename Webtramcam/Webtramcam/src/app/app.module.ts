import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ProductComponent } from './product/product.component';
import { SuggestedProductsComponent } from './suggested-products/suggested-products.component';
import { HomeComponent } from './home/home.component';
import { ProductsComponent } from './products/products.component';
import { ProductDetailsComponent } from './product-details/product-details.component';
import { CartComponent } from './cart/cart.component';
import { OrderComponent } from './order/order.component';
import { ReactiveFormsModule } from '@angular/forms';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { OpenProductsDirective } from './directives/open-products.directive';
import { OpenProductDetailsDirective } from './directives/open-product-details.directive';
import { RegisterComponent } from './register/register.component';
import { LoginComponent } from './login/login.component';
import { JwtModule } from '@auth0/angular-jwt';
import { AdminHomeComponent } from './admin/admin-home/admin-home.component';
import { AccountComponent } from './account/account.component';
import { AdminComponent } from './admin/admin.component';
import { AdminOrderComponent } from './admin/admin-order/admin-order.component';
import { AdminProductComponent } from './admin/admin-product/admin-product.component';
import { AdminLoginComponent } from './admin/admin-login/admin-login.component';

@NgModule({
  declarations: [
    AppComponent,
    ProductComponent,
    SuggestedProductsComponent,
    HomeComponent,
    ProductsComponent,
    ProductDetailsComponent,
    CartComponent,
    OrderComponent,
    HeaderComponent,
    FooterComponent,
    PageNotFoundComponent,
    OpenProductsDirective,
    OpenProductDetailsDirective,
    RegisterComponent,
    LoginComponent,
    AdminHomeComponent,
    AccountComponent,
    AdminComponent,
    AdminOrderComponent,
    AdminProductComponent,
    AdminLoginComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    HttpClientModule,
    RouterModule,
    JwtModule.forRoot({
      config:{
        tokenGetter:()=>{
          return localStorage.getItem('user');
        },
        allowedDomains:['localhost:7149'],
      },
    }),
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }

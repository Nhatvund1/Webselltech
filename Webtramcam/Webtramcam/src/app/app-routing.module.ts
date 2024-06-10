import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { ProductComponent } from './product/product.component';
import { ProductDetailsComponent } from './product-details/product-details.component';
import { CartComponent } from './cart/cart.component';
import { OrderComponent } from './order/order.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { ProductsComponent } from './products/products.component';
import { AdminHomeComponent } from './admin/admin-home/admin-home.component';
import { AdminOrderComponent } from './admin/admin-order/admin-order.component';
import { AdminLoginComponent } from './admin/admin-login/admin-login.component';
const routes: Routes = [
  {path:'home',component:HomeComponent},
  {path:'products',component:ProductsComponent},
  {path:'product-details',component:ProductDetailsComponent},
  {path:'cart',component:CartComponent},
  {path:'orders',component:OrderComponent},
  {path:'admin',component:AdminLoginComponent},
  {path:'admin/home',component:AdminHomeComponent},
  {path:'admin/order',component:AdminOrderComponent},
  {path:'admin/login',component:AdminLoginComponent},
  {path:'',redirectTo:'/home',pathMatch:'full'},
  {path:'**',component:PageNotFoundComponent},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

import { Component,OnInit } from '@angular/core';
import { UtilityServiceService } from '../services/utility-service.service';
import { NavigationService } from '../services/navigation.service';
import { Cart, Payment } from '../models/models';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit{
  usersCart: Cart = {
    id: 0,
    user: this.utilityService.getUser(),
    cartItems: [],
    ordered: false,
    orderedOn: '',
  };
  usersPaymentInfo: Payment = {
    id: 0,
    user: this.utilityService.getUser(),
    paymentMethod: {
      id: 0,
      type: '',
      provider: '',
      available: false,
      reason: '',
    },
    totalAmount: 0,
    shipingCharges: 0,
    amountReduced: 0,
    amountPaid: 0,
    createdAt: '',
  };

  usersPreviousCarts: Cart[] = [];
  constructor(
    public utilityService:UtilityServiceService,
    private navigationService:NavigationService
  ){} 
  ngOnInit(): void {
    // Get Cart
    this.navigationService
      .getActiveCartOfUser(this.utilityService.getUser().id) // Lấy thông tin giỏ hàng hiện tại của người dùng
      .subscribe((res: any) => {
        this.usersCart = res;

        // Calculate Payment
        this.utilityService.calculatePayment(
          this.usersCart,
          this.usersPaymentInfo
        );
      });

    // Get Previous Carts
    this.navigationService
      .getAllPreviousCarts(this.utilityService.getUser().id) // lấy danh sách các giỏ hàng trước đó của người dùng 
      .subscribe((res: any) => {
        this.usersPreviousCarts = res;
      });
  }
}

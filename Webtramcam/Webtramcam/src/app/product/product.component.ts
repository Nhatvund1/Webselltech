import { Component ,Input,OnInit} from '@angular/core';
import { Product } from '../models/models';
import { UtilityServiceService } from '../services/utility-service.service';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.css']
})
export class ProductComponent implements OnInit{
  @Input() view: 'grid' | 'list' |'currcartitem' |'prevcartitem' = 'grid';
  @Input() product: Product = {
    id: 0,
    title: '',
    description: '',
    price: 0,
    quantity: 0,
    productCategory: {
      id: 1,
      category: '',
      subCategory: '',
    },
    offer: {
      id: 1,
      title: '',
      discount: 0,
    },
    imageName: '',
  };
  constructor(
    public utilityService : UtilityServiceService
  ){}
  ngOnInit(): void {

  }
  removeFromCart(productId: number) {
    this.utilityService.deletecartitems(productId).subscribe((res) => {
      if (res === 'deleted') {
        // Handle success, maybe update the UI or display a message
        this.utilityService.changeCart.next(productId);
      } else {
        // Handle error
        console.log('Failed to remove product from cart');
      }
    });
  }

}

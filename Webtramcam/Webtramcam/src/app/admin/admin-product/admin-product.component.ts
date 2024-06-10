import { Component, Input, OnInit } from '@angular/core';
import { Product } from 'src/app/models/models';
import { UtilityServiceService } from 'src/app/services/utility-service.service';

@Component({
  selector: 'app-admin-product',
  templateUrl: './admin-product.component.html',
  styleUrls: ['./admin-product.component.css']
})
export class AdminProductComponent implements OnInit{
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
  removeProduct(id: number) {
    this.utilityService.deleteproductad(id).subscribe((res) => {
      if (res === 'deleted') {
        console.log('Successfully removed product');
      } else {
        console.log('Failed to remove product');
      }
    });
  }

}

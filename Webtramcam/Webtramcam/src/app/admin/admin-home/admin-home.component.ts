import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Category, NavigationItem, Offer, Product } from 'src/app/models/models';
import { NavigationService } from 'src/app/services/navigation.service';

@Component({
  selector: 'app-admin-home',
  templateUrl: './admin-home.component.html',
  styleUrls: ['./admin-home.component.css']
})
export class AdminHomeComponent implements OnInit, AfterViewInit{
  @ViewChild('tempButton') buttontemp:any;
  title = 'ProductCRUD';
  
  products: Product[] ;
  productsDisplay: Product[];
  
  navigationList: Category[] = [];
  offerList: Offer[] = []; 
  productForm: FormGroup = new FormGroup({
    title : new FormControl(''),
    description : new FormControl(''),
    CategoryId : new FormControl(''),
    offerId : new FormControl(''),
    price : new FormControl(''),
    quantity : new FormControl(''),
    imageName : new FormControl(''),
  })  ;

  constructor(
    private navigationService: NavigationService,private activatedRoute: ActivatedRoute,){
      this.products = [];
      this.productsDisplay = this.products;
    }
  ngOnInit(): void {
    this.loadCategories();
    this.loadOffers();
    
    this.navigationService.getallProduct().subscribe((res : any)=>{
      for(let emp of res){
        this.products.unshift(emp);
      }
      this.productsDisplay =this.products;
    })
  }
  loadCategories() {
    this.navigationService
      .getCategoryList().subscribe((list: Category[])=>{
        this.navigationList = list;
      })
  }
  loadOffers() {
    this.navigationService.getOfferList().subscribe((list: Offer[]) => {
      this.offerList = list;
    });
  }
  onCategoryChange(event: any) {
    const selectedSubcategory = event.target.value;
    console.log(`Selected Subcategory: ${selectedSubcategory}`);
  }
  ngAfterViewInit(): void {
    // this.buttontemp.nativeElement.click();
  }
  addProduct() {
    if (this.productForm.valid) {
      // Create a new Product object with form data
      const productData: Product = {
        id: 0, // You may need to adjust this ID depending on how it's managed on the server.
        title: this.productForm.get('title')?.value,
        description: this.productForm.get('description')?.value,
        productCategory: {
          id: this.productForm.get('CategoryId')?.value, // Adjust the category ID as needed.
          category: '',
          subCategory: '', // You can add the subcategory here if it's available in your form.
        },
        offer: {
          id: this.productForm.get('offerId')?.value,
          title: '', // Set the title based on the selected offer.
          discount: 0, // Set the discount based on the selected offer.
        },
        price: this.productForm.get('price')?.value,
        quantity: this.productForm.get('quantity')?.value,
        imageName: this.productForm.get('imageName')?.value,
      };
  
      // Call the service to create the product
      this.navigationService.createProduct(productData).subscribe(
        (result) => {
          console.log('Product created successfully', result);
          this.productForm.reset();
          // Reload categories and offers after adding a new product if necessary
          this.loadCategories();
          this.loadOffers();
        },
        (error) => {
          console.error('Error creating product', error);
        }
      );
    } else {
      console.log('Invalid form data');
    }
  }
  onSubmit(){
    console.log(this.productForm.value);
  }
}

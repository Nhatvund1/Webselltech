import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Product, Review } from '../models/models';
import { NavigationService } from '../services/navigation.service';
import { UtilityServiceService } from '../services/utility-service.service';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.css'],
})
export class ProductDetailsComponent implements OnInit {
  imageIndex: number = 1;
  product!: Product;
  reviewControl = new FormControl('');
  showError = false;
  reviewSaved = false;
  otherReviews: Review[] = [];

  constructor(
    private activatedRoute: ActivatedRoute,
    private navigationService: NavigationService,
    public utilityService: UtilityServiceService
  ) {}

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe((params: any) => {
      let id = params.id;
      this.navigationService.getProduct(id).subscribe((res: any) => {
        this.product = res;
        this.fetchAllReviews();
      });
    });

  }

  submitReview() {
    let review = this.reviewControl.value;
    let checkcm = ["địt","fuck","mẹ","lồn","cc","đéo","buồi","bố","cặc","chó","đụ","đĩ","nứng","lol"];
    function check(review: string , checkcm:string[]):boolean {
      for (const char of checkcm) {
        if (review.includes(char)) {
          return true;
        }
      }
      return false;
    }
      if (review === '' || review === null || check(review,checkcm)) {
        this.showError = true;
          return;
      }
      let userid = this.utilityService.getUser().id;
      let productid = this.product.id;

      this.navigationService
        .submitReview(userid, productid, review)
        .subscribe((res) => {
          this.reviewSaved = true;
          this.fetchAllReviews();
          this.reviewControl.setValue('');
        });
    
  }


  fetchAllReviews() {
    this.otherReviews = [];
    this.navigationService
      .getAllReviewsOfProduct(this.product.id)
      .subscribe((res: any) => {
        for (let review of res) {
          this.otherReviews.push(review);
        }
      });
  }

}

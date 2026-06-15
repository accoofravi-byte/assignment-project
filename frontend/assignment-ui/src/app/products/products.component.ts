import { Component, OnInit } from '@angular/core';
import { ProductService } from '../services/product.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ChangeDetectorRef } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

export interface Product {
  id?: number;
  name: string;
  price: number;
  quantity: number;
}

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './products.html',
  styleUrl: './products.css',
})
export class ProductsComponent implements OnInit {
products: Product[] = [];
product: Product = {
    name: '',
    price: 0,
    quantity: 0
  };

constructor(
  private productService: ProductService,
  private cdr: ChangeDetectorRef,
  private auth: AuthService,
  private router: Router
) {}

ngOnInit(): void {
  if (this.auth.getToken()) {
    this.router.navigate(['/products']);
  }
  console.log('RAVI DEPLOY TEST V3');
  this.loadProducts();
}

loadProducts() {
  console.log('Calling API...');

  this.productService.getProducts().subscribe({
    next: (data) => {
      console.log('Received:', data);

      this.products = data;

      this.cdr.detectChanges();

      console.log('Products array:', this.products);

      setTimeout(() => {
        console.log('Length after assignment:', this.products.length);
      }, 1000);
    },

    error: (err) => {
      console.error('API ERROR:', err);
    }
  });
}

save() {
  if(this.product.id) {
    this.productService.updateProduct(
      this.product.id, 
      this.product)
    .subscribe(() => {
      this.loadProducts();
      this.resetForm();
    });
  }
    else {
      this.productService.createProduct(
        this.product
      ).subscribe(() => {
        this.loadProducts();
        this.resetForm();
      });
    }
}

edit(product: any) {
  this.product = { ...product };
}

delete(id: number) {
  this.productService.deleteProduct(id)
  .subscribe(() => {
    this.loadProducts();
      console.log('Products after reload:', this.products);
  });
}

resetForm() {
  this.product = {
    id: undefined,
    name: '',
    price: 0,
    quantity: 0
  };
}

logout() {
  this.auth.logOut();
  this.router.navigate(['/']);
}
}

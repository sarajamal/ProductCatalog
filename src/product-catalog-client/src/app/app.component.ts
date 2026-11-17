import { Component, ViewChild } from '@angular/core';
import { ProductListComponent } from './components/product-list/product-list.component';
import { ProductFormComponent } from './components/product-form/product-form.component';
import { Product } from './models/product.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [ProductListComponent, ProductFormComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'Product Catalog';
  selectedProduct: Product | null = null;

  @ViewChild(ProductListComponent) productList!: ProductListComponent;

  onEdit(product: Product): void {
    this.selectedProduct = product;
  }

  onSaved(): void {
    this.selectedProduct = null;
    this.productList.load();
  }

  onCancelled(): void {
    this.selectedProduct = null;
  }
}

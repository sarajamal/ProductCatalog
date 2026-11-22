import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Product } from '../../models/product.model';
import { ProductService } from '../../services/product.service';

// Products with stock below this number are shown as low stock
export const LOW_STOCK_THRESHOLD = 20;

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.css'
})
export class ProductListComponent implements OnInit {
  products: Product[] = [];
  loading = false;
  errorMessage = '';
  readonly lowStockThreshold = LOW_STOCK_THRESHOLD;

  @Output() edit = new EventEmitter<Product>();

  constructor(private productService: ProductService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.errorMessage = '';
    this.productService.getAll().subscribe({
      next: (data) => {
        // Lowest stock first, so products that need restocking appear at the top
        this.products = [...data].sort((a, b) => a.stock - b.stock);
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'تعذّر تحميل المنتجات. تأكدي أن الـ API شغّال على localhost:5233.';
        this.loading = false;
      }
    });
  }

  isLowStock(product: Product): boolean {
    return product.stock < this.lowStockThreshold;
  }

  get lowStockCount(): number {
    return this.products.filter((p) => this.isLowStock(p)).length;
  }

  onEdit(product: Product): void {
    this.edit.emit(product);
  }

  onDelete(product: Product): void {
    if (!confirm(`حذف "${product.productName}"؟`)) {
      return;
    }
    this.productService.delete(product.productId).subscribe({
      next: () => this.load(),
      error: () => (this.errorMessage = 'تعذّر حذف المنتج.')
    });
  }
}

import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { Category, Product } from '../../models/product.model';
import { ProductService } from '../../services/product.service';
import { CategoryService } from '../../services/category.service';

// Form model: fields can be empty (null) until the user fills them in
interface ProductFormModel {
  productId?: number;
  productName: string;
  price: number | null;
  stock: number | null;
  categoryId: number | null;
}

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './product-form.component.html',
  styleUrl: './product-form.component.css'
})
export class ProductFormComponent implements OnInit, OnChanges {
  @Input() productToEdit: Product | null = null;
  @Output() saved = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  @ViewChild('f') form?: NgForm;

  categories: Category[] = [];
  model: ProductFormModel = this.emptyModel();
  errorMessage = '';

  constructor(
    private productService: ProductService,
    private categoryService: CategoryService
  ) {}

  ngOnInit(): void {
    // No category is pre-selected: the user must choose one ("اختر الفئة المناسبة")
    this.categoryService.getAll().subscribe({
      next: (data) => (this.categories = data),
      error: () => (this.errorMessage = 'تعذّر تحميل الفئات.')
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['productToEdit']) {
      this.model = this.productToEdit ? { ...this.productToEdit } : this.emptyModel();
      this.resetFormState();
    }
  }

  get isEditing(): boolean {
    return !!this.model.productId;
  }

  save(): void {
    this.errorMessage = '';

    // Show every field's message if something is missing or invalid
    if (this.form?.invalid) {
      this.form.control.markAllAsTouched();
      return;
    }

    // New products must have a price and stock greater than 0
    if (!this.isEditing && (!(Number(this.model.price) > 0) || !(Number(this.model.stock) > 0))) {
      this.errorMessage = 'السعر والمخزون لازم يكونون أكبر من 0 عند الإضافة.';
      return;
    }

    const payload = this.model as unknown as Partial<Product>;
    if (this.isEditing) {
      this.productService.update(this.model.productId!, payload).subscribe({
        next: () => this.finish(),
        error: () => (this.errorMessage = 'تعذّر حفظ التعديلات.')
      });
    } else {
      this.productService.create(payload).subscribe({
        next: () => this.finish(),
        error: () => (this.errorMessage = 'تعذّر إضافة المنتج.')
      });
    }
  }

  cancel(): void {
    this.model = this.emptyModel();
    this.errorMessage = '';
    this.resetFormState();
    this.cancelled.emit();
  }

  private finish(): void {
    this.model = this.emptyModel();
    this.resetFormState();
    this.saved.emit();
  }

  // Clear "touched" state so the red messages don't show on a fresh form
  private resetFormState(): void {
    this.form?.control.markAsPristine();
    this.form?.control.markAsUntouched();
  }

  private emptyModel(): ProductFormModel {
    return { productName: '', price: null, stock: null, categoryId: null };
  }
}

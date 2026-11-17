import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product } from '../models/product.model';

// نفس الشكل اللي شغالة فيه ProductsController عندك في الباك اند:
// GET /api/products  |  GET /api/products/5  |  POST  |  PUT /5  |  DELETE /5
const API_URL = 'http://localhost:5233/api/products';

@Injectable({
  providedIn: 'root'
})

//Observable
//الدوال هذي ما ترسل الطلب لحالها، بس ترجّع Observable. الطلب ما ينرسل فعلياً إلا لما الـ Component يسوي .subscribe().
export class ProductService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<Product[]> {
    return this.http.get<Product[]>(API_URL);
  }

  getById(id: number): Observable<Product> {
    return this.http.get<Product>(`${API_URL}/${id}`);
  }

  create(product: Partial<Product>): Observable<Product> {
    return this.http.post<Product>(API_URL, product);
  }

  update(id: number, product: Partial<Product>): Observable<void> {
    return this.http.put<void>(`${API_URL}/${id}`, product);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${API_URL}/${id}`);
  }
}

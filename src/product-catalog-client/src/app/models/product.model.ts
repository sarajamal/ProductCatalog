//الحقول هنا نفس أسماء خصائص كلاس Product في C#
// ، لكنها مكتوبة بـ camelCase لأن .NET يحوّل الأسماء لهذا الشكل في JSON تلقائياً.
export interface Category {
  categoryId: number;
  categoryName: string;
}

export interface Product {
  productId: number;
  productName: string;
  price: number;
  stock: number;
  categoryId: number;
  category?: Category;
}

/* { "productId": 7, "productName": "Laptop", "price": 3500, "stock": 10,
  "categoryId": 1, "category": { "categoryId": 1, "categoryName": "Electronics" } }*/

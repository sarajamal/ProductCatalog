# دليل الـ MCP Server — مشروع ProductCatalog

> المرحلة 2 (أدوات القراءة) — آخر تحديث: 2026-11-25

---

## 1. المشكلة

عندك تطبيق ProductCatalog فيه منتجات وفئات.
لو بغيتي تعرفين **"وش المنتجات اللي مخزونها قليل؟"** لازم:

- تفتحين التطبيق في المتصفح
- تدورين في الجدول بنفسك

ولو سألتي Claude نفس السؤال، ما يقدر يجاوب، لأنه **ما يقدر يوصل لبيانات مشروعك**.

---

## 2. الحل

**MCP Server**

برنامج صغير بـ C# يقف بين Claude والـ API حقك، ويعطي Claude **أدوات** يستخدمها عشان يجيب البيانات الحقيقية.

**MCP = Model Context Protocol**

طريقة موحّدة تخلي الـ AI يستخدم برامجنا كأدوات.

| الحرف | الكلمة | المعنى |
|---|---|---|
| M | Model | الـ AI (مثل Claude) |
| C | Context | المعلومات اللي يوصل لها |
| P | Protocol | الطريقة الموحّدة للربط |

---

## 3. المثال اللي بنمشي عليه في كل الدليل

> **"وش المنتجات اللي مخزونها أقل من 20؟"**

---

## 4. كيف يشتغل (الرسمة)

```
أنتِ / Claude
     │  "وش المنتجات اللي مخزونها قليل؟"
     ▼
┌──────────────────────┐
│     MCP Server        │   src\McpServer
│  (الأداة list_products)│
└──────────┬───────────┘
           │  GET http://localhost:5233/api/products
           ▼
┌──────────────────────┐
│  ProductCatalog API   │   src\ProductCatalogApi
└──────────┬───────────┘
           │  EF Core
           ▼
┌──────────────────────┐
│     SQL Server        │   جدول Products
└──────────────────────┘
           │
           ▼
   يرجع: Mechanical Keyboard (16)، Product3 (19) ...
```

يعني الـ MCP Server **ما يكلّم قاعدة البيانات مباشرة**، يكلّم الـ API بس، مثل ما تسوي واجهة Angular.

---

## 5. الملفات في المشروع

```
src\McpServer\
├── McpServer.csproj          ← المشروع: .NET 8 + حزمة ModelContextProtocol 2.4.0
├── Program.cs                ← يشغّل الـ server ويحدد رابط الـ API
├── ProductCatalogClient.cs   ← الكلاس اللي يكلّم الـ API (كل الأدوات تمر عليه)
├── Models\Product.cs         ← شكل المنتج والفئة
└── Tools\CatalogReadTools.cs ← الأدوات الخمس
```

### الفكرة في الكود (مختصرة)

**Program.cs**

```csharp
builder.Services
    .AddMcpServer()               // سجّل MCP Server
    .WithStdioServerTransport()   // يتكلم مع Claude عن طريق stdin/stdout
    .WithToolsFromAssembly();     // يلقى كل الأدوات المعرّفة في المشروع
```

**Tools\CatalogReadTools.cs**

```csharp
[McpServerTool(Name = "list_products"),
 Description("Lists all products ... sorted by stock (lowest first).")]
public static Task<string> ListProducts(ProductCatalogClient api, CancellationToken ct) => ...
```

- **`[McpServerTool]`**: يقول للـ MCP إن هذي الدالة أداة.
- **`Description`**: الوصف اللي يقرأه Claude عشان يعرف متى يستخدم الأداة. **هذا مهم جداً**.
- **`ProductCatalogClient api`**: الـ MCP يعطيه للدالة تلقائياً (Dependency Injection، نفس فكرة .NET).

---

## 6. الأدوات الخمس

| الأداة | المدخل | وش ترجع | مثال |
|---|---|---|---|
| `list_categories` | — | كل الفئات | Electronics, Books, Clothing |
| `list_products` | — | كل المنتجات، من الأقل مخزون | Mechanical Keyboard (16) أول وحدة |
| `get_product` | `productId` (رقم) | منتج واحد، أو "No product found" | `2` ← Mechanical Keyboard |
| `get_products_by_category` | `categoryId` (رقم) | منتجات فئة وحدة | `2` ← كتب |
| `search_products` | `query` (نص) | المنتجات اللي اسمها فيه النص | `key` ← Mechanical Keyboard |

**إذا الـ API طافي:** الأداة ما تنهار، ترجع رسالة واضحة:

> Could not reach ProductCatalogApi ... Make sure the API is running (scripts\start-api.bat).

---

## 7. التشغيل والتجربة (خطوة خطوة)

### الخطوة 1: شغّلي الـ API

دبل كلك على:

```
C:\Users\Lenovo\ProductCatalogApi\scripts\start-api.bat
```

انتظري لين يطلع:

```
Now listening on: http://0.0.0.0:5233
```

### الخطوة 2: (أول مرة أو بعد أي تعديل) ابني الـ MCP Server

دبل كلك على:

```
scripts\build-mcp.bat
```

لازم يطلع:

```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

(النتيجة محفوظة بعد في `.logs\mcp-build.log`)

### الخطوة 3: افتحي صفحة التجربة

**MCP Inspector**

صفحة في المتصفح تجربين فيها الأدوات بيدك، قبل ما نربطها بـ Claude.

دبل كلك على:

```
scripts\test-mcp.bat
```

انتظري ~30 ثانية. المتصفح يفتح لحاله.
إذا ما فتح: افتحي `.logs\mcp-inspector.log` وانسخي الرابط اللي يبدأ بـ:

```
http://127.0.0.1:6274?MCP_INSPECTOR_API_TOKEN=...
```

### الخطوة 4: وش تشوفين في الصفحة

```
┌─────────────────────────────────────────────────┐
│  MCP Inspector                [Servers] [Tools] │
├─────────────────────────────────────────────────┤
│  ┌───────────────────────────────┐              │
│  │ dotnet     ● Disconnected [○] │ ← اضغطي هنا  │
│  │ STDIO                          │              │
│  │ dotnet run --project src\...   │              │
│  └───────────────────────────────┘              │
└─────────────────────────────────────────────────┘
```

1. اضغطي المفتاح جنب **Disconnected** ← يصير **Connected** (أخضر)
2. اضغطي **Tools** فوق

```
┌────────────────┬──────────────────────────┐
│ Tools (5)      │ list_products            │
│                │ Lists all products ...   │
│ get_product    │                          │
│ get_products_… │ [ Execute Tool ]         │
│ list_categories│                          │
│ list_products ◄│ Results:                 │
│ search_products│ [ { "productName": ... } │
└────────────────┴──────────────────────────┘
```

3. اختاري أداة من اليسار
4. إذا تطلب مدخل (رقم أو نص) اكتبيه
5. **Execute Tool** ← النتيجة تطلع في **Results**

### الخطوة 5: جربي المثال

1. اختاري `list_products` ← **Execute Tool**
2. شوفي أول المنتجات: هذي اللي مخزونها قليل (مترتبة من الأقل)

### للإيقاف

سكّري النافذتين السوداء: `ProductCatalogApi` و `mcp-inspector`.

---

## 8. مشاكل شائعة

| المشكلة | السبب | الحل |
|---|---|---|
| الأداة ترجع "Could not reach ProductCatalogApi" | الـ API طافي | شغّلي `start-api.bat` |
| الصفحة ما فتحت | Inspector لسا يحمّل، أو المتصفح ما انفتح | انتظري، أو خذي الرابط من `.logs\mcp-inspector.log` |
| المفتاح ما يصير Connected | الـ MCP Server ما انبنى، أو فيه خطأ | شغّلي `build-mcp.bat` وشوفي `.logs\mcp-build.log` |
| `address already in use` | فيه API شغال من قبل | سكّري النافذة القديمة، أو لا تشغّلين `start-api` و `start-dev` مع بعض |

---

## 9. وش الجاي

| # | المرحلة | الحالة |
|---|---|---|
| 1 | الهيكل و Git | ✓ |
| 2 | أدوات القراءة (هذا الدليل) | ✓ |
| 3 | الربط مع Claude Desktop: تسألين Claude مباشرة بدل الـ Inspector | الجاي |
| 4 | أدوات الكتابة: `create_product`, `update_stock`, `low_stock_report` | لاحقاً |
| 5 | Unit Tests بـ xUnit | لاحقاً |
| 6 | التوثيق النهائي | لاحقاً |

بعد المرحلة 3 بيصير الاستخدام كذا:

```
أنتِ في Claude: "وش المنتجات اللي مخزونها أقل من 20؟"
Claude: يستدعي list_products ← يقرأ النتيجة ← يرد: "Mechanical Keyboard (16) و Product3 (19)"
```

---

## 10. قاموس المصطلحات

**MCP (Model Context Protocol)**
طريقة موحّدة تخلي الـ AI يستخدم برامجنا كأدوات.

**MCP Server**
البرنامج اللي يعرض الأدوات (عندنا: `src\McpServer`).

**MCP Client**
البرنامج اللي يستخدم الأدوات (Claude Desktop، أو MCP Inspector للتجربة).

**Tool**
دالة يقدر الـ AI يستدعيها، لها اسم ووصف ومدخلات.

**stdio (Standard Input/Output)**
طريقة الاتصال: الـ Client يشغّل الـ Server ويتكلمون عن طريق الإدخال والإخراج النصي، بدون منفذ شبكة. عشان كذا الـ logs تروح لـ stderr، لأن stdout محجوز للرسائل.

**MCP Inspector**
صفحة تجربة رسمية من MCP، تستدعين منها الأدوات بيدك.

**Typed HttpClient**
كلاس (`ProductCatalogClient`) فيه `HttpClient` جاهز برابط الـ API، يُعطى للأدوات تلقائياً.

# CLAUDE.md — تعليمات العمل على مشروع ProductCatalog

أنت تعمل كـ Software Developer كامل الصلاحية على هذا المشروع، ولست مجرد مستشار.

## معلومات المشروع

- **الباك اند:** مشروع ASP.NET Core Web API اسمه `ProductCatalogApi`، موجود في:
  `src/ProductCatalogApi`
  يستخدم Entity Framework Core + SQL Server، فيه Controllers لـ Products و Categories.
  يشتغل على: http://localhost:5233 (و Swagger على /swagger)
- **الفرونت اند:** تطبيق Angular 18 (standalone) اسمه `product-catalog-client`، موجود في:
  `src/product-catalog-client`
  يشتغل على: http://localhost:4200
- **MCP Server:** خدمة C# في `src/McpServer` تغلّف الـ API عشان Claude Desktop يستخدمها (أدوات قراءة وكتابة) — التوثيق في `services/McpServer/`.
- **تشغيل الاثنين معاً:** `scripts/start-dev.bat` يشغّل الباك اند والفرونت اند في نافذتين.
  وفيه أيضاً: `scripts/start-api.bat` و `scripts/open-in-chrome.bat` و `scripts/open-in-vscode.bat`.
- **قاعدة البيانات:** كلمة المرور / Connection String في user secrets وليست في الملفات
  (انظر `src/ProductCatalogApi/appsettings.Development.example.json`).
- **جذر المستودع على الجهاز:** `C:\Users\Lenovo\ProductCatalogApi`
- هذا المشروع هو الأداة لتحقيق الهدف التقييمي السنوي: "Developing Web Application Through Vibe Coding"
  (ويشمل: Backend .NET + Frontend Angular، استخدام أوضاع مختلفة من الذكاء الاصطناعي (Planning/Editing)، و Unit Tests).

## بنية المستودع (Knowledge Base)

| المجلد | المحتوى |
|---|---|
| `src/` | الكود |
| `tests/` | Unit Tests (xUnit) |
| `services/<Service>/` | srs, plan, bootstrap, gaps, enhancements, qa, evidence, reviews لكل خدمة |
| `standards/` | معايير الكود والـ API والـ commits والـ branches |
| `templates/` | قوالب المستندات |
| `lessons/` | الدروس المستفادة `L-YYYYMMDD-short-name` |
| `kb.json` | المشاريع + المرحلة الحالية + owner لكل خدمة |

قبل أي مهمة: اقرأ `kb.json` لمعرفة المرحلة الحالية، ثم ملفات الخدمة المعنية في `services/`.
بعد أي مهمة: حدّث ملفات الخدمة (plan / qa / evidence / gaps) والدروس إذا فيه درس مستفاد.

## طريقة العمل المطلوبة (التزم بها دائماً)

1. ادخل لملفات المشروع على الجهاز مباشرة واعمل عليها (مو شرح فقط).
2. افهم بنية المشروع والتقنيات المستخدمة قبل أي تعديل.
3. أي تعديل أو إضافة أو إصلاح مطلوب، نفّذه بنفسك مباشرة على الملفات — لا تكتفِ بإعطاء Code Snippets.
4. بعد كل تعديل، شغّل/أعد تشغيل المشروع عند الحاجة، وتحقق من النتيجة عن طريق متصفح Chrome الحقيقي من خلال امتداد Claude in Chrome — وليس أي متصفح خاص (built-in browser). لو الامتداد غير متصل، قل ذلك واطلب التأكد إنه مفعّل ومسجّل دخول قبل ما تكمل.
5. إذا ظهرت أخطاء أثناء التشغيل أو التطوير، حلّلها وأصلحها بنفسك.
6. حافظ على الكود الحالي وبنية المشروع، ولا تحذف أو تغيّر أي وظيفة موجودة بدون داعٍ.
7. قبل أي تغيير كبير يؤثر على بنية المشروع أو قاعدة البيانات أو البيانات الحالية، اشرح التغيير وخذ الموافقة أولاً.
8. في نهاية كل تعديل، أخبر بالتحديد: ما هي الملفات اللي تعدّلت، وش اللي تغيّر فيها بالضبط، ونتيجة الاختبار/التحقق.
9. التواصل بالعربي، مع شرح المصطلحات التقنية الإنجليزية بشكل مبسّط (المطوّرة متمكنة من .NET لكن جديدة على Angular)، والمصطلحات الإنجليزية في سطر مستقل.

## قواعد Git

- لا تعمل مباشرة على `main`: افتح فرع `feature/...` أو `fix/...` أو `kb/...` (انظر `standards/branching.md`).
- صيغة الـ commit: `feat(Scope): ...` / `fix(Scope): ...` / `kb(Scope): ...` (انظر `standards/commit-convention.md`).
- لا ترفع أسرار (connection strings, passwords) — استخدم user secrets.
- ملفات `.bat` تبقى بنهايات أسطر CRLF.

## GitHub: مين يسوي وش (من 2026-11-27)

| الخطوة | مين |
|---|---|
| فرع + تعديل + تحقق + commit | Claude |
| `git push` للفرع (مو `main` أبداً، وبدون force push) | Claude |
| فتح الـ Pull Request: العنوان والوصف من `.github/pull_request_template.md` (في Chrome لأن `gh` غير مثبّت) | Claude |
| متابعة الـ CI، وإصلاح أي فشل على نفس الفرع | Claude |
| **Squash and merge إلى `main`** | **Claude، بعد موافقة سارة الصريحة على كل PR** — قبل الدمج يكتب لها: وش تغيّر بالضبط، ووش بيصير بعد الدمج، وحالة الـ CI، ويسألها «أدمج؟». بدون «نعم» منها ما يدمج |
| بعد الدمج: `git switch main` و `git pull` وحذف الفرع المحلي | Claude (مباشرة بعد الدمج) |

- لا ترفع ملفات غير متتبعة ما لها علاقة بالتغيير.
- أدوات الكتابة في MCP على البيانات الحقيقية تحتاج موافقة صريحة كل مرة.

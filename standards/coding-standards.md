# Coding Standards

## C# / .NET

- Target **.NET 8**, `Nullable` and `ImplicitUsings` enabled.
- File-scoped namespaces: `namespace ProductCatalogApi.Controllers;`
- Naming: `PascalCase` for types, methods, properties; `camelCase` for locals/parameters; `_camelCase` for private fields; interfaces start with `I`.
- One public type per file; file name = type name.
- Use `async`/`await` end-to-end for I/O (EF Core, HTTP). Async methods end with `Async` (except controller actions).
- Inject dependencies through constructors; no `new` for services.
- Validate input at the boundary (controllers / MCP tools) and return `ValidationProblem` / `BadRequest`, never throw for expected validation failures.
- Never hard-code secrets or connection strings — use configuration + user secrets.
- Keep controllers thin; move non-trivial logic into services so it can be unit tested.
- Formatting follows the root `.editorconfig` (`dotnet format` before committing).

## TypeScript / Angular

- Angular 18 **standalone** components; one component per folder (`.ts`, `.html`, `.css`, `.spec.ts`).
- Services are `providedIn: 'root'` and return `Observable<T>`; components `subscribe` (or use the `async` pipe).
- Models in `src/app/models`, services in `src/app/services`, components in `src/app/components`.
- Strong typing — avoid `any`.
- API base URLs in one place (environment file or service constant), not scattered.
- 2-space indentation, single quotes (Angular CLI defaults).

## Tests

- .NET: xUnit, project per service under `tests/` (`<Service>.Tests`). Name: `Method_Scenario_ExpectedResult`.
- Angular: Jasmine/Karma specs next to the code (`*.spec.ts`).

## Comments

- Explain *why*, not *what*. Arabic learning comments are allowed in the client while learning, but keep them short.

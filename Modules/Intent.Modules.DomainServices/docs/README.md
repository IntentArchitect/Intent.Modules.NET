# Intent.DomainServices

This module realises `Domain Services` designed in the _Domain Designer_ in Intent Architect as interfaces and implementations for C#.

## What Are Domain Services?

Domain Services are a key pattern in Domain-Driven Design (DDD) that encapsulate business logic which doesn't naturally belong to any single domain entity or value object. They represent operations or workflows that span across entities or require external dependencies, while maintaining a focus on the ubiquitous language of your domain.

In the context of Intent Architect, Domain Services are:

- **Stateless operations** that orchestrate business logic across domain boundaries
- **Cohesive units** grouped by domain responsibility, expressed as interfaces
- **Automatically injected** into your application layer where they can be consumed by application services, handlers, or other components

## Why Use Domain Services?

Domain Services provide several critical benefits:

- **Separation of Concerns**: Keep domain logic separate from infrastructure and application concerns. Your domain layer remains focused on expressing the rules of your business, not on how those rules are technically implemented or consumed.
- **Rich Domain Model**: Rather than anemic domain models where entities hold only data, Domain Services allow you to express complex, multi-entity operations using the language and concepts of your domain. This makes your codebase more maintainable and aligned with stakeholder understanding.
- **Reusability and Consistency**: By centralizing business logic in Domain Services, you ensure that critical operations are defined once and reused consistently across your application—whether called from an API handler, a scheduled job, or an event subscriber.
- **Testability**: Domain Services can be easily unit tested in isolation, validating business rules without needing to set up an entire application context.


### Real-World Example: Product Category Assignment

Consider an e-commerce domain where you need to assign a product to a category. This operation involves:

1. **Multiple Aggregate Roots**: `Product` and `Category` are separate aggregates with independent lifespans and consistency boundaries
2. **Cross-Aggregate Business Rules**: Validation rules like "category must be active," "product cannot exceed category limit," and "category restrictions" span both aggregates
3. **Bidirectional Consistency**: Changes must be coordinated in both aggregates to maintain domain invariants

Rather than placing this logic on either entity, you define a stateless `IProductCategoryService`:

```csharp
public interface IProductCategoryService
{
    void AssignProductToCategory(Product product, Category category);
    void RemoveProductFromCategory(Product product, Category category);
}

public class ProductCategoryService : IProductCategoryService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    
    public ProductCategoryService(
        IProductRepository productRepository, 
        ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }
    
    public void AssignProductToCategory(Product product, Category category)
    {
        // Validate business rules across aggregates
        if (!category.IsActive)
            throw new InactiveCategoryException("Cannot assign products to inactive categories");
        
        if (product.IsDiscontinued)
            throw new InvalidOperationException("Cannot assign discontinued products");
        
        // Coordinate state changes across both aggregate roots
        product.AssignToCategory(category.Id);
        category.AddProduct(product.Id);
        
        // Persist both aggregates to maintain consistency
        _productRepository.Update(product);
        _categoryRepository.Update(category);
    }
    
    public void RemoveProductFromCategory(Product product, Category category)
    {
        if (!product.CategoryId.Equals(category.Id))
            throw new InvalidOperationException("Product is not assigned to this category");
        
        // Coordinate removal from both sides
        product.UnassignFromCategory();
        category.RemoveProduct(product.Id);
        
        _productRepository.Update(product);
        _categoryRepository.Update(category);
    }
}
```

The Domain Service orchestrates the assignment while keeping each aggregate focused on its own concerns: `Product` knows about its category membership, and `Category` knows about its product collection. The business rules that depend on both aggregates' state are centralized in the Domain Service, ensuring consistency across your application.

## Dependency Injection Registration

By default, the `Domain Service` will be registered with the dependency injection contain as `Transient`. However it is possible to change this default, as well as change the scope of an individual _Domain Service_.

> ⚠️ **NOTE**
>
> The scope set at an individual Domain Service level will always take precedence over the default scope.

### Global Default

The default scope for Domain Services can be setting under the `Domain Settings` section:

![Global scope](images/global-scope.png)

Available options are:

- Transient (the default)
- Scoped

### Individual Domain Service Scope

Changing the registration scope of an individual service can be done via the `Service Registration Scope` setting on the Domain Service itself:

![Service scope](images/service-scope.png)

Available options are:

- Transient (the default)
- Scoped
- Singleton

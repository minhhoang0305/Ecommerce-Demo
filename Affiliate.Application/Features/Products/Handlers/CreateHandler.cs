using MediatR;

public class CreateHandler : IRequestHandler<CreateCommand, Guid>
{
    private readonly IProductRepository _productRepository;

    public CreateHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Guid> Handle(CreateCommand request, CancellationToken cancellationToken)
    {
        var product = new Products
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Category = request.Category,
            Price = request.Price,
            Description = request.Description,
            Stock = request.Stock,
        };

        return await _productRepository.CreateAsync(product);
    }
}

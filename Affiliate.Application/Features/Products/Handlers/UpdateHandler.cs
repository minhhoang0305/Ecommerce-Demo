using MediatR;

public class UpdateHandler : IRequestHandler<UpdateCommand, Unit>
{
    private readonly IProductRepository _repository;
    public UpdateHandler(IProductRepository repository)
    {
        _repository = repository;
    }
    public async Task<Unit> Handle(UpdateCommand request, CancellationToken cancellationToken)
    {
        var existingProduct = await _repository.GetByIdAsync(request.Id);
        if (existingProduct == null)
        {
            throw new Exception("Product not found");
        }

        existingProduct.Name = request.Name;
        existingProduct.Price = request.Price;
        existingProduct.Description = request.Description;

        await _repository.UpdateAsync(existingProduct);
        return Unit.Value;
    }
}
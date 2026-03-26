using MediatR;
public class GetHandler : IRequestHandler<GetByIdAsync, Products?>
{
    private readonly IProductRepository _repository;

    public GetHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Products?> Handle(GetByIdAsync request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.Id);
    }
}
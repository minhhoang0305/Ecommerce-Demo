using MediatR;
public class GetAllHandler : IRequestHandler<GetByAllAsync, IEnumerable<Products>>
{
    private readonly IProductRepository _repository;

    public GetAllHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Products>> Handle(GetByAllAsync request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync();
    }
}
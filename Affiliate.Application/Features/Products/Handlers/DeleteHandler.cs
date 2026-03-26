using MediatR;


public class DeleteHandler : IRequestHandler<DeleteCommand, Unit>
{
    private readonly IProductRepository _repository;

    public DeleteHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.Id);
        return Unit.Value;
    }
}
using MediatR;
public record GetByAllAsync(): IRequest<IEnumerable<Products>>;
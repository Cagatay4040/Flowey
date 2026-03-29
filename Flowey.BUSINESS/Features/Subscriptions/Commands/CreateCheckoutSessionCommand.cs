using Flowey.CORE.Interfaces.Services;
using Flowey.CORE.Result.Abstract;
using Flowey.CORE.Result.Concrete;
using Flowey.SHARED.Constants;
using Flowey.SHARED.Enums;
using MediatR;

namespace Flowey.BUSINESS.Features.Subscriptions.Commands
{
    public  class CreateCheckoutSessionCommand : IRequest<IDataResult<string>>
    {
        public int MonthsToPurchase { get; set; } = 1;

        public CreateCheckoutSessionCommand(int monthsToPurchase)
        {
            MonthsToPurchase = monthsToPurchase;
        }
    }

    public class CreateCheckoutSessionCommandHandler : IRequestHandler<CreateCheckoutSessionCommand, IDataResult<string>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IPaymentService _paymentService;

        public CreateCheckoutSessionCommandHandler(ICurrentUserService currentUserService, IPaymentService paymentService)
        {
            _currentUserService = currentUserService;
            _paymentService = paymentService;
        }

        public async Task<IDataResult<string>> Handle(CreateCheckoutSessionCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId().Value;

            string checkoutUrl = await _paymentService.CreateCheckoutSessionAsync(userId, request.MonthsToPurchase);

            if (string.IsNullOrEmpty(checkoutUrl))
                return new DataResult<string>(ResultStatus.Error, null, Messages.PaymentGatewayCommunicationError);

            return new DataResult<string>(ResultStatus.Success, checkoutUrl, Messages.CheckoutSessionCreated);
        }
    }
}

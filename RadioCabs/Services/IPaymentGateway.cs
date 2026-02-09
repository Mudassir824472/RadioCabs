using System.Threading;
using System.Threading.Tasks;

namespace RadioCabs.Services
{
    public interface IPaymentGateway
    {
        bool IsConfigured { get; }

        Task<PaymentSessionResult> CreateCheckoutSessionAsync(PaymentCheckoutRequest request, CancellationToken cancellationToken);

        Task<PaymentSessionResult> GetCheckoutSessionAsync(string sessionId, CancellationToken cancellationToken);
    }
}

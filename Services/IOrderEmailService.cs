namespace E_commerceData.Services
{
    public interface IOrderEmailService
    {
        Task SendOrderEmailAsync(string email, string name, int orderId, IEnumerable<object> items, decimal totalPrice);
    }

    public class OrderEmailService : IOrderEmailService
    {
        private readonly IEmailSender _emailSender;

        public OrderEmailService(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task SendOrderEmailAsync(string email, string name, int orderId, IEnumerable<object> items, decimal totalPrice)
        {
            string subject = $"Order Confirmation #{orderId}";
            string html = $@"
            <h2>Hi {name},</h2>
            <p>Thank you for your order #{orderId}!</p>
            <h3>Order Summary:</h3>
            <ul>
                {string.Join("", items.Select(item =>
                        $"<li>{((dynamic)item).product_name} x{((dynamic)item).quantity} - ${((dynamic)item).price}</li>"))}
            </ul>
            <strong>Total: ${totalPrice}</strong>
            <p>We'll notify you when it ships.</p>";

            await _emailSender.SendEmailAsync(email, subject, html);
        }
    }
}
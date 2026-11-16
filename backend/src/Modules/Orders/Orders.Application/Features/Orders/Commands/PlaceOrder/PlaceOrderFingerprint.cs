using System.Security.Cryptography;
using System.Text;

namespace Orders.Application.Features.Orders.Commands.PlaceOrder;

/// <summary>
/// A hash of what the customer asked for. A repeat with the same idempotency key and the same hash is the same
/// order; the same key with another hash is a client bug and gets 409.
/// </summary>
internal static class PlaceOrderFingerprint
{
    public static string Of(PlaceOrderCommand command)
    {
        var text = new StringBuilder()
            .Append(command.CustomerName.Trim()).Append('|')
            .Append(command.Phone.Trim()).Append('|');
        foreach (var line in command.Lines.OrderBy(l => l.ProductId))
            text.Append(line.ProductId).Append('x').Append(line.Quantity).Append(';');

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text.ToString())));
    }
}

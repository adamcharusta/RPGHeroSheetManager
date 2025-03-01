using System.Text;
using Newtonsoft.Json;
using RPGHeroSheetManagerAPI.Infrastructure.RabbitMq.Common;

namespace RPGHeroSheetManagerAPI.Infrastructure.RabbitMq.Messages;

public class ConfirmEmailMessage : BaseMessage
{
    public ConfirmEmailMessage(string to, string token)
    {
        To = to;
        Token = token;
        var msg = new { To, Token };
        Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg));
    }

    public ConfirmEmailMessage(byte[] bytes)
    {
        var msg = JsonConvert.DeserializeObject<ConfirmEmailMessage>(Encoding.UTF8.GetString(bytes));
        To = msg!.To;
        Token = msg.Token;
    }

    private string To { get; }
    private string Token { get; }

    public override bool Durable => true;
}

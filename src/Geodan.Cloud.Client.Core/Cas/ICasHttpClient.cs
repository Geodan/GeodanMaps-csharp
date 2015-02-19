using System.Threading.Tasks;

namespace Geodan.Cloud.Client.Core.Cas
{
    public delegate void CasLoginSuccessfulHandler(object sender);
    public delegate void CasLoginFailedHandler(object sender, string error);

    public interface ICasHttpClient
    {
        string ErrorString { get; }
        bool ModAuthCas { get; set; }
        string Password { get; set; }
        string ServiceUrl { get; set; }
        string TicketServiceUrl { get; set; }
        string CasLoginRedirectUrl { get; set; }
        string Username { get; set; }

        event CasLoginSuccessfulHandler CasLoginSuccessful;
        event CasLoginFailedHandler CasLoginFailed;

        Task<bool> Login();
        Task<bool> Logout();
    }
}

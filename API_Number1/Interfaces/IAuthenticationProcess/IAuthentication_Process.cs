using API_Number1.DTO_S.User_DTO;

namespace API_Number1.Interfaces.IAuthenticationProcess
{
    public interface IAuthentication_Process
    {

        Task<IResult> UserAuthenticationProcess(SignInRequest signIn);
    }
}

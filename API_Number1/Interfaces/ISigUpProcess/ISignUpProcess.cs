using API_Number1.DTO_S.User_DTO;
using API_Number1.Models;

namespace API_Number1.Interfaces.ISigUpProcess
{
    public interface ISignUpProcess
    {
        Task<IResult> CreateUserProcess(SignUpRequest signUpRequest);

    }
}

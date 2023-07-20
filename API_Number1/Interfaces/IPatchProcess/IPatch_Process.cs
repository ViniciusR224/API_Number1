using API_Number1.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace API_Number1.Interfaces.IPatchProcess
{
    public interface IPatch_Process
    {
       Task<IResult> UserPatchProcess(Guid id,JsonPatchDocument<User> jsonPatchDocument);

    }
}

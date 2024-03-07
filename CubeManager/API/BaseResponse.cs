using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CubeManager.API;

public class BaseResponse<T>
{
    public string? Message { get; set; }
    public T Data { get; set; }
}

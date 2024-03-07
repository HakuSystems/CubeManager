namespace CubeManager.API;

public enum StatusRequests
{
    //200, 400, 500, 409, 408, 401
    //OK, RequestError, Error, Conflict, Timeout, Unauthorized
    Ok = 200,
    RequestError = 400,
    Error = 500,
    Conflict = 409,
    Timeout = 408,
    Unauthorized = 401
}
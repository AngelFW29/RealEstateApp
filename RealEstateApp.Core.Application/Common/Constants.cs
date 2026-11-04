namespace RealEstateApp.Core.Application
{
    public static class Roles
    {
        public const string Admin = "Administrador";
        public const string Developer = "Desarrollador";
        public const string Agent = "Agente";
        public const string Client = "Cliente";
    }

    public class Result
    {
        public List<string> Errors { get; set; } = new();
        public string? Message { get; set; }
        public bool NotFound { get; set; }
        public bool Success => Errors.Count == 0;
        public string FirstError => Errors.FirstOrDefault() ?? string.Empty;

        public static Result Ok(string? message = null) => new() { Message = message };
        public static Result Fail(params string[] errors) => new() { Errors = errors.ToList() };
        public static Result NotFoundResult(string error) => new() { Errors = { error }, NotFound = true };
    }

    public class Result<T> : Result
    {
        public T? Data { get; set; }
        public static Result<T> Ok(T data, string? message = null) => new() { Data = data, Message = message };
        public new static Result<T> Fail(params string[] errors) => new() { Errors = errors.ToList() };
    }
}
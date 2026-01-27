namespace EventManagementAPI.Commons
{
    public class DtoValidationResult
    {
        public bool IsValid { get; set; }
        public List<ResultDetails> Errors { get; set; }
    }

    public class ResultDetails
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
    }
}

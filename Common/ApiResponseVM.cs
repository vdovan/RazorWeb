namespace AppMvc.Common
{
    public class ApiResponseVm
    {
        public ApiResponseVm(){ 
            this.Ret = ResultType.FALSE;
        }
        public ResultType Ret { get; set; }
        public string? UserMsg { get; set; }
        public object? Msg { get; set; }
    }
}
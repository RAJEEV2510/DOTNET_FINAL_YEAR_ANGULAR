
namespace API.Error
{
    public class ApiExpection
    {
      
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }


       public   ApiExpection(int StatusCode ,string message=null,string details=null)
        {
            this.StatusCode = StatusCode;
            this.Message = message;
            this.Details = details;
        }

    }
}
